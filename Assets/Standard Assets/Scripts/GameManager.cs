using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Networking;
using System; 
#if UNITY_EDITOR
using UnityEditor; 
using UnityEditor.Callbacks;
#endif

public class GameManager : NetworkBehaviour {
	/*
	 * Manager de tous les éléments UI.
	 * 
	 */ 

    public enum AscendanceTerrain {
        MALEFIQUE, 
        ASTRALE, 
        NONE
    };

    public AscendanceTerrain ascendanceTerrain = AscendanceTerrain.NONE; 

	// On update automatiquement le numéro de BUILD après chaque BUILD
	#if UNITY_EDITOR
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, String salut) {
		string currentVersion = Application.version; 
		string[] versionList = currentVersion.Split ('.'); 
		int newSubSubVersion = int.Parse (versionList [3]) + 1;
		PlayerSettings.bundleVersion = versionList [0] + "." + versionList [1] + "." + versionList [2] + "." + newSubSubVersion.ToString (); 
	}
	#endif

	// Bouton
	GameObject NextPhase; 
	// Texte
	GameObject CurrentPhase;

#pragma warning disable CS0649 // Le champ 'GameManager.AKA' n'est jamais assigné et aura toujours sa valeur par défaut null
	GameObject AKA; 
#pragma warning restore CS0649 // Le champ 'GameManager.AKA' n'est jamais assigné et aura toujours sa valeur par défaut null
	GameObject CurrentTour;

	GameObject Capacite_Effet; 

	GameObject NomJoueur1;
	GameObject NomJoueur2;

    GameObject InfoCarteBattle; 

    /// <summary>
    /// AKA sur le tour, c'est à dire à son niveau maximum au début du tour. 
    /// </summary>
	int AKATour = 0;
#pragma warning restore CS0414 // Le champ 'GameManager.AKARemanent' est assigné, mais sa valeur n'est jamais utilisée
    // On ne peut lancer qu'un sort par tour. 
    public int sortLance = 0; 

	public Player.Phases Phase = Player.Phases.INITIATION;
	// On doit aussi synchroniser le tour!
	public int Tour = 1; 

	// Variable globale stockée dans le gameManager, pour savoir si le joueur est en train de choisir des cartes pour exercer ses capacités de cartes. 
	// Dans ce cas, le clique sur une carte aura une signification différente. 
	// On garde aussi en mémoire la carte qui fait le choix
	public bool choixCartes = false; 
	[HideInInspector]
	public GameObject CarteChoix; 
	public List<GameObject> CartesChoisies; 

	/*
	 * Lors de l'attaque. 
	 */ 
	GameObject MyPlayerEntity; 
	GameObject OtherPlayerEntity; 

	bool syncPhase = true; 

	// Sert à visualiser le fait qu'une carte est sélectionnée. 
	public GameObject HitPrefab;

	public int nombreInvocationsSanctuaire = 0; 

	GameObject ShowCards;

    // Cette variable permet de savoir si un sort est en train d'être jouer.
    // Si c'est le cas cette variable sera mise à jour par une carte sort. 
    // Et sera récupérée lors d'un clic sur une carte. 
    [HideInInspector]
    public GameObject SortEnCours = null;

    private GameObject CartesCimetiere;
    private GameObject CarteBaseCimetiere;

    private GameObject EffetParticuleTerrain;
    
    [HideInInspector]
    public bool gameIsPaused = false;
    public bool IPausedTheGame = false; 
    public Sprite Pause;
    public Sprite Play;

    private GameObject DisplayInfo;
    private GameObject PauseButton;

    private GameObject ChoixCartesDebut;
    private GameObject CarteDebutPrefab; 
    /// <summary>
    /// Si l'int est à 0, il n'a pas encore été set
    /// à 1, il est false. 
    /// à 2, il est true. 
    /// </summary>
    int choixDebut = 0;

    GameObject ProposerEffet;
    /// <summary>
    /// Lien dans le GameManager de l'objet qui a demandé à execute un effet. (clic droit sur la carte). 
    /// </summary>
    GameObject ObjetDemandeEffet; 

	// Use this for initialization
	void Start () {
		NomJoueur1 = GameObject.Find ("NomJoueur1"); 
		NomJoueur2 = GameObject.Find ("NomJoueur2"); 
		//SetNames (); 
		NextPhase = GameObject.Find ("ButtonPhase");
		CurrentPhase = GameObject.Find ("Phase"); 
		CurrentTour = GameObject.Find ("Tour"); 
		Capacite_Effet = GameObject.Find ("Capacite_Effet"); 
		setNamePhaseUI (Phase); 
		setTour (Tour); 
		ShowCards = GameObject.Find ("ShowCards");
        CartesCimetiere = transform.Find("CartesCimetiere").gameObject;
        CarteBaseCimetiere = CartesCimetiere.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        CarteBaseCimetiere.SetActive(false); 
        CartesCimetiere.SetActive(false);
        EffetParticuleTerrain = GameObject.Find("EffetParticuleTerrain");
        EffetParticuleTerrain.SetActive(false);
        InfoCarteBattle = GameObject.Find("InfoCarte");
        InfoCarteBattle.SetActive(false);
        DisplayInfo = GameObject.Find("DisplayInfo");
        PauseButton = GameObject.Find("PauseButton");
        DisplayInfo.SetActive(false);
        PauseButton.SetActive(false);
        ChoixCartesDebut = GameObject.Find("ChoixCartesDebut");
        CarteDebutPrefab = ChoixCartesDebut.transform.Find("ChoixCartesDebutFond").Find("CarteDebut").gameObject;
        ChoixCartesDebut.SetActive(false);
        ProposerEffet = GameObject.Find("ProposerEffet");
        ProposerEffet.SetActive(false); 
        StartCoroutine(MontrerCartesAChoisirDebut()); 
        // StartCoroutine(PiocheDebut (6)); 
	}
	
	// Update is called once per frame
	void Update () {
		if (syncPhase) {
			//StartCoroutine (CoroutineDebugPhase ()); 
		}
	}

	IEnumerator CoroutineDebugPhase(){
		syncPhase = false; 
		yield return new WaitForSeconds (0.5f); 
		//setNamePhaseUI (Phase); 
		print (Phase);
		syncPhase = true; 
	}


	public void GoToNextPhase(){
		/*
		 * Lors d'un appui sur le bouton continuer, on passe à la phase suivante. 
		 */ 

		GameObject PlayerObject = FindLocalPlayer ();
		if (FindLocalPlayerID() == Tour) {
			if (Phase == Player.Phases.FINALE) {
				// Il faut passer la main au joueur suivant!
				Phase = Player.Phases.INITIATION; 
				// On change le tour. 
				Tour = (Tour == 1) ? 2 : 1; 
				PlayerObject.SendMessage ("ChangeUITour", Tour); 
			} else {
				Phase++; 
			}
 
			PlayerObject.SendMessage ("ChangeUIPhase", Phase); 
			StartNewPhase (); 
		}
	}

	void StartNewPhase(){
		switch (Phase){
		case Player.Phases.INITIATION:
			
			break;
		case Player.Phases.PIOCHE:
			AllCardsNoAttack (); 
			Pioche (); 
			break;
		case Player.Phases.PREPARATION:

			break; 
		case Player.Phases.PRINCIPALE1:

			break; 
		case Player.Phases.PRINCIPALE2:

			break; 
		case Player.Phases.COMBAT:

			break; 
		case Player.Phases.FINALE:

			break; 
		}

	}

	void AllCardsNoAttack(){
		/*
		 * Comme on début un nouveau tour, toutes les cartes peuvent à nouveau attaquer. 
		 */ 
		GameObject[] AllCards = GameObject.FindGameObjectsWithTag ("BoardSanctuaire"); 
		for (int i = 0; i < AllCards.Length; ++i) {
			AllCards [i].SendMessage ("resetHasAttacked"); 
		}
	}

//	void CalculAKA(){
//		/*
//		 * En phase d'initiation, calcul de l'AKA rémanent de chaque joueur. 
//		 * Le terme de rémanent désigne l'AKA fixé pour le tour et par le joueur, différent de l'AKA courant qui est
//		 * la valeur fonction des actions du joueur qui peut en épuiser.
//		 */ 
//		AKARemanent = GameObject.Find ("CartesChampBatailleJoueur1").transform.childCount
//		+ GameObject.Find ("CartesSanctuaireJoueur1").transform.childCount; 
//		updateAKAAffichage (AKARemanent); 
//	}

	void updateAKAAffichage(int newAKA){
		AKA.GetComponent<Text> ().text = "AKA : " + newAKA.ToString (); 
	}

	void Pioche(){
		/*
		 * Pioche d'une carte lors de la phase de pioche. 
		 */ 
		GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player"); 
		for (int i = 0; i < Players.Length; ++i) {
			if (Players [i].GetComponent<Player> ().PlayerID == Tour) {
				Players [i].SendMessage ("PiocherNouvelleCarte");
				return; 
			}
		}
		//GameObject.Find ("CartesMainJoueur1").SendMessage ("TirerCarte");
	}


	void setNamePhaseUI(Player.Phases newPhase){
		/*
		 * La phase courante est écrite en haut à gauche de l'écran.
		 */ 
		Phase = newPhase; 
		string PhaseToString = " ";

        SendNextPhaseAllCards(newPhase); 

		switch (Phase) {
		    case Player.Phases.INITIATION:
			    PhaseToString = "Initiation"; 
			    ChangeAKAJoueur ();
                // On reset le nombre de sorts lancés à chaque début de tour.    
                sortLance = 0; 
			    //CalculAKA (); 
			    nombreInvocationsSanctuaire = 0; 
			    // Après le calcul on passe directement à la phase suivante. 
			    Debug.Log("PHASE d'INITIATION");
                //GoToNextPhase(); 
                updateAllSorts(); 
			    break;
		    case Player.Phases.PIOCHE:
			    PhaseToString ="Pioche";
			    break;
		    case Player.Phases.PREPARATION:
			    PhaseToString ="Preparation";
                break; 
		    case Player.Phases.PRINCIPALE1:
			    PhaseToString ="Principale1";
                break; 
		    case Player.Phases.PRINCIPALE2:
			    PhaseToString ="Principale2";
                break; 
		    case Player.Phases.COMBAT:
			    PhaseToString ="Combat";
                break; 
		    case Player.Phases.FINALE:
			    PhaseToString ="Finale";
                break; 
		}
		CurrentPhase.GetComponent<Text> ().text = PhaseToString;
        // On ne peut pas réagir à la FIN de la phase d'initiation <=> au début de la phase de pioche. 
        if ((Tour != FindLocalPlayerID()) && (Phase != Player.Phases.PIOCHE)) {
            StartCoroutine(ProposeToPauseGame()); 
        }
	}

    void SendNextPhaseAllCards(Player.Phases currentPhase) {
        /*
         * Envoyer à toutes les cartes qu'on change de phase. 
         */
        GameObject[] AllEntites = GameObject.FindGameObjectsWithTag("BoardSanctuaire"); 
        for (int i = 0; i < AllEntites.Length; ++i) {
            try {
                if (AllEntites[i].GetComponent<Entite>().isFromLocalPlayer) {
                    AllEntites[i].GetComponent<Entite>().UpdateNewPhase(currentPhase, Tour);
                }
            } catch (NullReferenceException e) {
                Debug.Log(e); 
            }
        }

        GameObject[] AllAssistances = GameObject.FindGameObjectsWithTag("Assistance");
        for (int i = 0; i < AllAssistances.Length; ++i) {
            try {
                if (AllAssistances[i].GetComponent<Assistance>().isFromLocalPlayer &&
                    AllAssistances[i].GetComponent<Assistance>().assistanceState == Assistance.State.JOUEE) {
                    AllAssistances[i].SendMessage("UpdateNewPhase", currentPhase);
                }
            } catch (NullReferenceException e) {
                Debug.Log(e); 
            }
        }
    }

	void setTour(int newTour){
		/*
		 * Changer le Tour, fonction appelée par le Player. 
		 */ 
		Tour = newTour; 
		CurrentTour.GetComponent<Text> ().text = Tour.ToString();

	}

	GameObject FindLocalPlayer(){
		/*
		 * Trouver le joueur local, pour lui faire envoyer les fonctions [Command]
		 */ 
		GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player"); 
		if (Players [0].GetComponent<Player> ().isLocalPlayer) {
			return Players [0]; 
		} else {
			return Players [1]; 
		}
	}

	GameObject FindNotLocalPlayer(){
		/*
		 * Trouver le joueur qui n'est pas local, pour lui faire envoyer les fonctions [Command]
		 */ 
		GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player"); 
		if (!Players [0].GetComponent<Player> ().isLocalPlayer) {
			return Players [0]; 
		} else {
			return Players [1]; 
		}
	}

	int FindLocalPlayerID(){
		/*
		 * Mettre le résultat de cette fonction dans une variable?? 
		 * 
		 * Compliqué car au Start() du GameManager, les 2 Players ne sont pas arrivés dans le scene. 
		 */ 
		GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player"); 
		for (int i = 0; i < Players.Length; i++) {
			if (Players [i].GetComponent<Player> ().isLocalPlayer) {
				return Players [i].GetComponent<Player> ().PlayerID; 
			}
		}
		return 0; 
	}


	void AttackMyPlayer(GameObject MyPlayer){
		/*
		 * Choix de la première carte d'attaque, celle du joueur
		 */ 
		MyPlayerEntity = MyPlayer; 
		// On s'assure qu'il n'y ait pas d'autre fleches rouges
		Destroy(GameObject.FindGameObjectWithTag ("Marque"));
		Instantiate (HitPrefab, MyPlayer.transform.position, MyPlayer.transform.rotation); 
	}

	void AttackOtherPlayer(GameObject OtherPlayer){
		/*
		 * Choix de la carte à attaquer, celle de l'adversaire. 
		 */ 
		OtherPlayerEntity = OtherPlayer; 

		Attack (); 
	}

	void Attack(){
		/*
		 * On compare les forces des entités dans cette fonction. 
		 * 
		 * Et on inflige les points de dégâts aux deux entités. 
		 */ 

		Entite.Ascendance AscendanceMyEntity = MyPlayerEntity.GetComponent<Entite> ().carteAscendance; 
		Entite.Element ElementMy = MyPlayerEntity.GetComponent<Entite>().carteElement; 
		int multiplicateurDegatsMy = 1;

		Debug.Log (OtherPlayerEntity.tag); 

		if (OtherPlayerEntity.tag == "Player") {
            /*
			 * Si l'entité attaquée est directement le joueur adverse. 
			 * L'objet envoyé est le joueur adverse. 
             * Il faut vérifier que le joueur n'ait pas de carte sur son champ de bataille. 
			 */
            if ((FindLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").childCount == 0) 
                ||( MyPlayerEntity.GetComponent<Entite>().attaqueDirecte)){
                OtherPlayerEntity.SendMessage("AttackPlayer", MyPlayerEntity.GetComponent<Entite>().STAT);
            } else {
                DisplayMessage("Cette carte ne peut pas attaquer le joueur directement");
                return; 
            }
		} else if (OtherPlayerEntity.tag == "Assistance") {
            /*
             * Si l'entité attaquée est une assistance. 
             */
            OtherPlayerEntity.SendMessage("DetruireCarte");

        } else {
			/*
			 * Si l'entité attaquée est une carte du joueur adverse.  
			 */ 
			Entite.Ascendance AscendanceOtherEntity = OtherPlayerEntity.GetComponent<Entite> ().carteAscendance;
			Entite.Element ElementOther = OtherPlayerEntity.GetComponent<Entite> ().carteElement; 
			int multiplicateurDegatsOther = 1; 

			// On regarde d'abord les ascendances des deux entités. 
			if (AscendanceMyEntity == Entite.Ascendance.NEUTRE || AscendanceOtherEntity == Entite.Ascendance.NEUTRE) {
				// Si un des elements est neutre, l'autre n'est ni fort ni faible face à lui. 
			} else if (AscendanceMyEntity == Entite.Ascendance.ELEMENTAIRE && AscendanceOtherEntity != Entite.Ascendance.ELEMENTAIRE) {
				multiplicateurDegatsMy = 2;
			} else if (AscendanceMyEntity != Entite.Ascendance.ELEMENTAIRE && AscendanceOtherEntity == Entite.Ascendance.ELEMENTAIRE) {
				multiplicateurDegatsOther = 2; 
			} else if (AscendanceMyEntity == Entite.Ascendance.ELEMENTAIRE && AscendanceOtherEntity == Entite.Ascendance.ELEMENTAIRE) {
				// SI les deux sont élémentaires
			
				// vérifier ici les condition aux limites avec les derniers éléments à chaque fois. 
				if (ElementMy - ElementOther == 1) {
					multiplicateurDegatsOther = 2;
				} else if (ElementOther - ElementMy == 1) {
					multiplicateurDegatsMy = 2; 
				}
			}

			Debug.Log ("Il vient d'y avoir une attaque"); 
			int attackMy = MyPlayerEntity.GetComponent<Entite> ().STAT * multiplicateurDegatsMy; 
			int attackOther = OtherPlayerEntity.GetComponent<Entite> ().STAT * multiplicateurDegatsOther; 
			Debug.Log ("Attaque faite au personnage 1 : " + attackOther.ToString ()); 
			Debug.Log ("Attaque faite au personnage 2 : " + attackMy.ToString ()); 

			// On Détruit la carte la plus faible, ou les deux dans un cas d'égalité
			if (multiplicateurDegatsMy * MyPlayerEntity.GetComponent<Entite> ().STAT ==
			   multiplicateurDegatsOther * OtherPlayerEntity.GetComponent<Entite> ().STAT) {
				// Cas d'égalité
				MyPlayerEntity.SendMessage ("DetruireCarte"); 
				OtherPlayerEntity.SendMessage ("DetruireCarte"); 
			} else if (multiplicateurDegatsMy * MyPlayerEntity.GetComponent<Entite> ().STAT >
			          multiplicateurDegatsOther * OtherPlayerEntity.GetComponent<Entite> ().STAT) {
				// La carte du joueur local est plus forte. 
				OtherPlayerEntity.SendMessage ("DetruireCarte"); 
			} else {
				// La carte de l'autre joueur est plus forte. 
				MyPlayerEntity.SendMessage ("DetruireCarte"); 
			}
		}

		// Notre entité a attaqué. 
		MyPlayerEntity.SendMessage ("setHasAttacked", true); 

		// On redétruit la fleche rouge. 
		Destroy (GameObject.FindGameObjectWithTag ("Marque"));
			
		MyPlayerEntity = null; 
		OtherPlayerEntity = null; 
	}

	void Detruire(){
		/*
		 * Capacité d'une carte: Detruire des cartes.
		 * Etapes:
		 * 1. Ecrire sur l'écran détruire les cartes. Noircir l'écran. 
		 * 2. Permettre au joueur de détruire les cartes. 
		 * 
         * INITULISEE
		 */ 

		Capacite_Effet.GetComponent<Text> ().text = "Detruire les cartes souhaitées"; 
	}

	public void DebutChoixCartes(int nombreCartes){
		/*
		 * Choix de cartes par un des joueurs, ayant pour objectif d'exercer une capacite sur certaines cartes
		 * Comme DEGATS ou DETRUIRE, par exemple. 
		 */ 

		choixCartes = true; 
		// On pourrait ici lancer une animation comme par exemple, un fond noir et les cartes en surbrillance pour montrer le choix. 

		// On remet la liste des cartes choisies à zero.
		CartesChoisies = new List<GameObject>(); 
	}

	public void FinChoixCartes(){
		/*
		 * Fin de choix des cartes du joueur. 
		 */  
		choixCartes = false; 
		// ici on finira l'animation

	}

	public IEnumerator WaitForChoixCartes(int nombreCartes){
		/*
		 * On attend que l'utilisateur ait choisi toutes ses cartes. 
		 */ 
		while (CartesChoisies.Count < nombreCartes) {
			yield return new WaitForSeconds (0.5f); 
		}
	}

	public void CarteChoisie(GameObject Carte){
		// La carte envoie un message au GameManager pour dire qu'elle a été choisie. 
		if (!choixCartes) {
			throw new Exception ("Le choix des cartes était désactivé. Cette méthode ne devrait pas être appelée"); 
		}
		CartesChoisies.Add (Carte); 
	}

	public IEnumerator PiocheDebut(int nombreCartes){
		/*
		 * Pioche au début de la partie. 
		 */ 
		yield return new WaitForSeconds (0.35f);
        yield return FindLocalPlayer().GetComponent<Player>().CreateDeck(); 
		for (int i = 0; i < nombreCartes; ++i) {
            Debug.Log("JE PIOCHE AU DEBUT");
            Debug.Log("LA CARTE " + i.ToString()); 
            yield return FindLocalPlayer().GetComponent<Player>().PiocherCarteRoutine();
            yield return new WaitForSeconds(0.5f);
		}
	}

    public IEnumerator PiocheMultiple(int nombreCartes) {
        /*
         * Pioche de plusieurs Cartes. 
         */ 
        yield return new WaitForSeconds(0.35f);
        for (int i = 0; i < nombreCartes; ++i) {
            Debug.Log("JE PIOCHE AU DEBUT");
            Debug.Log("LA CARTE " + i.ToString());
            yield return FindLocalPlayer().GetComponent<Player>().PiocherCarteRoutine();
            yield return new WaitForSeconds(0.5f);
        }
    }

	public void setPlayerPVUI(int IDJoueur, int PV){
		/*
		 * Mettre à joueur les points de vie du joueur sur l'écran. 
		 * 
		 * Si le joueur a l'ID 1 c'est qu'il est le serveur. 
		 * Sinon c'est le client. 
		 */ 
		if (FindLocalPlayer ().GetComponent<Player> ().isServer) {
			// Si le joueur local est le server. 
			if (IDJoueur == 1) {
				NomJoueur1.transform.Find ("PVJoueur1").gameObject.GetComponent<Text> ().text = "PV:" + PV.ToString (); 
			} else {
				NomJoueur2.transform.Find ("PVJoueur2").gameObject.GetComponent<Text> ().text = "PV:" + PV.ToString ();
			}
		} else {
			if (IDJoueur == 2) {
				NomJoueur1.transform.Find ("PVJoueur1").gameObject.GetComponent<Text> ().text = "PV:" + PV.ToString (); 
			} else {
				NomJoueur2.transform.Find ("PVJoueur2").gameObject.GetComponent<Text> ().text = "PV:" + PV.ToString ();
			}
		}
	}

	public void setPlayerAKAUI(int IDJoueur, int AKA){
		/*
		 * Mettre à joueur l'AKA du joueur sur l'écran. 
		 * 
		 * Si le joueur a l'ID 1 c'est qu'il est le serveur. 
		 * Sinon c'est le client. 
		 */ 
		if (FindLocalPlayer ().GetComponent<Player> ().isServer) {
			// Si le joueur local est le server. 
			if (IDJoueur == 1) {
				NomJoueur1.transform.Find ("AKAJoueur1").gameObject.GetComponent<Text> ().text = "AKA:" + AKA.ToString (); 
			} else {
				NomJoueur2.transform.Find ("AKAJoueur2").gameObject.GetComponent<Text> ().text = "AKA:" + AKA.ToString ();
			}
		} else {
			if (IDJoueur == 2) {
				NomJoueur1.transform.Find ("AKAJoueur1").gameObject.GetComponent<Text> ().text = "AKA:" + AKA.ToString (); 
			} else {
				NomJoueur2.transform.Find ("AKAJoueur2").gameObject.GetComponent<Text> ().text = "AKA:" + AKA.ToString ();
			}
		}
	}

	void ChangeAKAJoueur(){
		/*
		 * 
		 * Au début de chaque tour
		 * 
		 * l'ID du joueur est 1 si le joueur est le serveur
		 * L'ID du joueur est 2 si le joueur n'est pas le serveur.
		 * 
		 * On met l'AKA des deux joueurs à jour. 
		 * 
		 */ 
		int currentAKA = GameObject.FindGameObjectsWithTag ("BoardSanctuaire").Length;
        Debug.Log(currentAKA); 
        AKATour = currentAKA; 
		NomJoueur1.transform.Find ("AKAJoueur1").gameObject.GetComponent<Text> ().text = "AKA : " + currentAKA.ToString(); 
		NomJoueur2.transform.Find ("AKAJoueur2").gameObject.GetComponent<Text> ().text = "AKA : " + currentAKA.ToString(); 
		FindLocalPlayer ().SendMessage ("setPlayerAKADebutTour", currentAKA); 
	}

	public void DisplayMessage(string message){
		/*
		 * Message à afficher. 
		 */
		StartCoroutine (DisplayMessageRoutine (message)); 
	}

	IEnumerator DisplayMessageRoutine(string message){
		/*
		 * Coroutine associée à la fonction DisplayMessage
		 */ 
		Capacite_Effet.GetComponent<Text> ().text = message; 
		yield return new WaitForSeconds (5f); 
		Capacite_Effet.GetComponent<Text> ().text = ""; 

	}

	void InvocationElementaireAir(int cout){
		/*
		 * Les cartes à montrer sont choisies au hasard. 
		 */ 

		// on trouve maintenant les cartes de la main du joueur. 
		GameObject[] AllCardsHandAndDeck = GameObject.FindGameObjectsWithTag("Carte"); 
		// AllCards Given = toutes les cartes de la main du joueur
		List<GameObject> AllCardsGiven = new List<GameObject> (); 
		for (int i = 0; i < AllCardsHandAndDeck.Length; ++i) {
			if (AllCardsHandAndDeck [i].GetComponent<Entite> ().hasAuthority) {
				// Normalement les cafrtes avec authority sont les seules cartes instanciées par le joueur sur le serveur, avec le Tag Carte donc
				// les seules cartes de la main. 
				AllCardsGiven.Add(AllCardsHandAndDeck[i]); 
			}
		}
		string[] AllCardsRandom = new string[cout]; 
		int random; 
		for (int i = 0; i < cout; ++i) {
			random = UnityEngine.Random.Range (0, AllCardsGiven.Count); 
			AllCardsRandom [i] = AllCardsGiven [random].GetComponent<Entite> ().shortCode; 
			AllCardsGiven.RemoveAt (random); 
		}
		Debug.Log (AllCardsGiven.Count); 
		// ShowCards.SendMessage ("ShowCardsToChoose", AllCardsGiven); 
		FindLocalPlayer().GetComponent<Player>().CmdSendCards (AllCardsRandom);
	}

    void updateAllSorts() {
        /*
         * A chaque tour on update les sorts qui sont en cours. 
         */ 
        GameObject[] SortsJoues = GameObject.FindGameObjectsWithTag("SortJoue");
        for (int i = 0; i < SortsJoues.Length; ++i) {
            SortsJoues[i].SendMessage("updateSortNewTurn"); 
        }
    }

    void ShowCemetery(List<GameObject> allCartesCimetiere) {
        /*
         * Montrer les cartes dans le cimetiere. 
         */

        CartesCimetiere.SetActive(true);
        // CartesCimetiere->ViewPort->Content->CarteUIToShow. 
        GameObject Content = CartesCimetiere.transform.GetChild(0).GetChild(0).gameObject; 

        for (int i = 0; i < Content.transform.childCount; ++i) {
            if (Content.transform.GetChild(i).gameObject.activeInHierarchy) {
                // on détruit tous les objets là auparavant. 
                Destroy(Content.transform.GetChild(i).gameObject); 
            }
        }

        CarteBaseCimetiere.SetActive(true); 
        Debug.Log(CarteBaseCimetiere); 

        for (int i = 0; i < allCartesCimetiere.Count; ++i) {
            // On instancie une nouvelle carte pour chaque carte du cimetière.
            GameObject NouvelleCarte = Instantiate(CarteBaseCimetiere);
            // NouvelleCarte.AddComponent<Carte>(); 
            NouvelleCarte.SendMessage("setImage", allCartesCimetiere[i].GetComponent<Entite>().shortCode);
            Debug.Log("1 carte instanciée dans le cimetière");
            NouvelleCarte.transform.SetParent(Content.transform, false); 
        }

        CarteBaseCimetiere.SetActive(false);
    }

    void HideCemetery() {
        /*
         * Cacher le cimetiere
         */
        CartesCimetiere.SetActive(false); 
    }

    void EffetTerrain(Entite.Ascendance _ascendance) {
        /*
         * On montre l'effet de terrain par un effet de particule,
         * Noires dans le cas d'un effet maléfique,
         * Blanches dans le cas d'un effet astral, 
         * Pas d'effet si le terrain n'a pas d'effet.
         * 
         */

        GameManager.AscendanceTerrain previousAscendance = ascendanceTerrain; 

        if (_ascendance == Entite.Ascendance.MALEFIQUE) {
            ascendanceTerrain = AscendanceTerrain.MALEFIQUE; 
            EffetParticuleTerrain.SetActive(true);
#pragma warning disable CS0618 // 'ParticleSystem.startColor' est obsolète : 'startColor property is deprecated. Use main.startColor instead.'
            EffetParticuleTerrain.GetComponent<ParticleSystem>().startColor = Color.black; 
#pragma warning restore CS0618 // 'ParticleSystem.startColor' est obsolète : 'startColor property is deprecated. Use main.startColor instead.'
        } else if (_ascendance == Entite.Ascendance.ASTRALE) {
            ascendanceTerrain = AscendanceTerrain.ASTRALE;
            EffetParticuleTerrain.SetActive(true);
#pragma warning disable CS0618 // 'ParticleSystem.startColor' est obsolète : 'startColor property is deprecated. Use main.startColor instead.'
            EffetParticuleTerrain.GetComponent<ParticleSystem>().startColor = Color.white;
#pragma warning restore CS0618 // 'ParticleSystem.startColor' est obsolète : 'startColor property is deprecated. Use main.startColor instead.'
        } else {
            ascendanceTerrain = AscendanceTerrain.NONE;
            EffetParticuleTerrain.SetActive(false); 
        }

        GameObject[] AllCartes = GameObject.FindGameObjectsWithTag("BoardSanctuaire"); 
        for (int i = 0; i < AllCartes.Length; ++i) {
            AllCartes[i].GetComponent<Entite>().updateChangementAscendaceTerrain(ascendanceTerrain, previousAscendance); 
        }

    }

    public AscendanceTerrain GetAscendanceTerrain() {
        return ascendanceTerrain; 
    }

    public void ShowCarteInfo(string shortCode, string Info) {
        /*
         * Afficher les informations liées à la carte. 
         */ 
        InfoCarteBattle.SetActive(true);
        InfoCarteBattle.GetComponent<InfoCarteBattle>().SetInfoCarte(shortCode, Info);
    }

    public void HideInfoCarte() {
        InfoCarteBattle.SetActive(false); 
    }


    /// <summary>
    /// Permet au joueur de mettre le jeu en pause afin de pouvoir réagir à une action de l'autre joueur. 
    /// </summary>
    public void PauseGame() {
        if (gameIsPaused) {
            gameIsPaused = false;
            // PauseButton.GetComponent<Image>().sprite = Play; 
        } else {
            gameIsPaused = true;
            // PauseButton.GetComponent<Image>().sprite = Pause;
        }
        FindLocalPlayer().GetComponent<Player>().CmdSetGameToPause(gameIsPaused);
        Debug.Log("GameIsSetToPause"); 

    }

    public void GameIsSetToPause(bool gamePaused) {
        // Update le sprite du bouton pause. 
        Debug.Log("Valeur du jeu : " + gameIsPaused.ToString());
        Debug.Log("Valeur reçue par le serveur : " + gamePaused.ToString()); 
        if (gameIsPaused == gamePaused) {
            // Dans ce cas c'est ce joueur qui a demandé à ce que le jeu soit mis en pause. 
            // On le laisse donc faire son action.
            IPausedTheGame = gameIsPaused;
            if (gameIsPaused) {

            } else {
                PauseButton.SetActive(false); 
            }
        } else {
            gameIsPaused = gamePaused; 
            DisplayInfo.SetActive(true); 
            if (gameIsPaused) {
                DisplayInfo.GetComponent<Text>().text = "Le jeu a été stoppé par votre adversaire."; 
            } else {
                DisplayInfo.GetComponent<Text>().text = "La partie a repris.";
                StartCoroutine(DeactivateDisplayInfo());
                PauseButton.SetActive(false);
            }
        }

        if (gamePaused) {
            PauseButton.GetComponent<Image>().sprite = Pause;
        } else {
            PauseButton.GetComponent<Image>().sprite = Play;
        }
        Debug.Log("game Is Actually paused.");
    }

    private IEnumerator DeactivateDisplayInfo() {
        yield return new WaitForSeconds(2f);
        DisplayInfo.SetActive(false); 
    }

    public IEnumerator ProposeToPauseGame(int playerID=0) {
        /*
         * Après une action, on propose au joueur adverse de mettre le jeu en pause. 
         * Si le playerID reste à 0, c'est que le choix du joueur qui peut mettre le jeu
         * en pause est géré par un autre élément. Exemple: Lors d'un changement de phase, 
         * c'est déjà géré dans la fonction du gameManager. 
         * 
         * Sinon, l'ID passée en paramètre est celle proposée par le joueur qui appelle la fonction. 
         * La proposition du bouton pause est donc uniquement pour celui qui n'a pas fait l'action
         */
        if ((FindLocalPlayerID() != playerID) || (playerID == 0)) {
            PauseButton.SetActive(true);
            yield return new WaitForSeconds(2f);
            if (!gameIsPaused) {
                PauseButton.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Permettre au joueur de choisir s'il veut garder sa première pioche, sa deuxième, sa troisième...
    /// </summary>
    /// <returns></returns>
    public IEnumerator MontrerCartesAChoisirDebut(int nombreCartes=6) {
        yield return new WaitForSeconds(0.5f);
        if (nombreCartes == 6) {
            yield return FindLocalPlayer().GetComponent<Player>().CreateDeck();
        } else {
            FindLocalPlayer().GetComponent<Player>().MelangerDeck(); 
        }

        ChoixCartesDebut.SetActive(true);
        CarteDebutPrefab.SetActive(true); 
        // On récupère les cartes du dessus du deck. 
        List<GameObject> _cartesDebut = FindLocalPlayer().GetComponent<Player>().RecupererCartesDessusDeck(6);

        Transform ParentCartesDebut = ChoixCartesDebut.transform.Find("ChoixCartesDebutFond");

        // On détruit toutes les cartes déjà display piochées auparavant. 
        if (ParentCartesDebut.childCount != 1) {
            for (int i = 0; i < ParentCartesDebut.childCount; ++i) {
                if (ParentCartesDebut.GetChild(i).gameObject != CarteDebutPrefab) {
                    Destroy(ParentCartesDebut.GetChild(i).gameObject); 
                }
            }
        }

        for (int i = 0; i < nombreCartes; ++i) {
            GameObject newCarte = Instantiate(CarteDebutPrefab);
            newCarte.transform.SetParent(ParentCartesDebut, false);
            // Si ça ne marche pas, il FAUT différencier, sort, entité, et assistance. 
            switch (_cartesDebut[i].GetComponent<CarteType>().thisCarteType) {
                case CarteType.Type.ASSISTANCE:
                    newCarte.GetComponent<CarteDebut>().InfoDebut(_cartesDebut[i].GetComponent<Carte>().shortCode,
                    _cartesDebut[i].GetComponent<Carte>().GetInfoCarte());
                    break; 
                 case CarteType.Type.ENTITE:
                    newCarte.GetComponent<CarteDebut>().InfoDebut(_cartesDebut[i].GetComponent<Entite>().shortCode,
                    _cartesDebut[i].GetComponent<Carte>().GetInfoCarte());
                    break;
                 case CarteType.Type.SORT:
                    newCarte.GetComponent<CarteDebut>().InfoDebut(_cartesDebut[i].GetComponent<Sort>().shortCode,
                    _cartesDebut[i].GetComponent<Carte>().GetInfoCarte());
                    break;
            }
        }

        CarteDebutPrefab.SetActive(false);

        StartCoroutine(WaitForResponseCartesDebut(nombreCartes)); 
    }

    public IEnumerator WaitForResponseCartesDebut(int nombreCartes) {
        Debug.Log("ChoixDebut" + choixDebut.ToString()); 
        while (choixDebut == 0) {
            yield return new WaitForSeconds(0.5f); 
        }

        if (choixDebut == 1) {
            choixDebut = 0; 
            StartCoroutine(MontrerCartesAChoisirDebut(nombreCartes - 1));
        }
        else {
            choixDebut = 0; 
            for (int i = 0; i < nombreCartes; ++i) {
                Debug.Log("JE PIOCHE AU DEBUT DANS LA COROUTINE");
                Debug.Log("LA CARTE " + i.ToString());
                yield return FindLocalPlayer().GetComponent<Player>().PiocherCarteRoutine();
                yield return new WaitForSeconds(0.5f);
            }

            ChoixCartesDebut.SetActive(false);
        }
    }

    public void ChoixDebutOK() {
        choixDebut = 2; 
    }

    public void ChoixDebutNotOK() {
        choixDebut = 1; 
    }

    public void ProposerEffetJoueur(string effetsToDisplay, GameObject ObjectAsking) {
        /**
         * Proposer au joueur de jouer un effet. 
         */
        ProposerEffet.SetActive(true);
        ProposerEffet.transform.Find("EffetProposeText").GetComponent<Text>().text = effetsToDisplay;
        ObjetDemandeEffet = ObjectAsking; 
    }

    public void ReponseEffetPropose(int reponse) {
        ObjetDemandeEffet.SendMessage("ReponseEffet", reponse);
        ProposerEffet.SetActive(false); 
    }

}