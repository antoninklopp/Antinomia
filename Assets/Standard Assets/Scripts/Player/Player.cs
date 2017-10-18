﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 
using UnityEngine.UI; 
using System; 
using UnityEngine.SceneManagement;

/// <summary>
/// Classe représentant l'objet "joueur". 
/// </summary>
public class Player : NetworkBehaviourAntinomia	 {
	/*
	 * TODO: Toutes les commandes de spawn doivent passer par player 
	 * avec à chaque fois 
	 * if (!isLocalPlayer)
	 * 
	 * Les Sync Var doivent être ici!
	 * TODO; Réorganiser le code. 
	 */ 

	private Deck CardDeck = new Deck (); 
	// On sauvegarde les cartes déjà piochées, notamment pour pouvoir donner la possibilité de changer sa main de départ. 
	private List<GameObject> cartesPiochees = new List<GameObject> (); 

	GetPlayerInfoGameSparks playerInfo; 
	public GameObject CartePrefab;
    public GameObject SortPrefab; 
	bool cardOk = true; 

    /// <summary>
    /// ID du joueur
    /// Le serveur aura toujours l'ID 1, 
    /// le client aura toujours l'ID 2. 
    /// </summary>
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

    /// <summary>
    /// Le tour en cours. 
    /// Il est égal à l'ID du joueur dont c'est le tour. 
    /// </summary>
	[SyncVar(hook="ChangementTour")]
	int Tour;

	bool phaseChange = true; 

	GameObject NomJoueur1; 
	GameObject NomJoueur2;

    [SyncVar(hook="OnName")]
    string PlayerName = " ";

    /// <summary>
    /// Le prefab de la pile d'effets. 
    /// </summary>
    public GameObject PilePrefab;

    // Use this for initialization
    public override void Start() {
        base.Start(); 
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
    public override void Update () {
        base.Update(); 
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

        int nombreDeCartesMain = transform.Find("MainJoueur").Find("CartesMainJoueur").childCount; 
        if (CardDeck.Cartes.Count != 0) {
            if (CardDeck.Cartes[0].GetComponent<CarteType>().thisCarteType == CarteType.Type.ENTITE) {
                oID = CardDeck.Cartes[0].GetComponent<Entite>().oID;
            }
            else if (CardDeck.Cartes[0].GetComponent<CarteType>().thisCarteType == CarteType.Type.SORT) {
                oID = CardDeck.Cartes[0].GetComponent<Sort>().oID;
            }
            else if (CardDeck.Cartes[0].GetComponent<CarteType>().thisCarteType == CarteType.Type.ASSISTANCE) {
                oID = CardDeck.Cartes[0].GetComponent<Assistance>().oID;
            } else { 
                throw new Exception("Aucune OID n'a été donnée à la carte"); 
            }
            // TestConnection(); 
            Debug.Log("On pioche une carte"); 
            CmdPiocherNouvelleCarte1(oID);
        } else {
            Debug.Log(CardDeck.Cartes.Count); 
            throw new Exception("Impossible d'instancier la carte, il n'y a plus de cartes dans le deck");
        }
		yield return new WaitForSeconds (0.2f);
		CmdPiocherNouvelleCarte2 (oID); 
		cartesPiochees.Add (CardDeck.Cartes [0]); 
		CardDeck.Cartes.Remove (CardDeck.Cartes [0]);

        // On regarde si le nombre de cartes du joueur a augmenté (dans le cas où le il y aurait un probleme lors de la pioche). 
        yield return new WaitForSeconds(0.1f);
        // L'objet a été détruit donc c'est bon.
        CmdTestIfObjectInfoDestroyed(15); 
	}

    [Command]
    void CmdTestIfObjectInfoDestroyed(int nombreEssais) {
        if(ObjectInfo != null) {
            Debug.Log("<color=red>L'Object Info n'a pas été détruit</color>");
            RpcTestIfObjectInfoDestroyed(false, nombreEssais); 
        } else {
            Debug.Log("<color=red>L'Object Info a été détruit</color>");
            RpcTestIfObjectInfoDestroyed(true, nombreEssais); 
        }
    }

    [ClientRpc]
    void RpcTestIfObjectInfoDestroyed(bool destroyed, int nombreEssais) {
        if (!isLocalPlayer) {
            return;
        } 
        if (destroyed) {
            AntinomiaLog("La carte a bien été créée"); 
        } else {
            StartCoroutine(RepiocherCarte(--nombreEssais));
            AntinomiaLog("On retente de piocher"); 
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nombreTests"></param>
    /// <returns></returns>
    IEnumerator RepiocherCarte(int nombreTests) {
        Debug.Log("On tente de repiocher la carte"); 
        CmdPiocherNouvelleCarte2("fbhkbh");
        yield return new WaitForSeconds(0.1f); 
        if (nombreTests > 0) {
            CmdTestIfObjectInfoDestroyed(nombreTests);
        } else {
            AntinomiaLog("En attendant encore la carte n'a quand même pas pu être récupérée. "); 
        }
    }

    /// <summary>
    /// Creation du deck. 
    /// Appelle aussi la fonction qui mélange le deck. 
    /// </summary>
    /// <returns></returns>
	public IEnumerator CreateDeck(){
		/*
		 * Création du deck au début de la partie.
		 * 
		 * Il faudra pouvoir faire un choix mais pour l'instant on récupère toutes les cartes du joueur. 
		 * Full bourrin. 
		 * 
		 * On attend une seconde pour récupérer les infos du serveur, sinon l'objet est vide. 
		 */ 
         
		if (PlayerPrefs.GetInt ("ChoixDeck") == 0) {
			yield return playerInfo.WaitForPlayerCards (CartePrefab); 
			CardDeck = playerInfo.GetAllCardsAsDeck (CartePrefab);
		} else {
			yield return playerInfo.WaitForPlayerDecks (CartePrefab, PlayerPrefs.GetInt("ChoixDeck")); 
			CardDeck = playerInfo.GetDeck (CartePrefab, PlayerPrefs.GetInt("ChoixDeck"));
		}

		MelangerDeck ();
	}

	public void MelangerDeck(){
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

    /// <summary>
    /// Permet de récupérer les X premières cartes du haut du deck. 
    /// </summary>
    /// <param name="nombre">nombre de cartes à récupérer</param>
    public List<GameObject> RecupererCartesDessusDeck(int nombre) {
        List<GameObject> _liste = new List<GameObject>(); 
        for (int i = 0; i < nombre; ++i) {
            _liste.Add(CardDeck.Cartes[i]); 
        }
        return _liste;
    }

	[Command]
	void CmdPiocherNouvelleCarte1(string oID){
		ObjectInfo = new GameObject();
		GetPlayerInfoGameSparks playerInfo = ObjectInfo.AddComponent<GetPlayerInfoGameSparks> ();
		playerInfo.StartCardByoID(oID, CartePrefab); 
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="oID">ID de la carte dans la metaCollection</param>
	[Command]
	void CmdPiocherNouvelleCarte2(string oID){
		GameObject CardToInstantiate = ObjectInfo.GetComponent<GetPlayerInfoGameSparks>().GetCardoID(); 
        Debug.Log("Carte instanciee" + CardToInstantiate);

        Debug.Log("<color=green> ON PIOCHE LA CARTE ICI </color>"); 
        try {
            GameObject NouvelleCarte = Instantiate(CardToInstantiate) as GameObject;
            // On enlève la carte du deck. 
            // On passe la carte du deck à la main 
            if (NouvelleCarte.GetComponent<Entite>() != null) {
                NouvelleCarte.GetComponent<Entite>().setState("MAIN");
            }
		    NouvelleCarte.transform.SetParent (transform.Find("MainJoueur").Find("CartesMainJoueur")); 
		    // On crée la nouvelle carte sur le serveur. 
		    NetworkServer.SpawnWithClientAuthority(NouvelleCarte, connectionToClient);
		    // Il faut réordonner les cartes. 
		    transform.Find("MainJoueur").Find("CartesMainJoueur").SendMessage("ReordonnerCarte");
            //StartCoroutine(NouvelleCarteRPCRoutine (ID), NouvelleCarte);

            Destroy(ObjectInfo);
        } catch (ArgumentException e) {
            // Ici la carte n'est pas arrivée "à temps" à l'objet. Il faut donc rappeler cette fonction
            Debug.LogWarning(e);
            Debug.Log("La carte n'a pas pu être instanciée");
        }
        Destroy(CardToInstantiate); 
	}



	void ChangeUIPhase(Phases newPhase){
		/*
		 * Changer la variable synchronisée de la phase, côté client et serveur. 
         * 
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

    /// <summary>
    /// Afficher le nom des deux joueurs. 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Detruire une carte. 
    /// </summary>
    /// <param name="ID">ID de la carte à détruire. </param>
	void DetruireCarte(int ID){
		CmdDetruireCarte (ID); 
	}

    /// <summary>
    /// Envoyer une carte au cimetiere. 
    /// </summary>
    /// <param name="ID">ID de la carte à détruire</param>
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
        RpcDetruireCarte(ID); 
	}

    /// <summary>
    /// Detruire la carte sur le client. 
    /// </summary>
    /// <param name="ID">IDCardGame de la carte</param>
    [ClientRpc]
    void RpcDetruireCarte(int ID) {
        // Si cette méthode a été appelée c'est pour être éxécutée sur une carte qui n'était pas du joueur local.
        if (!isLocalPlayer) {
            AntinomiaLog("Detruire carte sur le client"); 
            GameObject LaCarteSurLeServeur = FindCardWithID(ID);
            AntinomiaLog(ID); 
            LaCarteSurLeServeur.SendMessage("DetruireCarte");
        }
    }

    /// <summary>
    /// Envoi d'une méthode qui doit être éxecutée par l'autre joueur. 
    /// </summary>
    /// <param name="ID">l'ID de la carte dans le partie</param>
    /// <param name="voidName">Le string de la méthode à utiliser</param>
    /// <param name="_intToUse">l'entier demandé par la méthode</param>
    [Command]
    public void CmdEnvoiMethodToServerCarteWithIntParameter(int ID, string voidName, int _intToUse, int playerID) {
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
        // GameObject LaCarteSurLeServeur = FindCardWithID(ID);
        // LaCarteSurLeServeur.SendMessage(voidName, _intToUse); 
        RpcEnvoiMethodToServerCarteWithIntParameter(ID, voidName, _intToUse, playerID); 
    }

    [ClientRpc]
    public void RpcEnvoiMethodToServerCarteWithIntParameter(int ID, string voidName, int _intToUse, int playerID) {
        if (isLocalPlayer) {
            // Le but est d'envoyer une méthode à l'autre joueur!
            // Donc pas au joueur local!
            return; 
        }
        GameObject LaCarteSurLeServeur = FindCardWithID(ID);
        LaCarteSurLeServeur.SendMessage(voidName, _intToUse);
        Debug.Log("La méthode " + voidName + " a été éxecutée sur le serveur"); 
    }

    /// <summary>
    /// Rajouter des points de vie au joueur. 
    /// </summary>
    /// <param name="puissance">nombre de points de vie à rajouter au joueur. </param>
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

    /// <summary>
    /// Attaquer le joueur directement. 
    /// </summary>
    /// <param name="puissance">Nombre de PV enlevés au joueur</param>
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

    /// <summary>
    /// Changer l'AKA du joueur
    /// Non utilisé par la syncvar. 
    /// </summary>
    /// <param name="newAKA">Nouvelle valeur de l'AKA</param>
	void setPlayerAKA(int newAKA){
		/*
		 * Changer l'AKA du joueur
		 */ 
		PlayerAKA = newAKA;
		CmdChangePlayerAKA (PlayerAKA); 
	}

    /// <summary>
    /// Changer l'AKA du joueur, sur le serveur
    /// </summary>
    /// <param name="AKA">Nouvelle valeur de l'AKA</param>
	[Command]
	void CmdChangePlayerAKA(int AKA){
		PlayerAKA = AKA; 
	}

    /// <summary>
    /// Changer l'AKA du joueur
    /// Utilisé par la syncvar
    /// </summary>
    /// <param name="newAKA"></param>
	void ChangePlayerAKA(int newAKA){
        // Fonction appelée par la syncvar
        PlayerAKA = newAKA; 
		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ().setPlayerAKAUI (PlayerID, PlayerAKA); 
	}

    /// <summary>
    /// Enlever de l'AKA au joueur
    /// </summary>
    /// <param name="AKAToSubtract">Valeur de l'AKA à enlever</param>
	public void subtractAKA(int AKAToSubtract){
		PlayerAKA -= AKAToSubtract;
		setPlayerAKA (PlayerAKA); 
	}

    /// <summary>
    /// Ajouter de l'AKA au joueur
    /// </summary>
    /// <param name="AKAToAdd">Valeur de l'AKA à ajouter</param>
    public void addAKA(int AKAToAdd) {
        PlayerAKA += AKAToAdd;
        setPlayerAKA(PlayerAKA); 
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

    /// <summary>
    /// Montrer des cartes (souvent choisies lors d'un effet à un l'autre joueur)
    /// Execute sur le serveur. 
    /// </summary>
    /// <param name="allCards">La liste des shortCodes des cartes à montrer. </param>
    [Command]
	public void CmdSendCards(string[] allCards){
		RpcShowCardsToOtherPlayer (allCards); 
	}

    /// <summary>
    /// Montrer des cartes (souvent choisies lors d'un effet à un l'autre joueur)
    /// Execute sur les clients
    /// </summary>
    /// <param name="allCards">La liste des shortCodes des cartes à montrer. </param>
	[ClientRpc]
	public void RpcShowCardsToOtherPlayer (string[] allCards){
		GameObject.FindGameObjectWithTag ("GameManager").transform.Find ("ShowCards").
            gameObject.SendMessage ("RpcShowCardsToOtherPlayer", allCards);
    }

    /// <summary>
    /// Mettre le jeu en pause. 
    /// Execute sur le serveur.
    /// </summary>
    /// <param name="gameIsPaused"></param>
    [Command]
    public void CmdSetGameToPause(bool gameIsPaused) {
        // GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().gameIsPaused = gameIsPaused;
        RpcSetGameToPause(gameIsPaused); 
    }

    /// <summary>
    /// Mettre le jeu en pause.
    /// Execute sur le client.
    /// </summary>
    /// <param name="gameIsPaused"></param>
    [ClientRpc]
    private void RpcSetGameToPause(bool gameIsPaused) {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GameIsSetToPause(gameIsPaused); 
    }

    /// <summary>
    /// Contrairement à <see cref="CmdSetGameToPause(bool)"/> cette fonction ne fait que changer la valeur de pause. 
    /// Elle est utilé avant de pouvoir réagir à un effet.
    /// C'est-à-dire que si on peut réagir, le jeu est forcément "pas en pause"
    /// </summary>
    /// <param name="gameIsPaused"></param>
    [Command]
    public void CmdOnlySetPause(bool gameIsPaused) {
        RpcOnlySetPause(gameIsPaused); 
    }

    /// <summary>
    /// <see cref="CmdOnlySetPause(bool)"/>
    /// </summary>
    [ClientRpc]
    private void  RpcOnlySetPause(bool gameIsPaused) {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().gameIsPaused = gameIsPaused; 
    }

    /// <summary>
    /// Creer une nouvelle pile d'effet. 
    /// </summary>
    [Command]
    public void CmdCreerPile() {
        GameObject NouvellePile = Instantiate(PilePrefab); 
        NetworkServer.SpawnWithClientAuthority(NouvellePile, connectionToClient);
    }

    /// <summary>
    /// Detruire la pile une fois que tous les effets ont été executés. 
    /// </summary>
    /// <param name="Pile">La pile à détruire</param>
    [Command]
    public void CmdDetruirePile(GameObject Pile) {
        NetworkServer.Destroy(Pile); 
    }

    /// <summary>
    /// Jouer l'effet si le joueur n'a pas autorité sur la pile
    /// </summary>
    [Command]
    public void CmdJouerEffet() {
        Debug.Log("CmdJouerEffet"); 
        RpcJouerEffet(PlayerID); 
    }

    /// <summary>
    /// Jouer effet si le joueur n'a pas autorité sur la pile. 
    /// </summary>
    /// <param name="playerID">ID du joueur qui n'a pas autorité.</param>
    [ClientRpc]
    public void RpcJouerEffet(int playerID) {
        AntinomiaLog("RpcJouerEffet"); 
        // Si on appelle cette la fonction, c'est forcément depuis le joueur local, 
        // on veut donc executer la pile sur le joueur non local.
        if (!isLocalPlayer) {
            AntinomiaLog("Je suis le joueur qui a créé la pile"); 
            GameObject.FindGameObjectWithTag("Pile").GetComponent<PileAppelEffet>().DefaireLaPile(); 
        }
    }

    /// <summary>
    /// Ajouter un effet à la pile si le joueur n'a pas autorité. 
    /// </summary>
    /// <param name="IDObjetEffet"></param>
    /// <param name="ListeObjetsCible"></param>
    /// <param name="numeroEffet"></param>
    /// <param name="numeroListeEffet"></param>
    /// <param name="PlayerID"></param>
    [Command]
    public void CmdAjouterEffetALaPile(int IDObjetEffet, int[] ListeObjetsCible, int numeroEffet,
                                      int numeroListeEffet, int PlayerID) {
        RpcAjouterEffetALaPile(IDObjetEffet, ListeObjetsCible, numeroEffet, numeroListeEffet, PlayerID); 
    }

    /// <summary>
    /// Ajouter un effet à la pile si le joueur a autorité. 
    /// </summary>
    /// <param name="IDObjetEffet"></param>
    /// <param name="ListeObjetsCible"></param>
    /// <param name="numeroEffet"></param>
    /// <param name="numeroListeEffet"></param>
    /// <param name="PlayerID"></param>
    [ClientRpc]
    public void RpcAjouterEffetALaPile(int IDObjetEffet, int[] ListeObjetsCible, int numeroEffet,
                                      int numeroListeEffet, int PlayerID) {
        GameObject.FindGameObjectWithTag("Pile").GetComponent<PileAppelEffet>().RpcAjouterEffetALaPile(IDObjetEffet, ListeObjetsCible,
                                                                    numeroEffet, numeroListeEffet, PlayerID); 
    }

    /// <summary>
    /// Si le joueur X ne veut pas répondre au dernier effet du joueur Y, 
    /// on propose au joueur Y de défaire la pile d'effets.
    /// Fonction appelée sur le serveur. 
    /// </summary>
    /// <param name="PlayerID">ID du player qui appelle la fonction, ici le joueur X</param>
    [Command]
    public void CmdProposerDefaireLaPile(int PlayerID) {
        RpcProposerDefaireLaPile(PlayerID); 
    }

    /// <summary>
    /// equivalent sur le client de <see cref="CmdProposerDefaireLaPile(int)"/>
    /// </summary>
    /// <param name="PlayerID">ID du player qui appelle la fonction, ici le joueur X</param>
    [ClientRpc]
    public void RpcProposerDefaireLaPile(int PlayerID) {
        if (PlayerID != FindLocalPlayer().GetComponent<Player>().PlayerID) {
            StartCoroutine(getGameManager().GetComponent<GameManager>().ProposerDefaireLaPile()); 
        }
    }


    /// ////////////////////////////////////////////////////////////////////////////////////////////
    ///                       TEST DU RESEAU                                                     //
    /// ///////////////////////////////////////////////////////////////////////////////////////////

    string testStatus = "Testing network connection capabilities.";
    string testMessage = "Test in progress";
    string shouldEnableNatMessage = "";
    bool doneTesting = false;
    bool probingPublicIP = false;
    int serverPort = 9999;
    bool useNat = false;
    float timer = 0.0f;
    ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;

    void TestConnection() {
        // Start/Poll the connection test, report the results in a label and
        // react to the results accordingly
        connectionTestResult = Network.TestConnection();
        switch (connectionTestResult) {
            case ConnectionTesterStatus.Error:
                testMessage = "Problem determining NAT capabilities";
                doneTesting = true;
                break;

            case ConnectionTesterStatus.Undetermined:
                testMessage = "Undetermined NAT capabilities";
                doneTesting = false;
                break;

            case ConnectionTesterStatus.PublicIPIsConnectable:
                testMessage = "Directly connectable public IP address.";
                useNat = false;
                doneTesting = true;
                break;

            // This case is a bit special as we now need to check if we can
            // circumvent the blocking by using NAT punchthrough
            case ConnectionTesterStatus.PublicIPPortBlocked:
                testMessage = "Non-connectable public IP address (port " +
                    serverPort + " blocked), running a server is impossible.";
                useNat = false;
                // If no NAT punchthrough test has been performed on this public
                // IP, force a test
                if (!probingPublicIP) {
                    connectionTestResult = Network.TestConnectionNAT();
                    probingPublicIP = true;
                    testStatus = "Testing if blocked public IP can be circumvented";
                    timer = Time.time + 10;
                }
                // NAT punchthrough test was performed but we still get blocked
                else if (Time.time > timer) {
                    probingPublicIP = false;        // reset
                    useNat = true;
                    doneTesting = true;
                }
                break;

            case ConnectionTesterStatus.PublicIPNoServerStarted:
                testMessage = "Public IP address but server not initialized, " +
                    "it must be started to check server accessibility. Restart " +
                    "connection test when ready.";
                break;

            case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
                testMessage = "Limited NAT punchthrough capabilities. Cannot " +
                    "connect to all types of NAT servers. Running a server " +
                    "is ill advised as not everyone can connect.";
                useNat = true;
                doneTesting = true;
                break;

            case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
                testMessage = "Limited NAT punchthrough capabilities. Cannot " +
                    "connect to all types of NAT servers. Running a server " +
                    "is ill advised as not everyone can connect.";
                useNat = true;
                doneTesting = true;
                break;

            case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
            case ConnectionTesterStatus.NATpunchthroughFullCone:
                testMessage = "NAT punchthrough capable. Can connect to all " +
                    "servers and receive connections from all clients. Enabling " +
                    "NAT punchthrough functionality.";
                useNat = true;
                doneTesting = true;
                break;

            default:
                testMessage = "Error in test routine, got " + connectionTestResult;
                break;
        }

        if (doneTesting) {
            if (useNat)
                shouldEnableNatMessage = "When starting a server the NAT " +
                    "punchthrough feature should be enabled (useNat parameter)";
            else
                shouldEnableNatMessage = "NAT punchthrough not needed";
            testStatus = "Done testing";
        }

        // Debug.Log("<color=red>" + testMessage + "</color>"); 
    }

}
