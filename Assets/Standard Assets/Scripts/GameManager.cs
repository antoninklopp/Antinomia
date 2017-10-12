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


/// <summary>
/// Classe qui manage tous les éléments de UI 
/// ainsi que beaucoup de choix de cartes et les attaques. 
/// </summary>
public class GameManager : NetworkBehaviourAntinomia {

    /// <summary>
    /// Ascendance du terrain. Enum. 
    /// </summary>
    public enum AscendanceTerrain {
        MALEFIQUE, 
        ASTRALE, 
        NONE
    };

    /// <summary>
    /// Ascendance courante du terrain.
    /// </summary>
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
     /// <summary>
     /// Carte qui attaque
     /// </summary>
	GameObject MyPlayerEntity; 
    /// <summary>
    /// Carte qui est attaquée
    /// </summary>
	GameObject OtherPlayerEntity; 

	bool syncPhase = true;

    /// <summary>
    /// Sert à visualiser le fait qu'une carte est sélectionnée. 
    /// </summary>
    public GameObject HitPrefab;

    /// <summary>
    /// Nombre d'invocations dans le sanctuaire pour le tour en cours. Doit toujours être inférieur ou = 1; 
    /// </summary>
	public int nombreInvocationsSanctuaire = 0; 

	GameObject ShowCards;

    /// <summary>
    ///  Cette variable permet de savoir si un sort est en train d'être jouer.
    /// Si c'est le cas cette variable sera mise à jour par une carte sort. 
    /// Et sera récupérée lors d'un clic sur une carte. 
    /// </summary>
    [HideInInspector]
    public GameObject SortEnCours = null;

    /// <summary>
    /// Représentation du cimetière qui peut être ouverte lors d'un clic joueur, s'il
    /// veut voir les cartes de son cimetière ou du cimetière de l'autre joueur. 
    /// </summary>
    private GameObject CartesCimetiere;
    /// <summary>
    /// Le prefab de la carte du cimetière.
    /// </summary>
    private GameObject CarteBaseCimetiere;

    /// <summary>
    /// Effet de particule sur le terrain, qui matérialise l'ascendance du terrain. 
    /// </summary>
    private GameObject EffetParticuleTerrain;
    
    /// <summary>
    /// true si le jeu est en pause, false sinon
    /// </summary>
    [HideInInspector]
    public bool gameIsPaused = false;
    /// <summary>
    /// true si le joueur local a mis le jeu en pause, false sinon
    /// </summary>
    public bool IPausedTheGame = false; 
    /// <summary>
    /// Le sprite représentant le bouton pause
    /// </summary>
    public Sprite Pause;
    /// <summary>
    /// Le sprite représentant le bouton Play
    /// </summary>
    public Sprite Play;

    /// <summary>
    /// L'objet text qui permet de montrer des effets au milieu de l'écran
    /// </summary>
    private GameObject DisplayInfo;
    /// <summary>
    /// Le bouton pause. 
    /// </summary>
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

    /// <summary>
    /// La console de développement. 
    /// </summary>
    private GameObject Console; 


	// Use this for initialization
	public override void Start () {
        base.Start(); 

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
        Console = GameObject.Find("Console");
        StartCoroutine(MontrerCartesAChoisirDebut()); 
        // StartCoroutine(PiocheDebut (6)); 
	}
	
	// Update is called once per frame
	//void Update () {
	//	if (syncPhase) {
	//		//StartCoroutine (CoroutineDebugPhase ()); 
	//	}
	//}

	IEnumerator CoroutineDebugPhase(){
		syncPhase = false; 
		yield return new WaitForSeconds (0.5f); 
		//setNamePhaseUI (Phase); 
		print (Phase);
		syncPhase = true; 
	}

    /// <summary>
    /// Passage à la nouvelle phase lors du clic sur le bouton de passage à la nouvelle phase. 
    /// </summary>
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

    /// <summary>
    /// Methode appelée lorsqu'on passe à une nouvelle phase. 
    /// </summary>
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

    /// <summary>
    /// Lorsqu'une carte attaque, elle ne peut plus attaquer pour le tour. 
    /// A la fin de chaque tour, on remet "les compteurs à 0". 
    /// </summary>
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

    /// <summary>
    /// Pioche d'une carte
    /// </summary>
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

    /// <summary>
    /// Appelé lors du changement de phase. 
    /// Permet de mettre à jour l'information en haut à gauche de l'écran ainsi que
    /// de mettre à jour plusieurs choses slon les phases, comme l'AKA par exemple. 
    /// </summary>
    /// <param name="newPhase"></param>
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
            // StartCoroutine(ProposeToPauseGame()); 
        }
	}

    /// <summary>
    /// Envoyer à toutes les cartes, le fait que l'on passe à une autre phase, 
    /// afin qu'on puisse détecter s'il certaines cartes peuvent jouer des effets lors de cette nouvelle phase notamment. 
    /// </summary>
    /// <param name="currentPhase">La nouvelle phase de jeu.</param>
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

    /// <summary>
    /// Trouver l'ID du joueur qui n'est pas local. 
    /// </summary>
    /// <returns>Nn entier contenant 1 ou 2. </returns>
	private static int FindLocalPlayerID(){
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

    /// <summary>
    /// Choix de la carte qui attaque. 
    /// </summary>
    /// <param name="MyPlayer">La carte qui attaque</param>
	void AttackMyPlayer(GameObject MyPlayer){
		/*
		 * Choix de la première carte d'attaque, celle du joueur
		 */ 
		MyPlayerEntity = MyPlayer; 
		// On s'assure qu'il n'y ait pas d'autre fleches rouges
		Destroy(GameObject.FindGameObjectWithTag ("Marque"));
		Instantiate (HitPrefab, MyPlayer.transform.position, MyPlayer.transform.rotation); 
	}

    /// <summary>
    /// Choix de la carte A attaquer. 
    /// </summary>
    /// <param name="OtherPlayer">La carte attaquée</param>
	void AttackOtherPlayer(GameObject OtherPlayer){
		/*
		 * Choix de la carte à attaquer, celle de l'adversaire. 
		 */ 
		OtherPlayerEntity = OtherPlayer; 

		Attack (); 
	}

    /// <summary>
    /// Attaque d'ue carte, une assistance ou même directement du joueur par une autre carte.
    /// </summary>
	public void Attack(bool JouerEffet=false, GameObject _MyPlayerEntity=null, GameObject _OtherPlayerEntity=null){
        /*
		 * On compare les forces des entités dans cette fonction. 
		 * 
		 * Et on inflige les points de dégâts aux deux entités. 
		 */

        Debug.Log(_MyPlayerEntity);
        Debug.Log(_OtherPlayerEntity); 
        if (_MyPlayerEntity != null && _OtherPlayerEntity != null) {
            /*
             * Lorsqu'on défait la pile d'effets, on envoie directement les deux objets qui attaquent et qui sont attaqués. 
             * Ils overrident les objets de base. 
             */
            MyPlayerEntity = _MyPlayerEntity;
            OtherPlayerEntity = _OtherPlayerEntity;
            Debug.Log("On a override tout ça."); 
        }

        Debug.Log(MyPlayerEntity); 

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
            if ((FindNotLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").childCount == 0)
                || (MyPlayerEntity.GetComponent<Entite>().attaqueDirecte)) {
                if (!JouerEffet) {
                    AjouterEffetAttaquePile();
                    return; 
                }
                else {
                    Debug.Log("Le joueur adverse a été attaqué"); 
                    OtherPlayerEntity.SendMessage("AttackPlayer", MyPlayerEntity.GetComponent<Entite>().STAT);
                }
            }
            else {
                DisplayMessage("Cette carte ne peut pas attaquer le joueur directement");
                return;
            }
        }
        else if (OtherPlayerEntity.tag == "Assistance") {
            /*
             * Si l'entité attaquée est une assistance. 
             */
            if (JouerEffet) {
                OtherPlayerEntity.SendMessage("DetruireCarte");
            }
            else {
                AjouterEffetAttaquePile();
                return; 
            }
        }
        else {
            /*
			 * Si l'entité attaquée est une carte du joueur adverse.  
			 */

            if (JouerEffet) {
                Entite.Ascendance AscendanceOtherEntity = OtherPlayerEntity.GetComponent<Entite>().carteAscendance;
                Entite.Element ElementOther = OtherPlayerEntity.GetComponent<Entite>().carteElement;
                int multiplicateurDegatsOther = 1;

                // On regarde d'abord les ascendances des deux entités. 
                if (AscendanceMyEntity == Entite.Ascendance.NEUTRE || AscendanceOtherEntity == Entite.Ascendance.NEUTRE) {
                    // Si un des elements est neutre, l'autre n'est ni fort ni faible face à lui. 
                }
                else if (AscendanceMyEntity == Entite.Ascendance.ELEMENTAIRE && AscendanceOtherEntity != Entite.Ascendance.ELEMENTAIRE) {
                    multiplicateurDegatsMy = 2;
                }
                else if (AscendanceMyEntity != Entite.Ascendance.ELEMENTAIRE && AscendanceOtherEntity == Entite.Ascendance.ELEMENTAIRE) {
                    multiplicateurDegatsOther = 2;
                }
                else if (AscendanceMyEntity == Entite.Ascendance.ELEMENTAIRE && AscendanceOtherEntity == Entite.Ascendance.ELEMENTAIRE) {
                    // SI les deux sont élémentaires

                    // vérifier ici les condition aux limites avec les derniers éléments à chaque fois. 
                    if (ElementMy - ElementOther == 1) {
                        multiplicateurDegatsOther = 2;
                    }
                    else if (ElementOther - ElementMy == 1) {
                        multiplicateurDegatsMy = 2;
                    }
                }

                Debug.Log("Il vient d'y avoir une attaque");
                int attackMy = MyPlayerEntity.GetComponent<Entite>().STAT * multiplicateurDegatsMy;
                int attackOther = OtherPlayerEntity.GetComponent<Entite>().STAT * multiplicateurDegatsOther;
                Debug.Log("Attaque faite au personnage 1 : " + attackOther.ToString());
                Debug.Log("Attaque faite au personnage 2 : " + attackMy.ToString());

                // On Détruit la carte la plus faible, ou les deux dans un cas d'égalité
                if (multiplicateurDegatsMy * MyPlayerEntity.GetComponent<Entite>().STAT ==
                   multiplicateurDegatsOther * OtherPlayerEntity.GetComponent<Entite>().STAT) {
                    // Cas d'égalité
                    MyPlayerEntity.SendMessage("DetruireCarte");
                    OtherPlayerEntity.SendMessage("DetruireCarte");
                }
                else if (multiplicateurDegatsMy * MyPlayerEntity.GetComponent<Entite>().STAT >
                        multiplicateurDegatsOther * OtherPlayerEntity.GetComponent<Entite>().STAT) {
                    // La carte du joueur local est plus forte. 
                    OtherPlayerEntity.SendMessage("DetruireCarte");
                }
                else {
                    // La carte de l'autre joueur est plus forte. 
                    MyPlayerEntity.SendMessage("DetruireCarte");
                }
            }
            else {
                AjouterEffetAttaquePile();
                return; 
            }
        }

        // Notre entité a attaqué. 
        MyPlayerEntity.SendMessage("setHasAttacked", true);

        MyPlayerEntity = null;
        OtherPlayerEntity = null;
	}

    private void AjouterEffetAttaquePile() {
        // On prévient qu'on va jouer l'effet. 
        List<GameObject> Cibles = new List<GameObject>();
        // On ajoute le joueur adverse à la liste des cibles. 
        // Il faudra bien faire un cas séparé lors de la recherche de l'ID lorsqu'on défait la pile.
        Cibles.Add(OtherPlayerEntity);
        MettreEffetDansLaPile(MyPlayerEntity, Cibles, -1);

        MyPlayerEntity = null;
        OtherPlayerEntity = null;

        // On redétruit la fleche rouge. 
        Destroy(GameObject.FindGameObjectWithTag("Marque"));

        return;
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

    /// <summary>
    /// Pioche de cartes au début du jeu.
    /// </summary>
    /// <param name="nombreCartes"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Piocher plusieurs cartes. 
    /// </summary>
    /// <param name="nombreCartes">nombre de cartes à piocher</param>
    /// <returns></returns>
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

    /// <summary>
    /// Mettre à joueur les points de vie du joueur sur l'écran. 
    /// 
    /// Si le joueur a l'ID 1 c'est qu'il est le serveur. 
    /// Sinon c'est le client. 
    /// </summary>
    /// <param name="IDJoueur">ID du joueur</param>
    /// <param name="PV">PV du joueur à mettre à jour</param>
    public void setPlayerPVUI(int IDJoueur, int PV){
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
        // Debug.Log(currentAKA); 
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

    /// <summary>
    /// Cacher le cimetiere.
    /// </summary>
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

    /// <summary>
    /// Montrer les infos d'une carte.
    /// </summary>
    /// <param name="shortCode">shortCode de la carte</param>
    /// <param name="Info">l'Info à montrer</param>
    public void ShowCarteInfo(string shortCode, string Info) {
        /*
         * Afficher les informations liées à la carte. 
         */ 
        InfoCarteBattle.SetActive(true);
        InfoCarteBattle.GetComponent<InfoCarteBattle>().SetInfoCarte(shortCode, Info);
    }

    /// <summary>
    /// Cacher le panneau qui montre les infos de la carte.
    /// </summary>
    public void HideInfoCarte() {
        InfoCarteBattle.SetActive(false); 
    }


    /// <summary>
    /// Permet au joueur de mettre le jeu en pause afin de pouvoir réagir à une action de l'autre joueur. 
    /// Appelé directement depuis le bouton de pause sur l'écran. 
    /// </summary>
    public void PauseGame() {
        if (gameIsPaused) {
            gameIsPaused = false;
        } else {
            gameIsPaused = true;
        }
        FindLocalPlayer().GetComponent<Player>().CmdSetGameToPause(gameIsPaused);
        Debug.Log("GameIsSetToPause"); 

    }
    
    /// <summary>
    /// Montrer que le jeu a été mis en pause, ou au contraire le désactiver en fonction du paramètre. 
    /// </summary>
    /// <param name="gamePaused"></param>
    public void GameIsSetToPause(bool gamePaused) {
        // Update le sprite du bouton pause. 
        Debug.Log("Valeur du jeu : " + gameIsPaused.ToString());
        Debug.Log("Valeur reçue par le serveur : " + gamePaused.ToString()); 
        if (gameIsPaused == gamePaused) {
            // Dans ce cas c'est ce joueur qui a demandé à ce que le jeu soit mis en pause. 
            // On le laisse donc faire son action.
            IPausedTheGame = gameIsPaused;
            if (gamePaused) {

            } else {
                Debug.Log("Le jeu reprend. "); 
                // Une fois qu'il a fini ses actions on défait la pile. 
                PauseButton.SetActive(false);
                //On regarde s'il y a une pile d'effet en cours. 
                if (GameObject.FindGameObjectWithTag("Pile") != null) {
                    GameObject Pile = GameObject.FindGameObjectWithTag("Pile");
                    if (Pile.GetComponent<PileAppelEffet>().DernierEffetVientJoueur(FindLocalPlayer().GetComponent<Player>().PlayerID)) {
                        // Si c'est le joueur qui a mis un effet en dernier dans la pile, on la défait. 
                        AntinomiaLog("Après avoir mis le jeu en pause, je défais la pile");
                        Pile.GetComponent<PileAppelEffet>().DefaireLaPile();
                    } 
                    //Sinon on ne fait rien. 
                }

            }
        } else {
            this.gameIsPaused = gamePaused; 
            DisplayInfo.SetActive(true); 
            if (gameIsPaused) {
                DisplayInfo.GetComponent<Text>().text = "Le jeu a été stoppé par votre adversaire."; 
            } else {
                DisplayInfo.GetComponent<Text>().text = "La partie a repris.";
                StartCoroutine(DeactivateDisplayInfo());
                PauseButton.SetActive(false);
            }
        }

        this.gameIsPaused = gamePaused; 

        if (gamePaused) {
            PauseButton.GetComponent<Image>().sprite = Pause;
        } else {
            PauseButton.GetComponent<Image>().sprite = Play;
        }
        Debug.Log("game Is Actually paused.");
    }

    /// <summary>
    /// Desactiver l'objet DisplayInfo qui permet d'afficher des Informations au milieu de l'écran
    /// </summary>
    /// <param name="time">Temps après lequel le texte disparait</param>
    /// <returns></returns>
    private IEnumerator DeactivateDisplayInfo(float time=2f) {
        yield return new WaitForSeconds(time);
        DisplayInfo.SetActive(false);
        AntinomiaLog("On désactive les infos"); 
    }

    /// <summary>
    /// Montrer une information au centre de l'écran. 
    /// Et non sur le côté comme lors dans la méthode DisplayMessage. 
    /// </summary>
    public void DisplayInfoToPlayer(string message) {
        DisplayInfo.SetActive(true);
        DisplayInfo.GetComponent<Text>().text = message;
        StartCoroutine(DeactivateDisplayInfo()); 
    }

    /// <summary>
    /// Après une action, on propose au joueur adverse de mettre le jeu en pause. 
    /// Si le playerID reste à 0, c'est que le choix du joueur qui peut mettre le jeu
    /// en pause est géré par un autre élément.Exemple: Lors d'un changement de phase, 
    /// c'est déjà géré dans la fonction du gameManager. 
    /// 
    /// Sinon, l'ID passée en paramètre est celle proposée par le joueur qui appelle la fonction. 
    /// La proposition du bouton pause est donc uniquement pour celui qui n'a pas fait l'action
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="message">Lorsque le jeu est mis en pause, on peut vouloir afficher un message au joueur</param>
    /// <returns></returns>
    public IEnumerator ProposeToPauseGame(int playerID=0, string message="") {
        if ((FindLocalPlayerID() != playerID) || (playerID == 0)) {
            // Si on a reçu un effet, c'est que l'adversaire a réagi, le jeu n'est plus en pause. 
            FindLocalPlayer().GetComponent<Player>().CmdOnlySetPause(false); 
            PauseButton.SetActive(true);
            if (message != ""){
                // Si un message est fourni lors de l'appel, on l'affiche
                DisplayInfoToPlayer(message);
                //DeactivateDisplayInfo(10f); 
            }
            // Temps d'attente. 
            AntinomiaLog("Attente");
            MontrerQuellesCartesPeuventJoueur(); 
            yield return new WaitForSeconds(5f);
            if (!gameIsPaused) {
                PauseButton.SetActive(false);
                //On regarde s'il y a une pile d'effet en cours. 
                if (GameObject.FindGameObjectWithTag("Pile") != null) {
                    AntinomiaLog("Je n'ai pas mis en pause, je défais la pile");
                    GameObject.FindGameObjectWithTag("Pile").GetComponent<PileAppelEffet>().DefaireLaPile(); 
                }
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

    /// <summary>
    /// Attendre la réponse du joueur après qu'on lui proposait de piocher la main qu'on lui montre ou de repiocher. 
    /// </summary>
    /// <param name="nombreCartes">Le nombre de cartes à piocher</param>
    /// <returns>None</returns>
    public IEnumerator WaitForResponseCartesDebut(int nombreCartes) {
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
                yield return FindLocalPlayer().GetComponent<Player>().PiocherCarteRoutine();
                yield return new WaitForSeconds(0.2f);
            }

            ChoixCartesDebut.SetActive(false);
        }
    }

    /// <summary>
    /// Si le joueur accepte sa main de départ. 
    /// </summary>
    public void ChoixDebutOK() {
        choixDebut = 2; 
    }

    /// <summary>
    /// Si le joueur préfère repiocher. 
    /// </summary>
    public void ChoixDebutNotOK() {
        choixDebut = 1; 
    }

    /// <summary>
    /// Proposer au joueur de jouer un effet. 
    /// </summary>
    /// <param name="effetsToDisplay">Les effets à afficher</param>
    /// <param name="ObjectAsking">L'objet (la carte) qui a "proposé" de faire l'effet.</param>
    public void ProposerEffetJoueur(string effetsToDisplay, GameObject ObjectAsking) {
        /**
         * Proposer au joueur de jouer un effet. 
         */
        ProposerEffet.SetActive(true);
        ProposerEffet.transform.Find("EffetProposeText").GetComponent<Text>().text = effetsToDisplay;
        ObjetDemandeEffet = ObjectAsking; 
    }

    /// <summary>
    /// Reponse du joueur par rapport à l'effet proposé. 
    /// </summary>
    /// <param name="reponse">2, si le joueur accepte, 1 sinon</param>
    public void ReponseEffetPropose(int reponse) {
        ObjetDemandeEffet.SendMessage("ReponseEffet", reponse);
        ProposerEffet.SetActive(false); 
    }


    /// <summary>
    /// Rajouter un effet dans la pile.
    /// Crée la pile si elle n'existe pas. 
    /// </summary>
    /// <param name="ObjetEffet">Objet qui attaque</param>
    /// <param name="Cibles">Les Cibles de l'effet</param>
    /// <param name="numeroEffet">La position de l'effet dans la liste d'effets</param>
    /// <param name="numeroListEffet">Le numero de la liste d'effets</param>
    protected void MettreEffetDansLaPile(GameObject ObjetEffet, List<GameObject> Cibles, int numeroEffet, int numeroListEffet = 0) {
        Debug.Log("Effet dans la pile");
        GameObject Pile = GameObject.FindGameObjectWithTag("Pile");
        if (Pile == null) {
            // C'est le premier effet de la pile, il faut donc instancier la pile. 
            FindLocalPlayer().GetComponent<Player>().CmdCreerPile();
            StartCoroutine(MettreEffetDansLaPileRoutine(ObjetEffet, Cibles, numeroEffet, numeroListEffet));
        }
        else {
            // La pile existe déjà, on peut rajouter à la pile. 
            Pile.GetComponent<PileAppelEffet>().AjouterEffetALaPile(ObjetEffet, Cibles, numeroEffet, numeroListEffet);
            Debug.Log("Un effet a été ajouté à la pile");
        }

    }

    /// <summary>
    /// Rajouter un effet dans la pile.
    /// Ceci est le premier effet ajouté à la pile. 
    /// </summary>
    /// <param name="ObjetEffet">Objet qui attaque</param>
    /// <param name="Cibles">Les Cibles de l'effet</param>
    /// <param name="numeroEffet">La position de l'effet dans la liste d'effets</param>
    /// <param name="numeroListEffet">Le numero de la liste d'effets</param>
    protected IEnumerator MettreEffetDansLaPileRoutine(GameObject ObjetEffet, 
                                        List<GameObject> Cibles, int numeroEffet, int numeroListEffet = 0) {
        Debug.Log("Effet dans la pile");
        GameObject Pile = null;
        for (int i = 0; i < 5 && Pile == null; ++i) {
            yield return new WaitForSeconds(0.1f);
            Pile = GameObject.FindGameObjectWithTag("Pile");
        }

        Debug.Log("Pile créée");
        if (Pile == null) {
            throw new Exception("La pile ne s'est pas créée");
        }

        Pile.GetComponent<PileAppelEffet>().AjouterEffetALaPile(ObjetEffet, Cibles, numeroEffet, numeroListEffet);
        Debug.Log("Un effet a été ajouté à la pile");
    }

    /// <summary>
    /// Ouvrir la console
    /// </summary>
    public void OpenConsole() {
        if (Console.GetComponent<AntinomiaConsole>().state) {
            // Si la console est active on la désactive.
            Console.GetComponent<AntinomiaConsole>().DeactivateConsole(); 
        } else {
            // Sinon on l'active. 
            Console.GetComponent<AntinomiaConsole>().ActivateConsole();
        }
    }

    /// <summary>
    /// Add a log into the console. 
    /// </summary>
    public void Log(string log) {
        Debug.Log(log);
        Console.GetComponent<AntinomiaConsole>().AddStringToConsole(log); 
    }

    /// <summary>
    /// Retourne true si le jeu est en pause, au moins pour ce joueur, 
    /// false sinon
    /// </summary>
    public bool getGameIsPaused() {
        return gameIsPaused;
    }

    /// <summary>
    /// Montrer aux jours quelles cartes il peut jouer 
    /// POUR REPONDRE A UN EFFET.
    /// </summary>
    private void MontrerQuellesCartesPeuventJoueur() {



    }

}