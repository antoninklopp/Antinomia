using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 
using UnityEngine.UI; 
using System; 
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour	 {
	/*
	 * TODO: Toutes les commandes de spawn doivent passer par player 
	 * avec à chaque fois 
	 * if (!isLocalPlayer)
	 * 
	 * Les Sync Var doivent être ici!
	 * 
	 * TODO; Réorganiser le code. 
	 */ 

	private Deck CardDeck = new Deck (); 
	// On sauvegarde les cartes déjà piochées, notamment pour pouvoir donner la possibilité de changer sa main de départ. 
	private List<GameObject> cartesPiochees = new List<GameObject> (); 

	GetPlayerInfoGameSparks playerInfo; 
	public GameObject CartePrefab;
    public GameObject SortPrefab; 
	bool cardOk = true; 

	public int PlayerID = 0; 
	public bool Local; 

	// L'ID de chaque carte.
#pragma warning disable CS0414 // Le champ 'Player.cardID' est assigné, mais sa valeur n'est jamais utilisée
	private int cardID = 1; 
#pragma warning restore CS0414 // Le champ 'Player.cardID' est assigné, mais sa valeur n'est jamais utilisée

	private GameObject ObjectInfo; 

	[SyncVar(hook="ChangePlayerPV")]
	public int PlayerPV = 100;
	[SyncVar(hook="ChangePlayerAKA")]
	public int PlayerAKA = 0; 

	[HideInInspector]
	public enum Phases{INITIATION, PIOCHE, PREPARATION, PRINCIPALE1, COMBAT, PRINCIPALE2, FINALE}; 

	[HideInInspector]
	// Copier-coller des capacités de la classe carte. 
	public enum PhasesCapacites{
		// Toutes les capacités possibles d'une entité. 
		PIOCHE, // Faire piocher une carte (TODO: implémenter le fait de pouvoir en piocher plusieurs)
		DEFAUSSER, // faire défausser une ou des cartes d'une main
		POSITION, // Modifier la position d'une entité
		NOMBRE_ATTAQUE,  // Modifier le nombre d'attaque
		VEROUILLER, // verouiller la position d'une entité
		DETRUIRE, // Détruire des entités. 
		MODIFIER_PUISSANCE, // Modifier la puissance d'autres entités
		DEGATS, // Infliger des dégâts au joueur adverse
		REVELER, // Révéler des cartes de la main d'un ou plusieurs joueurs. 
		RENVOYER, // Renvoyer des entités à la main. 
		CHANGEMENT_FAIBLESSE, // Changement des faiblesses face aux autres natures. 
		ANNULER_OBLIGATION, // Annuler l'obligation de d'abord attaquer les entités avant le joueur. 
		OBLIGER_ATTAQUE, // Obliger les autres entités à l'attaquer. 
		DETRUIRE_ASSISTANCE, // Détruire une/des assistances/cartes
		CONTROLE, // Prendre le contrôle d'une/plusieurs entités. 
		MODIFIER_AKA, // modifier l'AKA rémanent et/ou courant. 
		MODIFIER_NOMBRE_INVOCATION, // Modifier le nombre d'invocations élémentaires limite par tour. 
		MODIFIER_LIMITE_SORTS, //Modifier le nombre limite de sorts par tour. 
		EMPECHER_CHOIX_UNITE, // Empecher une/des unités d'être choisies. 
		NONE // Aucune Capacité
	}

	[SyncVar(hook="ChangementPhase")]
	/*
	 * TODO: Pour une raison totalement inconnue, changer Phase = newPhase ne marche pas 
	 * dans la fonction changement du hook. 
	 * Complètement incompréhensible pour l'instant. 
	 * 
	 * La variable myPhase est inutile btw. 
	 * 
	 */ 
	Phases Phase = Phases.INITIATION; 
	Phases myPhase = Phases.INITIATION; 

	[SyncVar(hook="ChangementTour")]
	int Tour; 

	// En règle général, il n'y pas de phases de capacité.
#pragma warning disable CS0414 // Le champ 'Player.PhaseCapacite' est assigné, mais sa valeur n'est jamais utilisée
	PhasesCapacites PhaseCapacite = PhasesCapacites.NONE; 
#pragma warning restore CS0414 // Le champ 'Player.PhaseCapacite' est assigné, mais sa valeur n'est jamais utilisée



	bool phaseChange = true; 

	GameObject NomJoueur1; 
	GameObject NomJoueur2;

    [SyncVar(hook="OnName")]
    string PlayerName = " ";

    // Use this for initialization
    void Start() {
        Local = isLocalPlayer;

        /*
		 * Le serveur aura toujours l'ID 1, le client l'ID 2; 
		 * TODO : Ne fonctionne pas. 
		 */

        if (isServer && isLocalPlayer || !isLocalPlayer && !isServer) {
            PlayerID = 1;
        }
        else {
            PlayerID = 2;
        }

        if (!isLocalPlayer) {
            return;
        }

        // On update le nombre de game jouées par le joueur
        MatchMakingGameSparks _matchMaking = new MatchMakingGameSparks();
        _matchMaking.updatePlayerGameCount();

        playerInfo = GetComponent<GetPlayerInfoGameSparks>();
        print("playerID");

        StartCoroutine(SetNames ()); 
        // setPlayerPV (PlayerPV); 
    }

    // Update is called once per frame
    void Update () {
		if (!isLocalPlayer)
		{
			return;
		}


		if (cardOk && isServer) {
			//StartCoroutine(CoroutineCard()); 
		}
		//Local = isLocalPlayer; 

		if (phaseChange) {
			//Debug.Log (Phase); 
			//Debug.Log (myPhase); 
			//Debug.Log ("---------------------------"); 
			StartCoroutine (PhaseChangeRoutine ());
		}
	}

	IEnumerator PhaseChangeRoutine(){
		phaseChange = false; 
		//GameObject.FindGameObjectWithTag ("GameManager").SendMessage ("setNamePhaseUI", Phase); 
		yield return new WaitForSeconds (1f); 
		phaseChange = true; 

	}

	public override void OnStartLocalPlayer()
	{
		/*
		 * Lorsque le local player commence. 
		 */  
		base.OnStartLocalPlayer ();
		//transform.localScale = new Vector3 (1, 1, 0); 

		if (!isLocalPlayer) {
			transform.localScale = new Vector3 (1, -1, 1);
		} else {
			transform.localScale = Vector3.one; 
		}
	}

	[Command]
	public void CmdSpawnCard(){
		GameObject.Find ("CartesMainJoueur1").SendMessage ("TirerCarte");
	}

	IEnumerator CoroutineCard(){
		PiocherNouvelleCarte (); 
		cardOk = false;
		yield return new WaitForSeconds (10f); 
		cardOk = true;

	}

	/* -------------- 
	 * Pioche d'une nouvelle carte par le joueur
	 * Les objets doivent visiblement être créés depuis l'instance joueur
	 * 
	 * TODO: Encore une raison inconnue: à trouver. 
	 * 
	 * Les deux fonctions fonctionnent très bien. 
	 * 
	 * Trouver comment s'y retrouver avec toutes les instances des objets!
	 */ 

	public void DetruireCartesDessusDeck(int nombre){
		/*
		 * Lors d'un cout élémentaire pour une carte Terre, il faut détruire les x premières cartes du jeu.
		 */ 
		CardDeck.Cartes.RemoveRange (0, nombre); 
	}

	public void PiocherNouvelleCarte(){
		if (!isLocalPlayer) {
			return;
		}

		StartCoroutine (PiocherCarteRoutine ()); 
	}

	public IEnumerator PiocherCarteRoutine(){
        // Comme on ne peut instancier que des objets de type prefab référencié (ici c'est l'objet public carte prefab). 
        // Normalement, comme on a créé le deck à partir du prefab, ça devrait fonctionner. 
        string oID = " "; 
        if (CardDeck.Cartes.Count != 0) {
            if (CardDeck.Cartes[0].GetComponent<CarteType>().thisCarteType == CarteType.Type.ENTITE) {
                oID = CardDeck.Cartes[0].GetComponent<Entite>().oID;
                Debug.Log("C'est une entité");
            }
            else if (CardDeck.Cartes[0].GetComponent<CarteType>().thisCarteType == CarteType.Type.SORT) {
                oID = CardDeck.Cartes[0].GetComponent<Sort>().oID;
                Debug.Log("C'est un sort");
            }
            else if (CardDeck.Cartes[0].GetComponent<CarteType>().thisCarteType == CarteType.Type.ASSISTANCE) {
                oID = CardDeck.Cartes[0].GetComponent<Assistance>().oID;
                Debug.Log("C'est une assistance");
            } else { 
                throw new Exception("Aucune OID n'a été donnée à la carte"); 
            }
            Debug.Log("OID avant routine" + oID.ToString());
            Debug.Log("PiocherRoutine"); 
            CmdPiocherNouvelleCarte1(oID);
        } else {
            Debug.Log(CardDeck.Cartes.Count); 
            throw new Exception("Impossible d'instancier la carte, il n'y a plus de cartes dans le deck");
        }
		yield return new WaitForSeconds (0.2f);
		CmdPiocherNouvelleCarte2 (oID); 
		cartesPiochees.Add (CardDeck.Cartes [0]); 
		CardDeck.Cartes.Remove (CardDeck.Cartes [0]);
	}


	public IEnumerator CreateDeck(){
		/*
		 * Création du deck au début de la partie.
		 * 
		 * Il faudra pouvoir faire un choix mais pour l'instant on récupère toutes les cartes du joueur. 
		 * Full bourrin. 
		 * 
		 * On attend une seconde pour récupérer les infos du serveur, sinon l'objet est vide. 
		 */ 

		Debug.Log ("LE DECK COMMENCE A ETRE CREE"); 
		if (PlayerPrefs.GetInt ("ChoixDeck") == 0) {
			yield return playerInfo.WaitForPlayerCards (CartePrefab); 
			CardDeck = playerInfo.GetAllCardsAsDeck (CartePrefab);
		} else {
			yield return playerInfo.WaitForPlayerDecks (CartePrefab, PlayerPrefs.GetInt("ChoixDeck")); 
			CardDeck = playerInfo.GetDeck (CartePrefab, PlayerPrefs.GetInt("ChoixDeck"));
		}
		//yield return new WaitForSeconds (0.3f); 
		MelangerDeck ();
        Debug.Log("LE DECK A ETE CREE"); 
	}

	void MelangerDeck(){
		/*
		 * Melange des cartes dans le deck. 
		 */ 
		Deck DeckRandom = new Deck (); 
		DeckRandom.number = CardDeck.number; 

		int random; 
		int index = 0;
		// Maintenant on met les cartes dans le désordre. 
		while (CardDeck.Cartes.Count > 0) {
			random = UnityEngine.Random.Range (0, CardDeck.Cartes.Count); 
			Debug.Log ("RANDOM : ------------------------------------------ " + random.ToString ()); 
			DeckRandom.Cartes.Add(CardDeck.Cartes [random]); 
			CardDeck.Cartes.Remove (CardDeck.Cartes [random]); 
			index++; 
		}

		CardDeck = DeckRandom; 


	}

	[Command]
	void CmdPiocherNouvelleCarte1(string oID){
		ObjectInfo = new GameObject();
		GetPlayerInfoGameSparks playerInfo = ObjectInfo.AddComponent<GetPlayerInfoGameSparks> ();
        Debug.Log("oID CmdPiocherNouvelleCarte" + oID.ToString()); 
		playerInfo.StartCardByoID(oID, CartePrefab); 
		//CardToInstantiate = playerInfo.GetCardoID(); 
		// bool isCardOK = false; 
		// On attend de récupérer l'objet
		// isCardOK = playerInfo.cardoIDOk; 
	}

	[Command]
	void CmdPiocherNouvelleCarte2(string oID){
		GameObject CardToInstantiate = ObjectInfo.GetComponent<GetPlayerInfoGameSparks>().GetCardoID(); 
		Debug.Log ("Carte instantiée");
        Debug.Log(CardToInstantiate);
        //CardToInstantiate.GetComponent<Carte> ().IDCardGame = cardID; 
        //cardID++;

        try {
            GameObject NouvelleCarte = Instantiate(CardToInstantiate) as GameObject;
            // On enlève la carte du deck. 
            // On passe la carte du deck à la main 
            if (NouvelleCarte.GetComponent<Entite>() != null) {
                NouvelleCarte.GetComponent<Entite>().setState("MAIN");
            }
		    NouvelleCarte.transform.SetParent (transform.Find("MainJoueur").Find("CartesMainJoueur")); 
		    // Après avoir mis la nouvelle carte sur le serveur, on lui indique à quel joueur elle appartient. 
		    // NouvelleCarte.GetComponent<Carte>().isFromLocalPlayer = true;  //TODO : Cette fonction met toutes les cartes du serveur à true. 
		    // On crée la nouvelle carte sur le serveur. 
		    NetworkServer.SpawnWithClientAuthority(NouvelleCarte, connectionToClient);
		    // Il faut réordonner les cartes. 
		    transform.Find("MainJoueur").Find("CartesMainJoueur").SendMessage("ReordonnerCarte");
                //StartCoroutine(NouvelleCarteRPCRoutine (ID), NouvelleCarte);

        } catch (ArgumentException e) {
            Debug.LogWarning(e);
            Debug.Log("La carte n'a pas pu êyre instanciée");
        }
        Destroy(CardToInstantiate); 
		Destroy (ObjectInfo); 
	}

	void ChangeUIPhase(Phases newPhase){
		/*
		 * Changer la variable synchronisée de la phase, côté client et serveur. 
		 */  

		if (!isLocalPlayer) {
			return; 
		}

		if (isServer) {
			Phase = newPhase; 
		} else {
			CmdChangePhase (newPhase);
		}
	}

	[Command] 
	void CmdChangePhase(Phases newPhase){
		/*
		 * Changer la phase du client vers le serveur
		 * 
		 * Ceci aurait peut-être pu être fait depuis le GameManager.
		 * NON Impossible car pas d'authority.
		 */ 
		Phase = newPhase; 
	}

	public void ChangementPhase(Phases newPhase){
		/*
		 * Comme la syncvar ne fonctionne pas, on envoie directement la nouvelle variable vers le gameManager. 
		 */ 
		Phase = newPhase; 
		myPhase = newPhase; 
		ChangementBis (newPhase); 
		GameObject.FindGameObjectWithTag ("GameManager").SendMessage ("setNamePhaseUI", Phase); 
	}

	public void ChangementBis(Phases newPhase){
		Phase = newPhase; 
	}

	public void ChangementTour(int newTour){
		/*
		 * Changement de Tour. 
		 */ 
		GameObject.FindGameObjectWithTag ("GameManager").SendMessage ("setTour", newTour);
	}

	void ChangeUITour(int newTour){

		if (!isLocalPlayer)
			return; 


		if (isServer) {
			Tour = newTour; 
			RpcChangeTour (newTour); 
		} else {
			CmdChangeTour (newTour); 
			ChangementTour (newTour); 
		}

	}

	[Command]
	void CmdChangeTour(int newTour){
		Tour = newTour; 
	}

	[ClientRpc]
	void RpcChangeTour(int newTour){
		ChangementTour (newTour); 
	}

	IEnumerator SetNames(){
		/*
		 * Mettre les noms des joueurs
         * On attend un peu, pour être sur que les deux jours sont sur le serveur. 
		 */ 
		yield return new WaitForSeconds(0.2f);
        //NomJoueur1 = GameObject.FindGameObjectWithTag("GameManager").transform.Find ("NomJoueur1").gameObject; 
        //NomJoueur2 = GameObject.FindGameObjectWithTag("GameManager").transform.Find ("NomJoueur2").gameObject; 

#if UNITY_EDITOR
        CmdSetName("device");
#else
        CmdSetName(PlayerPrefs.GetString("user")); 
#endif

    }

    [ClientRpc]
	public void RpcSetNameOpponent(string name){
		NomJoueur2.GetComponent<Text> ().text = name; 
	}


    void OnName(string newName) {
        /*
         * Appelé par la syncVar du nom. 
         */
        PlayerName = newName;

        StartCoroutine(RoutineSetName(newName));
    }

    IEnumerator RoutineSetName(string newName) {
        /*
         * On utilise une coroutine pour attendre que tous les objets soient "arrivés" dans la partie. 
         */ 
        yield return new WaitForSeconds(0.2f); 
        NomJoueur1 = GameObject.FindGameObjectWithTag("GameManager").transform.Find("NomJoueur1").gameObject;
        NomJoueur2 = GameObject.FindGameObjectWithTag("GameManager").transform.Find("NomJoueur2").gameObject;
        if (isLocalPlayer) {
            // Joueur local
            NomJoueur1.GetComponent<Text>().text = newName;
        }
        else {
            NomJoueur2.GetComponent<Text>().text = newName;
        }

    }

	[Command]
	public void CmdSetName(string Name){
        /*
         * On veut transmettre à l'autre joueur le nom du player
		 */
        // Debug.Log ("Nom du joueur2"); 
        // GameObject.FindGameObjectWithTag("GameManager").transform.Find ("NomJoueur2").gameObject.GetComponent<Text> ().text = Name; 
        PlayerName = Name; 
    }

	void DetruireCarte(int ID){
		CmdDetruireCarte (ID); 
	}

	[Command]
	void CmdDetruireCarte(int ID){
		/*
		 * Lorsqu'on veut détruire une carte qui n'est pas du joueur local, 
		 * on ne peut pas envoyer une fonction command depuis cet object, 
		 * car le joueur n'a pas d'autorité sur l'objet, 
		 * comme ce n'est pas lui qui l'a spawn sur le network. 
		 * 
		 * il faut donc envoyer la carte au joueur local, 
		 * qui peut lui detruire la carte sur le réseau en lui envoyant la commande
		 * 
		 */ 
		Debug.Log ("Execute sur le serveur"); 
		GameObject LaCarteSurLeServeur = FindCardWithID (ID); 
		LaCarteSurLeServeur.SendMessage ("DetruireCarte"); 
	}

    [Command]
    public void CmdEnvoiMethodToServerCarteWithIntParameter(int ID, string voidName, int _intToUse) {
        /*
         * Lorsqu'on veut appliquer un effet sur une carte qui n'appartient pas au joueur, 
         * il faut passer par la carte sur le serveur, 
         * car sinon on n'a pas autorité sur la carte présente chez le joueur. 
         * 
         * Arguments:
         * ID : ID de la carte (ID dans la partie != oID)
         * voidName: la fonction a éxécuté sur la carte sur le serveur, qui ne doit avoir qu'un paramètre qui est un int
         * _intToUse : le paramètre
         * 
         */
        Debug.Log("Execute sur le serveur.");
        GameObject LaCarteSurLeServeur = FindCardWithID(ID);
        LaCarteSurLeServeur.SendMessage(voidName, _intToUse); 
    }

	GameObject FindCardWithID(int _ID_){
		/*
		 * Trouver la carte avec la bonne ID. 
		 */ 
		GameObject[] AllCartes = GameObject.FindGameObjectsWithTag ("BoardSanctuaire"); 
		for (int i = 0; i < AllCartes.Length; ++i) {
			// On cherche la carte avec le bon ID
			if (AllCartes [i].GetComponent<Entite> ().IDCardGame == _ID_) {
				return AllCartes [i]; 
			}
		}
		throw new Exception ("La carte n'a pas été trouvée"); 
		// return null; 
	}


	public void HealPlayer(int puissance){
		/*
		 * Par un effet ou une capacité, le joueur peut se faire "soigner", 
		 * ou au moins gagner des PVs. 
		 * 
		 * On met ensuite l'information en ligne sur le serveur. 
		 */ 
		PlayerPV += puissance; 
		CmdSetPlayerPV (PlayerPV); 
		Debug.Log ("UN HEAL"); 
		//GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ().setPlayerPVUI (PlayerID, PlayerPV); 
	}

	public void AttackPlayer(int puissance){
		/*
		 * Le joueur peut être directement attaqué si l'autre joueur n'a aucune autre carte 
		 * ni sur son board ni sur son sanctuaire.
		 * 
		 * Ici l'attaque est dirigée vers le joueur adverse sur lequel on n'a pas authority. 
		 * On ne peut donc pas envoyer une fonction depuis lui.
		 * Il faut donc transmettre l'information à notre joueur qui l'enverra. 
		 * 
		 */ 

		PlayerPV -= puissance; 
		if (isServer || isLocalPlayer) {
			CmdSetPlayerPV (PlayerPV);
		} else {
			// On est obligé d'envoyer l'information à l'autre joueur seulement s'il est ni le joueur local ni le serveur. 
			Debug.Log ("J'envoie le setPV à l'autre joueur"); 
			FindLocalPlayer ().GetComponent<Player>().CmdSetOtherPlayerLife(PlayerPV); 
		}

		if (PlayerPV <= 0) {
			// Le joueur a perdu. 
			OnPlayerLoses(); 
		}
		Debug.Log ("UNE ATTAQUE"); 
	}

	// ----- Mise à jour des informations sur le serveur. -------
	[Command]
	void CmdSetPlayerPV(int newPV){
		Debug.Log ("J'ai de nouveaux points de vie"); 
		PlayerPV = newPV; 
	}

	void ChangePlayerPV(int newPV){
		PlayerPV = newPV; 
		GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ().setPlayerPVUI (PlayerID, PlayerPV); 
	}

	[Command]
	void CmdSetOtherPlayerLife(int newPV){
		Debug.Log ("Salut je suis le joueur 1"); 
		FindPlayerWithID (1).GetComponent<Player>().CmdSetPlayerPV(newPV); 
	}

	GameObject FindNotLocalPlayer(){
		/*
		 * Trouve le joueur qui n'est pas local. 
		 */ 
		GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player"); 
		if (Players [0].GetComponent<Player>().isLocalPlayer) {
			return Players [1]; 
		} else {
			return Players [0]; 
		}
	}

	GameObject FindLocalPlayer(){
		GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player"); 
		if (Players [0].GetComponent<Player>().isLocalPlayer) {
			return Players [0]; 
		} else {
			return Players [1]; 
		}
	}

	GameObject FindPlayerWithID(int ID){
		/*
		 * trouver le joueur avec le bon ID. 
		 */ 
		GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player"); 
		if (Players [0].GetComponent<Player>().PlayerID == ID) {
			return Players [0]; 
		} else {
			return Players [1]; 
		}
	}


//	void setPlayerPV(int newPV){
//		PlayerPV = newPV; 
//		GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ().setPlayerPVUI (PlayerID, newPV); 
//	}

	// ------ Fin mise à jour des informations sur le serveur ------
	void setPlayerAKADebutTour(int newAKA){
		/*
		 * Au début du tour, on met à jour l'AKA des deux joueurs
		 */ 

		// Pour le joueur local. 
		CmdChangeBothPlayerAKA (newAKA);
	}

	[Command]
	void CmdChangeBothPlayerAKA(int newAKA){
		/*
		 * On change l'AKA des deux joueurs sur le serveur. 
		 */ 
		GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player"); 
		Players [0].GetComponent<Player> ().PlayerAKA = newAKA;
		try{
			Players [1].GetComponent<Player> ().PlayerAKA = newAKA;
#pragma warning disable CS0168 // La variable 'e' est déclarée, mais jamais utilisée
		} catch (IndexOutOfRangeException e){
#pragma warning restore CS0168 // La variable 'e' est déclarée, mais jamais utilisée
			Debug.Log ("Normalement on doit être au premier tour"); 
		}
	}

	void setPlayerAKA(int newAKA){
		/*
		 * Changer l'AKA du joueur
		 */ 
		PlayerAKA = newAKA;
		CmdChangePlayerAKA (PlayerAKA); 
	}

	[Command]
	void CmdChangePlayerAKA(int AKA){
		PlayerAKA = AKA; 
	}

	// Fonction appelée par la syncvar
	void ChangePlayerAKA(int newAKA){
		PlayerAKA = newAKA; 
		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ().setPlayerAKAUI (PlayerID, PlayerAKA); 
	}

	public void subtractAKA(int AKAToSubtract){
		PlayerAKA -= AKAToSubtract;
		setPlayerAKA (PlayerAKA); 
	}


	// ---- Fin du jeu : Le joueur gagne ou perd. 

	void OnPlayerLoses(){
		/*
		 * Quand le joueur perd
		 */ 
		SceneManager.LoadScene ("MainMenu"); 
		if (isServer) {
			RpcOnPlayerWins (); 
		} else {
			CmdOnPlayerWins (); 
		}
	}

	[Command]
	void CmdOnPlayerWins(){
		/*
		 * Quand le joueur perd, l'autre gagne. 
		 */ 
		SceneManager.LoadScene ("MainMenu"); 	
	}

	[ClientRpc]
	void RpcOnPlayerWins(){
		/*
		 * Quand le joueur perd, l'autre joueur gagne. 
		 */ 

		SceneManager.LoadScene ("MainMenu"); 
	}

	// --------- Montrer des cartes au joueur. 
	[Command]
	public void CmdSendCards(string[] allCards){
		//newListSync = new SyncListString (); 
		//		for (int i = 0; i < allCards.Length; ++i) {
		//			Debug.Log (allCards [i]); 
		//			newListSync.Add (allCards [i]);
		//		}
		//		AllCardsToShowOther = newListSync;  
		RpcShowCardsToOtherPlayer (allCards); 
	}

	[ClientRpc]
	public void RpcShowCardsToOtherPlayer (string[] allCards){
		GameObject.FindGameObjectWithTag ("GameManager").transform.Find ("ShowCards").gameObject.SendMessage ("RpcShowCardsToOtherPlayer", allCards); 
	}

}
