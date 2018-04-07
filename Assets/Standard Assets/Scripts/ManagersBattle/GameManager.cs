
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Networking;
using UnityEngine.SceneManagement; 
using System;
using AntinomiaException; 
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
	protected GameObject NextPhase; 
	// Texte
	protected GameObject CurrentPhase;

#pragma warning disable CS0649 // Le champ 'GameManager.AKA' n'est jamais assigné et aura toujours sa valeur par défaut null
	protected GameObject AKA; 
#pragma warning restore CS0649 // Le champ 'GameManager.AKA' n'est jamais assigné et aura toujours sa valeur par défaut null
	protected GameObject CurrentTour;

	protected GameObject Capacite_Effet; 

	protected GameObject NomJoueur1;
	protected GameObject NomJoueur2;

    protected GameObject InfoCarteBattle;
    protected GameObject InfoCarteBattlePhone; 

    /// <summary>
    /// AKA sur le tour, c'est à dire à son niveau maximum au début du tour. 
    /// </summary>
	private int AKATour = 0;

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
    [HideInInspector]
	public List<GameObject> _CartesChoisies; 

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

    /// <summary>
    /// Objet qui permet de choisir des cartes (pour des effets ou pour les montrer à votre adversaire). 
    /// </summary>
	protected GameObject ChooseCardsObject;

    /// <summary>
    /// Objet qui permet de voir les cartes choisies par l'adversaire par exemple. 
    /// </summary>
    private GameObject ShowCardsObject; 

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
    protected  GameObject CartesCimetiere;

    /// <summary>
    /// Le prefab de la carte du cimetière.
    /// </summary>
    protected GameObject CarteBaseCimetiere;

    /// <summary>
    /// Effet de particule sur le terrain, qui matérialise l'ascendance du terrain. 
    /// </summary>
    protected GameObject EffetParticuleTerrain;
    
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
    protected  GameObject DisplayInfo;

    /// <summary>
    /// Le bouton pause. 
    /// </summary>
    protected GameObject PauseButton;

    protected GameObject ChoixCartesDebut;

    protected GameObject CarteDebutPrefab; 

    /// <summary>
    /// Si l'int est à 0, il n'a pas encore été set
    /// à 1, il est false. 
    /// à 2, il est true. 
    /// </summary>
    int choixDebut = 0;

    protected GameObject ProposerEffet;
    /// <summary>
    /// Lien dans le GameManager de l'objet qui a demandé à execute un effet. (clic droit sur la carte). 
    /// </summary>
    protected GameObject ObjetDemandeEffet;

    /// <summary>
    /// La console de développement. 
    /// </summary>
    protected GameObject Console;

    /// <summary>
    /// Objet qui permet de proposer de défaire la pile, 
    /// il comporte deux boutons fils, oui et non.
    /// </summary>
    protected GameObject ProposerDefairePile;

    /// <summary>
    /// Cette variable permet de gérer la réponse du joueur qui peut défaire la pile. 
    /// 0, pas de réponse
    /// 1, réponse positive, 
    /// 2 réponse négative. 
    /// </summary>
    protected int defairePile = 0;

    /// <summary>
    /// Cet objet permet de matérialiser l'attente qu'il reste au joueur afin de pouvoir faire pause. 
    /// </summary>
    protected GameObject SliderPause;

    private int nombreDeCartesChoisies = 0;

    /// <summary>
    /// Un objet qui prévient le joueur que son adversaire est encore en train 
    /// de choisir ses cartes au début du tour. 
    /// </summary>
    private GameObject ChoixCartesAdversaire;

    /// <summary>
    /// L'objet de la fin du jeu qui indique si le joueur a perdu ou gagné. 
    /// </summary>
    private GameObject EndGame;

    /// <summary>
    /// Grace à cette variable, on va pouvoir bloquer l'interaction du joueur avec le jeu. 
    /// Pour appuyer sur le bouton continuer par exemple. 
    /// </summary>
    public static bool peutInteragir = true;

    /// <summary>
    /// Lors d'un clic sur le bouton pour pouvoir avoir les informations sur la carte, 
    /// on rajoute sur mobile le check automatique des effets. 
    /// </summary>
    private int IDCardGameAttenteJouerEffet = -1;

    /// <summary>
    /// Informations sur un effet de l'adversaire
    /// </summary>
    private GameObject InfoEffetTransmis;

    /// <summary>
    /// L'autre GameManager qui permet de transmettre des informations à l'autre joueur. 
    /// </summary>
    private GameObject GameManagerInformation;

    private GameObject eventManager;

    /// <summary>
    /// AKA remanent, c'est à dire l'AKA maximum au début du tour. 
    /// </summary>
    public static int AKARemanent;

    /// <summary>
    /// Tous les boutons de changement de phase, au milieu de l'écran.
    /// </summary>
    private GameObject AllButtonsPhase; 

    /// <summary>
    /// Initialisation du GameManager
    /// </summary>
    public override void Start () {
        // On récupère tous les objets UI
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
		ChooseCardsObject = GameObject.Find ("ChooseCards");
        ShowCardsObject = GameObject.Find("ShowCards"); 
        CartesCimetiere = transform.Find("CartesCimetiere").gameObject;
        CarteBaseCimetiere = CartesCimetiere.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        CarteBaseCimetiere.SetActive(false); 
        CartesCimetiere.SetActive(false);
        EffetParticuleTerrain = GameObject.Find("EffetParticuleTerrain");
        EffetParticuleTerrain.SetActive(false);
        InfoCarteBattle = GameObject.Find("InfoCarte");
        InfoCarteBattle.SetActive(false);
        InfoCarteBattlePhone = GameObject.Find("InfoCartePhone");
        InfoCarteBattlePhone.SetActive(false); 
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
        ProposerDefairePile = GameObject.Find("ProposerDefairePile");
        ProposerDefairePile.SetActive(false);
        // StartCoroutine(PiocheDebut (6)); 
        SliderPause = GameObject.Find("SliderPause");
        SliderPause.SetActive(false);
        ChoixCartesAdversaire = GameObject.Find("ChoixCartesAdversaire");
        ChoixCartesAdversaire.SetActive(false);
        EndGame = GameObject.Find("EndGame");
        EndGame.SetActive(false);
        InfoEffetTransmis = GameObject.Find("InfoEffetTransmis");
        InfoEffetTransmis.SetActive(false);
        GameManagerInformation = GameObject.Find("InformationManager");
        eventManager = GameObject.Find("EventManager");
        AllButtonsPhase = GameObject.Find("AllButtonsPhases");
        StartCoroutine(SetUpPhaseButtons()); 
	}

	IEnumerator CoroutineDebugPhase(){
		syncPhase = false; 
		yield return new WaitForSeconds (0.5f); 
		print (Phase);
		syncPhase = true; 
	}

    /// <summary>
    /// Au tout début, selon la taille de l'écran 
    /// et le type d'appareil, on change le type de bouton.
    /// </summary>
    private IEnumerator SetUpPhaseButtons() {
#if (UNITY_ANDROID || UNITY_IOS)
        // Ici on ne met que le petit bouton. 
        AllButtonsPhase.SetActive(false); 
#else
        NextPhase.SetActive(false);
        int localPlayerID = 0;
        do {
            // On attend que le player ait bien spawn. 
            localPlayerID = FindLocalPlayerID();
            yield return new WaitForSeconds(0.05f);
        } while (localPlayerID == 0); 

        // Le joueur qui commence est le joueur 1
        if (localPlayerID == 1) {
            ChangeButtonsNewTour(InformationManager.TourJoueur.JOUEUR_LOCAL); 
        } else {
            ChangeButtonsNewTour(InformationManager.TourJoueur.JOUEUR_ADVERSE);
        }
#endif
    }

    /// <summary>
    /// Passage à la nouvelle phase lors du clic sur le bouton de passage à la nouvelle phase. 
    /// </summary>
    /// <param name="defairePile">true si on défait la pile, false sinon. </param>
	public void GoToNextPhase(bool defairePile=false){

        // On ne peut pas changer de phase quand ce n'est pas son tour. 
        if (Tour != FindLocalPlayerID()) {
            return; 
        }

        // Si d'autres effets sont en cours, on ne peut pas changer de phase. 
        if (!defairePile && GameObject.Find("Pile") != null) {
            DisplayMessage("Impossible de passer à une autre phase pour l'instant. ");
            return;
        }

        // On ne peut pas réagir lors après la phase d'initiation. 
        if (!defairePile && Phase != Player.Phases.INITIATION) {
            Debug.Log("On demande un changement de phase"); 
            AjouterChangementDePhasePile();
            // NextPhase.SetActive(false); 
        } else {
            GameObject PlayerObject = FindLocalPlayer();

            //On vérifie que le joueur n'ait pas trop de cartes dans la main en phase finale
            if (Phase == Player.Phases.FINALE) {
                if (CheckMainPleine(PlayerObject)) {
                    return;
                }
            }

            if (FindLocalPlayerID() == Tour) {
                if (Phase == Player.Phases.FINALE) {
                    // Il faut passer la main au joueur suivant!
                    Phase = Player.Phases.INITIATION;
                    // On change le tour. 
                    Tour = (Tour == 1) ? 2 : 1;
                    PlayerObject.SendMessage("ChangeUITour", Tour);
                }
                else {
                    Phase++;
                }

                PlayerObject.SendMessage("ChangeUIPhase", Phase);
                StartNewPhase();
            }
            // NextPhase.SetActive(true); 
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

    /// <summary>
    /// Changer l'affichae de l'AKA 
    /// </summary>
    /// <param name="newAKA">Le nouvel aka</param>
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
                Players[i].GetComponent<Player>().PiocherNouvelleCarte(); 
				return; 
			}
		}
	}

    /// <summary>
    /// Appelé lors du changement de phase. 
    /// Permet de mettre à jour l'information en haut à gauche de l'écran ainsi que
    /// de mettre à jour plusieurs choses slon les phases, comme l'AKA par exemple. 
    /// </summary>
    /// <param name="newPhase"></param>
	protected void setNamePhaseUI(Player.Phases newPhase){
		/*
		 * La phase courante est écrite en haut à gauche de l'écran.
		 */ 
		Phase = newPhase; 
		string PhaseToString = " ";

        SendNextPhaseAllCards(newPhase);

        // Pour certaines phases Preparation, principales, combat, on affiche au centre de l'écran avec 
        // tutoriels peut etre. 
        bool informationCentreEcran = false;

        // Si on est sur windows, et qu'on defait la pile, 
        // on update l'UI des boutons
        if (AllButtonsPhase != null && AllButtonsPhase.activeInHierarchy) {
            UpdateButtonsNewPhase((int)Phase);
        }

        switch (Phase) {
		    case Player.Phases.INITIATION:
			    PhaseToString = "Initiation"; 
			    ChangeAKAJoueurDebutTour ();
                // On reset le nombre de sorts lancés à chaque début de tour.    
                sortLance = 0; 
			    //CalculAKA (); 
			    nombreInvocationsSanctuaire = 0; 
			    // Après le calcul on passe directement à la phase suivante. 
			    Debug.Log("PHASE d'INITIATION");
                //GoToNextPhase(); 
                UpdateAllSorts(); 
			    break;
		    case Player.Phases.PIOCHE:
			    PhaseToString ="Pioche";
			    break;
		    case Player.Phases.PREPARATION:
			    PhaseToString ="Preparation";
                informationCentreEcran = true; 
                break; 
		    case Player.Phases.PRINCIPALE1:
			    PhaseToString ="Principale1";
                informationCentreEcran = true;
                break; 
		    case Player.Phases.PRINCIPALE2:
			    PhaseToString ="Principale2";
                informationCentreEcran = true;
                break; 
		    case Player.Phases.COMBAT:
			    PhaseToString ="Combat";
                informationCentreEcran = true;
                break; 
		    case Player.Phases.FINALE:
			    PhaseToString ="Finale";
                break; 
		}


        if (informationCentreEcran) {
            if (GameManagerInformation != null) {
                GameManagerInformation.GetComponent<InformationManager>().SetInformation(Phase);
            }
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
    public void SendNextPhaseAllCards(Player.Phases currentPhase) {
        /*
         * Envoyer à toutes les cartes qu'on change de phase. 
         */

        StartCoroutine(SendNextPhaseAllCardsRoutine(currentPhase)); 
    }

    /// <summary>
    /// Routine associée à <see cref="SendNextPhaseAllCards(Player.Phases)"/>
    /// qui permet d'attendre que les effets joueurs aient été mis à jour. 
    /// </summary>
    /// <param name="currentPhase"></param>
    /// <returns></returns>
    private IEnumerator SendNextPhaseAllCardsRoutine(Player.Phases currentPhase) {
        yield return AttendreEffetsJoueur(); 
        UpdatePhaseAllCards(currentPhase);
    }

    /// <summary>
    /// Update la phhase et les effets sur toutes les cartes. 
    /// Peut être delayed par SendNextPhaseAllCardsRoutine (dans le cas où ce n'est pas le tour
    /// du joueur chez lequel on vérifie tous les effets). 
    /// </summary>
    /// <param name="currentPhase"></param>
    private void UpdatePhaseAllCards(Player.Phases currentPhase) { 
        GameObject[] AllEntites = GameObject.FindGameObjectsWithTag("BoardSanctuaire"); 
        for (int i = 0; i < AllEntites.Length; ++i) {
            try {
                if (AllEntites[i].GetComponent<Entite>().isFromLocalPlayer) {
                    AllEntites[i].GetComponent<Entite>().UpdateNewPhase(currentPhase, Tour);
                }
            } catch (NullReferenceException e) {
                // Debug.Log(e); 
            }
        }

        GameObject[] AllAssistances = GameObject.FindGameObjectsWithTag("Assistance");
        for (int i = 0; i < AllAssistances.Length; ++i) {
            try {
                if (AllAssistances[i].GetComponent<Assistance>() != null && AllAssistances[i].GetComponent<Assistance>().isFromLocalPlayer &&
                    AllAssistances[i].GetComponent<Assistance>().AssistanceState == Assistance.State.JOUEE) {
                    AllAssistances[i].SendMessage("UpdateNewPhase", currentPhase);
                }
            } catch (NullReferenceException e) {
                Debug.Log(e); 
            }
        }

        // Une fois qu'on a cherché tous les effets, on les joue depuis l'eventManager
        JouerEventManager(); 
    }

    /// <summary>
    /// Changer le tour
    /// </summary>
    /// <param name="newTour"></param>
	public void setTour(int newTour){

		Tour = newTour; 
		CurrentTour.GetComponent<Text> ().text = "Tour " + Tour.ToString();

        if (GameManagerInformation != null) {
            // Inforlation visuelle pour le joueur. 
            if (newTour == FindLocalPlayerID()) {
                GameManagerInformation.GetComponent<InformationManager>().SetInformation(InformationManager.TourJoueur.JOUEUR_LOCAL);
                if (AllButtonsPhase.activeInHierarchy) {
                    // Dans le cas où on est sur un grand écran, 
                    // on update tous les boutons
                    ChangeButtonsNewTour(InformationManager.TourJoueur.JOUEUR_LOCAL);
                }
            }
            else {
                GameManagerInformation.GetComponent<InformationManager>().SetInformation(InformationManager.TourJoueur.JOUEUR_ADVERSE);
                if (AllButtonsPhase.activeInHierarchy) {
                    // Dans le cas où on est sur un grand écran, 
                    // on update tous les boutons
                    ChangeButtonsNewTour(InformationManager.TourJoueur.JOUEUR_ADVERSE);
                }
            }
        }
	}

    /// <summary>
    /// Trouver l'ID du joueur qui n'est pas local. 
    /// </summary>
    /// <returns>Nn entier contenant 1 ou 2. </returns>
	public static int FindLocalPlayerID(){
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
	public virtual void AttackMyPlayer(GameObject MyPlayer){
		/*
		 * Choix de la première carte d'attaque, celle du joueur
		 */ 
		MyPlayerEntity = MyPlayer; 
		// On s'assure qu'il n'y ait pas d'autre fleches rouges
		Destroy(GameObject.FindGameObjectWithTag ("Marque"));
		// Instantiate (HitPrefab, MyPlayer.transform.position, MyPlayer.transform.rotation); 
	}

    /// <summary>
    /// Choix de la carte A attaquer. 
    /// </summary>
    /// <param name="OtherPlayer">La carte attaquée</param>
	public virtual void AttackOtherPlayer(GameObject OtherPlayer){
		/*
		 * Choix de la carte à attaquer, celle de l'adversaire. 
		 */ 
		OtherPlayerEntity = OtherPlayer; 

        // Si la carte est inexistante
        if (OtherPlayer == null) {
            Debug.LogError("Il n'y a pas de cartes");
            DisplayMessage("Impossible de trouver la carte à attaquer");
            return; 
        }
		Attack (); 
	}

    /// <summary>
    /// Attaque d'ue carte, une assistance ou même directement du joueur par une autre carte.
    /// </summary>
    /// <param name="STAT">Si on ne veut pas utiliser la stat de la carte mais celle d'un effet, on peut la passer en paramètre</param>
	public virtual void Attack(bool JouerEffet = false, GameObject _MyPlayerEntity = null, 
        GameObject _OtherPlayerEntity = null, int STAT = 0){
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

        try {
            Entite.Ascendance AscendanceMyEntity = MyPlayerEntity.GetComponent<Entite>().EntiteAscendance;
            Entite.Element ElementMy = MyPlayerEntity.GetComponent<Entite>().EntiteElement;
        } catch (NullReferenceException e) {
            Debug.Log(e);
            Debug.Log("Appui accidentel");
            return; 
        }
		int multiplicateurDegatsMy = 1;

		// Debug.Log (OtherPlayerEntity.tag);

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
                    if (STAT == 0) {
                        OtherPlayerEntity.SendMessage("AttackPlayer", MyPlayerEntity.GetComponent<Entite>().getPuissance());
                    } else {
                        // On override la STAT. 
                        OtherPlayerEntity.SendMessage("AttackPlayer", STAT); 
                    }
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
            // Si l'entité attaquée est une ENTITE du joueur adverse. 
            if (JouerEffet) {

                List<int> Multiplicateurs = CalculMultiplicateurs(MyPlayerEntity, OtherPlayerEntity); 
                int multiplicateurDegatsOther = Multiplicateurs[1];
                multiplicateurDegatsMy = Multiplicateurs[0]; 
                

                Debug.Log("Il vient d'y avoir une attaque");
                int attackMy = MyPlayerEntity.GetComponent<Entite>().getPuissance() * multiplicateurDegatsMy;
                int attackOther = OtherPlayerEntity.GetComponent<Entite>().getPuissance() * multiplicateurDegatsOther;
                Debug.Log("Attaque faite au personnage 1 : " + attackOther.ToString());
                Debug.Log("Attaque faite au personnage 2 : " + attackMy.ToString());

                // On Détruit la carte la plus faible, ou les deux dans un cas d'égalité
                if (multiplicateurDegatsMy * MyPlayerEntity.GetComponent<Entite>().getPuissance() ==
                   multiplicateurDegatsOther * OtherPlayerEntity.GetComponent<Entite>().getPuissance()) {
                    // Cas d'égalité
                    MyPlayerEntity.SendMessage("DetruireCarte");
                    OtherPlayerEntity.SendMessage("DetruireCarte");
                }
                else if (multiplicateurDegatsMy * MyPlayerEntity.GetComponent<Entite>().getPuissance() >
                        multiplicateurDegatsOther * OtherPlayerEntity.GetComponent<Entite>().getPuissance()) {
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

    /// <summary>
    /// Calcul les multiplicateurs des deux entités qui se battent
    /// </summary>
    /// <returns>en [0], mon entité, en [1] l'entité adverse. </returns>
    private List<int> CalculMultiplicateurs(GameObject MyPlayerEntity, GameObject OtherPlayerEntity) {

        Entite.Nature NatureOtherEntity = OtherPlayerEntity.GetComponent<Entite>().EntiteNature;
        Entite.Element ElementOther = OtherPlayerEntity.GetComponent<Entite>().EntiteElement;

        Entite.Nature NatureMyEntity = MyPlayerEntity.GetComponent<Entite>().EntiteNature;
        Entite.Element ElementMy = MyPlayerEntity.GetComponent<Entite>().EntiteElement;

        List<int> MultiplicateursRetour = new List<int>();
        // On ajoute 1 comme multiplicateur de base de mon entité
        int MultiplicateurMyEntite = 1; 
        // On ajoute 1 comme multiplicateur de base de l'autre entité. 
        int MultiplicateurEntiteOther = 1;

        // On regarde d'abord les ascendances des deux entités. 
        if (NatureMyEntity == Entite.Nature.NEUTRE || NatureOtherEntity == Entite.Nature.NEUTRE) {
            // Si un des elements est neutre, l'autre n'est ni fort ni faible face à lui. 
        }
        else if (NatureMyEntity == Entite.Nature.ELEMENTAIRE && NatureOtherEntity != Entite.Nature.ELEMENTAIRE) {
            MultiplicateurMyEntite = 2;
        }
        else if (NatureMyEntity != Entite.Nature.ELEMENTAIRE && NatureOtherEntity == Entite.Nature.ELEMENTAIRE) {
            MultiplicateurEntiteOther = 2;
        }
        else if (NatureMyEntity == Entite.Nature.ELEMENTAIRE && NatureOtherEntity == Entite.Nature.ELEMENTAIRE) {
            // SI les deux sont élémentaires

            // vérifier ici les condition aux limites avec les derniers éléments à chaque fois.
            // On en peut pour l'instant pas gérer les forces de terre et air avec les les enums. 
            if ((ElementMy - ElementOther == 1 )|| (ElementMy == Entite.Element.AIR && ElementOther == Entite.Element.TERRE)) {
                MultiplicateurEntiteOther = 2;
            }
            else if ((ElementOther - ElementMy == 1) || (ElementMy == Entite.Element.TERRE && ElementOther == Entite.Element.AIR)) {
                MultiplicateurMyEntite = 2;
            }
        }

        // On regarde si NOTRE entité est forte face à l'AUTRE (si forteFace == 1, alors NOTRE est forte face à AUTRE)
        int forteFace = MyPlayerEntity.GetComponent<Entite>().estForteFaceA(OtherPlayerEntity); 
        if (forteFace == 1) {
            MultiplicateurMyEntite = 1; 
        } else if (forteFace == -1) {
            MultiplicateurEntiteOther = 1; 
        }


        MultiplicateursRetour.Add(MultiplicateurMyEntite);
        MultiplicateursRetour.Add(MultiplicateurEntiteOther);
        return MultiplicateursRetour; 
    }

    /// <summary>
    /// Ajouter une attaque comme un effet dans la pile. 
    /// </summary>
    private void AjouterEffetAttaquePile() {
        // On prévient qu'on va jouer l'effet. 
        List<GameObject> Cibles = new List<GameObject> {
            // On ajoute le joueur adverse à la liste des cibles. 
            // Il faudra bien faire un cas séparé lors de la recherche de l'ID lorsqu'on défait la pile.
            OtherPlayerEntity
        };
        // -1 est le nombre spécial pour une attaque.
        MettreEffetDansLaPile(MyPlayerEntity, Cibles, -1);

        MyPlayerEntity = null;
        OtherPlayerEntity = null;

        // On redétruit la fleche rouge. 
        Destroy(GameObject.FindGameObjectWithTag("Marque"));

        return;
    }

    /// <summary>
    /// Ajouter un effet d'attaque à la pile de l'adversaire. 
    /// </summary>
    /// <param name="CarteAttaque"></param>
    public void AjouterEffetAttaquePileJoueurAdverse(GameObject CarteAttaque) {
        List<GameObject> Cibles = new List<GameObject> {
            FindNotLocalPlayer()
        };
        MettreEffetDansLaPile(CarteAttaque, Cibles, -1); 
    }

    /// <summary>
    /// Ajouter le changement de phase comme un effet dans la pile. 
    /// </summary>
    private void AjouterChangementDePhasePile() {
        // -4 est le nombre spécial pour le changement de phase
        MettreEffetDansLaPile(null, new List<GameObject>(), -4); 
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
		_CartesChoisies = new List<GameObject>(); 
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
		while (_CartesChoisies.Count < nombreCartes) {
			yield return new WaitForSeconds (0.5f); 
		}
	}

	//public void CarteChoisie(GameObject Carte){
    // INUTILISEE
	//	// La carte envoie un message au GameManager pour dire qu'elle a été choisie. 
	//	if (!choixCartes) {
	//		throw new Exception ("Le choix des cartes était désactivé. Cette méthode ne devrait pas être appelée"); 
	//	}
	//	CartesChoisies.Add (Carte); 
	//}

    /// <summary>
    /// Pioche de cartes au début du jeu.
    /// CETTE METHODE N'EST PLUS UTILISEE => Voir => MontrerCartesAChoisirDebut
    /// </summary>
    /// <param name="nombreCartes"></param>
    /// <returns></returns>
	public virtual IEnumerator PiocheDebut(int nombreCartes){
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

    /// <summary>
    /// Changer l'AKA sur l'UI visible. 
    /// 
    /// Mettre à joueur l'AKA du joueur sur l'écran. 
    /// Si le joueur a l'ID 1 c'est qu'il est le serveur.
    /// Sinon c'est le client. 
    /// 
    /// </summary>
    /// <param name="IDJoueur"></param>
    /// <param name="AKA"></param>
	public void setPlayerAKAUI(int IDJoueur, int AKA){

		if (FindLocalPlayer ().GetComponent<Player> ().isServer) {
			// Si le joueur local est le server. 
			if (IDJoueur == 1) {
                ChangerAKAJoueurUI(AKA, true); 
            } else {
                ChangerAKAJoueurUI(AKA, false);
            }
		} else {
			if (IDJoueur == 2) {
                ChangerAKAJoueurUI(AKA, true);
            } else {
                ChangerAKAJoueurUI(AKA, false);
            }
		}
    }

    /// <summary>
    /// Changer l'AKA d'un des joueurs
    /// </summary>
    /// <param name="newAKA"></param>
    private void ChangerAKAJoueurUI(int newAKA, bool local, bool newTurn=false) {
        
        // On change le Nomjoueur 1 si local est à true, 
        // on change NomJoueur2 sinon. 
        GameObject NomJoueur = local ? NomJoueur1.transform.Find("AKAJoueur1").gameObject : NomJoueur2.transform.Find("AKAJoueur2").gameObject;

        Debug.Log(NomJoueur);
        Debug.Log(NomJoueur.GetComponent<Text>()); 

        // On change la version textuelle.
        NomJoueur.GetComponent<Text>().text = "AKA:" + newAKA.ToString() + 
            "/" + AKARemanent.ToString();
        
        // On change la version slider. 

// #if false
        // On garde ce bout de code mort pour une bonne raison. 
        // Si on a besoin d'améliorer les performances, on enlèvera les particules et on gardera cette version. 
//        NomJoueur1.transform.Find("AKAJoueur1").Find("AKASliderJoueur1").gameObject.
//            GetComponent<Slider>().value = (float)AKA/AKARemanent;
//#endif

        // On change le slider 1 si on est sur le joueur local, 
        // sinon on change le slider 2. 
        GameObject Slider = local ? GameObject.Find("AKASliderJoueur1") : GameObject.Find("AKASliderJoueur2"); 

        // On change la version avec particules.
        // Normalement ce slider doit se trouver sur un autre layer de camera. 
        Slider.GetComponent<SliderAKA>().ChangeCurrentAKA(newAKA, AKARemanent, newTurn);

    }


    /// <summary>
    /// Au début de chaque tour
    /// l'ID du joueur est 1 si le joueur est le serveur
    /// L'ID du joueur est 2 si le joueur n'est pas le serveur.
    ///
    /// On met l'AKA des deux joueurs à jour. 
    /// </summary>
    private void ChangeAKAJoueurDebutTour(){
		int currentAKA = GameObject.FindGameObjectsWithTag ("BoardSanctuaire").Length;
        AKARemanent = currentAKA;

        // On change les barres d'AKA du joueur local et du pas local. 
        ChangerAKAJoueurUI(AKARemanent, true, true);
        ChangerAKAJoueurUI(AKARemanent, false, true);

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
		yield return new WaitForSeconds (2f); 
		Capacite_Effet.GetComponent<Text> ().text = ""; 

	}

    /// <summary>
    /// Invocation d'une carte élémentaire air. 
    /// </summary>
    /// <param name="cout"></param>
	public void InvocationElementaireAir(int cout){

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
		FindLocalPlayer().GetComponent<Player>().CmdSendCards (AllCardsRandom, "Cartes de votre adversaire", FindLocalPlayerID());
	}

    /// <summary>
    /// Mettre à jour tous les sorts. 
    /// </summary>
    private void UpdateAllSorts() {
        /*
         * A chaque tour on update les sorts qui sont en cours. 
         */ 
        GameObject[] SortsJoues = GameObject.FindGameObjectsWithTag("SortJoue");
        for (int i = 0; i < SortsJoues.Length; ++i) {
            try {
                SortsJoues[i].GetComponent<Sort>().UpdateSortNewTurn();
            } catch (MissingReferenceException e){
                Debug.LogError("Erreur recupérée " + e);
            } catch (NullReferenceException e) {
                Debug.LogError("Erreur recupérée "+ e);
            }
        }
    }

    /// <summary>
    /// Montrer les cartes dans le cimetière
    /// </summary>
    /// <param name="allCartesCimetiere">Cartes présentes dans le cimetière</param>
    public void ShowCemetery(List<GameObject> allCartesCimetiere) {
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
        
        Debug.Log(CarteBaseCimetiere); 

        for (int i = 0; i < allCartesCimetiere.Count; ++i) {
            // On instancie une nouvelle carte pour chaque carte du cimetière.
            GameObject NouvelleCarte = Instantiate(CarteBaseCimetiere);
            // NouvelleCarte.AddComponent<Carte>(); 
            NouvelleCarte.GetComponent<ImageCarteCimetiere>().setImage(allCartesCimetiere[i].GetComponent<Carte>().shortCode);
            NouvelleCarte.GetComponent<ImageCarteCimetiere>().Info = allCartesCimetiere[i].GetComponent<Carte>().GetInfoCarte(); 
            Debug.Log("1 carte instanciée dans le cimetière");
            NouvelleCarte.transform.SetParent(Content.transform, false);
            NouvelleCarte.SetActive(true); 
        }
        
    }

    /// <summary>
    /// Montrer les cartes dans le ban
    /// <seealso cref="ShowCemetery(List{GameObject})"/>
    /// </summary>
    /// <param name="allCartesCimetiere">Cartes présentes dans le cimetière</param>
    public void ShowBan(List<GameObject> allCartesBan, int nombreCartesFaceCachee = 0) {
        /*
         * Montrer les cartes dans le cimetiere. 
         */

        // On "renomme" juste la fonction car les fonctions ShowBan et ShowCemetery son les mêmes fonctions
        // Ici on montre les cartes face visible
        ShowCemetery(allCartesBan);

        GameObject Content = CartesCimetiere.transform.GetChild(0).GetChild(0).gameObject;

        // Puis on ajoute les cartes face cachée. 
        for (int i = 0; i < nombreCartesFaceCachee; ++i) {
            // On instancie une nouvelle carte pour chaque carte du cimetière.
            GameObject NouvelleCarte = Instantiate(CarteBaseCimetiere);
            CarteBaseCimetiere.GetComponent<ImageCarteCimetiere>().setImageBan(); 
            NouvelleCarte.transform.SetParent(Content.transform, false);
            NouvelleCarte.SetActive(true);
        }
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

    /// <summary>
    /// Cacher le cimetiere.
    /// <seealso cref="HideCemetery"/>
    /// </summary>
    void HideBan() {
        /*
         * Cacher le cimetiere
         */
        HideCemetery(); 
    }

    /// <summary>
    /// Montrer l'effet du terrain. 
    /// Changer l'effet du terrain. 
    /// </summary>
    /// <param name="_ascendance">Ascnedance de la carte</param>
    void EffetTerrain(AscendanceTerrain _ascendance) {
        /*
         * On montre l'effet de terrain par un effet de particule,
         * Noires dans le cas d'un effet maléfique,
         * Blanches dans le cas d'un effet astral, 
         * Pas d'effet si le terrain n'a pas d'effet.
         * 
         */

        GameManager.AscendanceTerrain previousAscendance = ascendanceTerrain; 

        if (_ascendance == AscendanceTerrain.MALEFIQUE) {
            ascendanceTerrain = AscendanceTerrain.MALEFIQUE; 
            EffetParticuleTerrain.SetActive(true);
#pragma warning disable CS0618 // 'ParticleSystem.startColor' est obsolète : 'startColor property is deprecated. Use main.startColor instead.'
            EffetParticuleTerrain.GetComponent<ParticleSystem>().startColor = Color.black; 
#pragma warning restore CS0618 // 'ParticleSystem.startColor' est obsolète : 'startColor property is deprecated. Use main.startColor instead.'
        } else if (_ascendance == AscendanceTerrain.ASTRALE) {
            ascendanceTerrain = AscendanceTerrain.ASTRALE;
            EffetParticuleTerrain.SetActive(true);
#pragma warning disable CS0618 // 'ParticleSystem.startColor' est obsolète : 'startColor property is deprecated. Use main.startColor instead.'
            EffetParticuleTerrain.GetComponent<ParticleSystem>().startColor = Color.white;
#pragma warning restore CS0618 // 'ParticleSystem.startColor' est obsolète : 'startColor property is deprecated. Use main.startColor instead.'
        } else {
            ascendanceTerrain = AscendanceTerrain.NONE;
            EffetParticuleTerrain.SetActive(false); 
        }

        StartCoroutine(AttendreGererEffetsToutesCartesChangementTerrain(previousAscendance)); 
    }

    /// <summary>
    /// Attendre avant de jouer les effets de joueurs. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator AttendreEffetsJoueur() {

        // On s'assure qu'on a bien remis l'entier à 0. 
        if (FindLocalPlayerID() == Tour) {
            FindLocalPlayer().GetComponent<Player>().CmdOnEffetPlayer(0);
        }

        while (FindLocalPlayer().GetComponent<Player>().EffetPlayer != 0) {
            // Debug.Log("On attend"); 
            yield return new WaitForSeconds(0.3f);
        }

        // Si ce n'est pas le tour de ce joueur
        if (FindLocalPlayerID() != Tour) {
            // Tant que l'entier reste à 0, on attend
            while (FindLocalPlayer().GetComponent<Player>().EffetPlayer == 0) {
                Debug.Log("On attend"); 
                yield return new WaitForSeconds(0.3f);
            }

            // Dans le cas où l'entier test n'est pas celui de notre joueur, on attend encore
            // Et on instantie l'objet d'information. 
            if (FindLocalPlayer().GetComponent<Player>().EffetPlayer != FindLocalPlayerID()) {
                GetComponent<InformerAdversaireChoixEffet>().AdversaireChoisitEffet();
                while (FindLocalPlayer().GetComponent<Player>().EffetPlayer != FindLocalPlayerID()) {
                    // Debug.Log("Le joueur choisit des effets");
                    yield return new WaitForSeconds(0.1f);
                }
                // Une fois que c'est notre tour, on detruit l'objet d'information. 
                GetComponent<InformerAdversaireChoixEffet>().DetruireAdversaireChoisitEffet();
            }

            if (FindLocalPlayer().GetComponent<Player>().EffetPlayer != FindLocalPlayerID()) {
                throw new UnusualBehaviourException("L'entier devrait être celui du joueur");
            }
        }
        // Si c'est le tour du joueur
        else {
            Debug.Log("C'est mon tour"); 
            // On vérifie que la variable globale soit bien à 0. 
            if (FindLocalPlayer().GetComponent<Player>().EffetPlayer != 0) {
                Debug.LogError("L'entier EffetPlayer devrait être à 0. Sa valeur est " +
                    FindLocalPlayer().GetComponent<Player>().EffetPlayer);
                // throw new UnusualBehaviourException("L'entier EffetPlayer devrait être à 0. Sa valeur est " + 
                //    FindLocalPlayer().GetComponent<Player>().EffetPlayer);
            }
        }
    }

    /// <summary>
    /// Gerer les effets de toutes les cartes lors d'un changement de terrain. 
    /// </summary>
    /// <returns></returns>
    IEnumerator AttendreGererEffetsToutesCartesChangementTerrain(AscendanceTerrain previousAscendance) {
        Debug.Log("On attend les effets des autres joueurs"); 
        yield return AttendreEffetsJoueur(); 
        GererEffetsToutesCartesChangementTerrain(previousAscendance); 

    }

    /// <summary>
    /// Gerer les effets de toutes les cartes au changement de terrain. 
    /// </summary>
    /// <param name="previousAscendance"></param>
    private void GererEffetsToutesCartesChangementTerrain(AscendanceTerrain previousAscendance) {
        GameObject[] AllCartes = GameObject.FindGameObjectsWithTag("BoardSanctuaire");
        for (int i = 0; i < AllCartes.Length; ++i) {
            AllCartes[i].GetComponent<Entite>().UpdateChangementAscendanceTerrain(ascendanceTerrain, previousAscendance);
        }
        JouerEventManager();
    }

    /// <summary>
    /// <see cref="EffetTerrain(Entite.Ascendance)"/>
    /// </summary>
    /// <param name="_ascendanceTerrain">Nouvelle ascendance du terrain.</param>
    public void EffetTerrain(string nature) {
        // ascendanceTerrain = _ascendanceTerrain;

        switch (nature) {
            case "ASTRALE":
            case "ASTRAL":
                EffetTerrain(AscendanceTerrain.ASTRALE); 
                break;
            case "MALEFIQUE":
                EffetTerrain(AscendanceTerrain.MALEFIQUE);
                break;
            case "NEUTRE":
                EffetTerrain(AscendanceTerrain.NONE);
                break; 
        }
    }

    /// <summary>
    /// Récupérer l'ascendance du terrain. 
    /// </summary>
    /// <returns></returns>
    public AscendanceTerrain GetAscendanceTerrain() {
        return ascendanceTerrain; 
    }

    /// <summary>
    /// Changer l'ascendance du terrain depuis la console de développement. 
    /// </summary>
    /// <param name="_ascendanceTerrain">Nouvelle ascendance du terrain.</param>
    public void SetAscendanceTerrain(AscendanceTerrain _ascendanceTerrain) {
        FindLocalPlayer().GetComponent<Player>().CmdChangeAscendanceTerrain(_ascendanceTerrain); 
    }

    /// <summary>
    /// Montrer les infos d'une carte.
    /// </summary>
    /// <param name="shortCode">shortCode de la carte</param>
    /// <param name="Info">l'Info à montrer</param>
    /// <param name="Effet">Sur téléphone on peut proposer les effets depuis ici.</param>
    public void ShowCarteInfo(string shortCode, string Info, int IDCardGame=-1) {
        /*
         * Afficher les informations liées à la carte. 
         */
#if (UNITY_ANDROID || UNITY_IOS)
        InfoCarteBattlePhone.SetActive(true);
        InfoCarteBattlePhone.transform.Find("PeutEtreJoue").gameObject.SetActive(false); 
        InfoCarteBattlePhone.GetComponent<InfoCarteBattle>().SetInfoCarte(shortCode, Info);
        if (IDCardGame != -1) {
            // Dans le cas où un effet peut être joué
            InfoCarteBattlePhone.transform.Find("PeutEtreJoue").gameObject.SetActive(true);
            InfoCarteBattlePhone.transform.Find("PeutEtreJoue").Find("Text").gameObject.GetComponent<Text>().text =
                "Un effet peut etre joué ! ";  
            IDCardGameAttenteJouerEffet = IDCardGame;
        }
#else
        InfoCarteBattle.SetActive(true);
        InfoCarteBattle.GetComponent<InfoCarteBattle>().SetInfoCarte(shortCode, Info);
#endif
    }

    /// <summary>
    /// Retrouver l'effet que la carte peut jouer.
    /// </summary>
    public void ClicJouerEffetCarte() {
        if (IDCardGameAttenteJouerEffet != -1) {
            // On stocke pour ne pas perdre la valeur à l'appel de HideInfoCarte
            int IDCardJoue = IDCardGameAttenteJouerEffet;
            HideInfoCarte();
            // On simule un clic droit sur la carte. 
            FindCardWithID(IDCardJoue).GetComponent<Carte>().RightClickOnCarte(); 
        }
    }

    /// <summary>
    /// Cacher le panneau qui montre les infos de la carte.
    /// </summary>
    public void HideInfoCarte() {
#if (UNITY_ANDROID || UNITY_IOS)
        InfoCarteBattlePhone.SetActive(false);
        IDCardGameAttenteJouerEffet = -1; 
#else
        InfoCarteBattle.SetActive(false);
#endif
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
                Debug.Log("Le jeu reprend"); 
                // Une fois qu'il a fini ses actions on défait la pile. 
                PauseButton.SetActive(false);
                //On regarde s'il y a une pile d'effet en cours. 
                AntinomiaLog("Je défais la pile"); 
                DefairePile(true); 

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
                DefairePile(true); 
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
    /// Defaire la pile des effets
    /// </summary>
    /// <param name="jeuEnPause">true si le jeu a été mis en pause. Si le jeu a été mis en pause, 
    /// on empêche le changement de phase.</param>
    void DefairePile(bool jeuEnPause = false) {
        if (GameObject.FindGameObjectWithTag("Pile") != null) {
            GameObject Pile = GameObject.FindGameObjectWithTag("Pile");
            if (Pile.GetComponent<PileAppelEffet>().DernierEffetVientJoueur(FindLocalPlayer().GetComponent<Player>().PlayerID)) {
                // Si c'est le joueur qui a mis un effet en dernier dans la pile, on la défait. 
                AntinomiaLog("Après avoir mis le jeu en pause, je défais la pile");
                Pile.GetComponent<PileAppelEffet>().DefaireLaPile(jeuEnPause);
                AntinomiaLog("C'est moi qui défais la pile"); 
            }
            //Sinon on ne fait rien. 
        }
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
    /// en pause est géré par un autre élément.Exemple: 
    /// Lors d'un changement de phase, c'est déjà géré dans la fonction du gameManager. 
    /// 
    /// Sinon, l'ID passée en paramètre est celle proposée par le joueur qui appelle la fonction. 
    /// La proposition du bouton pause est donc uniquement pour celui qui n'a pas fait l'action
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="message">Lorsque le jeu est mis en pause, on peut vouloir afficher un message au joueur</param>
    /// <returns>None</returns>
    public IEnumerator ProposeToPauseGame(int playerID=0, string message="") {
        if ((FindLocalPlayerID() != playerID) || (playerID == 0)) {
            Debug.Log("<color=orange> Pauser le jeu? </color>"); 
            // Si on a reçu un effet, c'est que l'adversaire a réagi, le jeu n'est plus en pause. 
            FindLocalPlayer().GetComponent<Player>().CmdOnlySetPause(false); 
            PauseButton.SetActive(true);
            StartCoroutine(ShowTimeSpendPause(1f)); 
            if (message != ""){
                // Si un message est fourni lors de l'appel, on l'affiche
                DisplayInfoToPlayer(message);
            }
            // Temps d'attente. 
            AntinomiaLog("Attente");
            // MontrerQuellesCartesPeuventJoueur();  // TODO : Cette méthode ne fonctionne pas pour l'instant. 
            yield return new WaitForSeconds(2f);
            if (!gameIsPaused) {
                PauseButton.SetActive(false);
                //On regarde s'il y a une pile d'effet en cours. 
                if (GameObject.FindGameObjectWithTag("Pile") != null) {
                    AntinomiaLog("Je n'ai pas mis en pause, j'explique à l'autre joueur que je n'ai pas souhaité répondre");
                    // GameObject.FindGameObjectWithTag("Pile").GetComponent<PileAppelEffet>().DefaireLaPile(); 
                    FindLocalPlayer().GetComponent<Player>().CmdProposerDefaireLaPile(FindLocalPlayerID()); 
                }
            }
        }
    }

    /// <summary>
    /// Informer l'autre joueur d'un effet sans lui proposer de mettre le jeu en pause ! 
    /// Peut arriver lorsque plusieurs effets sont joués à la suite chez un joueur. 
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="message"></param>
    public void InformerEffet(int playerID = 0, string message="") {
        if ((FindLocalPlayerID() != playerID) || (playerID == 0)) {
            // Si on a reçu un effet, c'est que l'adversaire a réagi, le jeu n'est plus en pause. 
            StartCoroutine(ShowTimeSpendPause(1f));
            if (message != "") {
                // Si un message est fourni lors de l'appel, on l'affiche
                DisplayInfoToPlayer(message);
            }
        }
    }

    /// <summary>
    /// Montrer au joueur pendant combien de temps il peut encore faire pause.
    /// ANIMATION
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowTimeSpendPause(float time) {
        float timeSpent = 1f;
        SliderPause.SetActive(true); 
        while (timeSpent >= 0f) {
            SliderPause.GetComponent<Image>().fillAmount = timeSpent;
            timeSpent -= 1f / 50f;
            yield return new WaitForSeconds(time / 50f);
            // Debug.Log(timeSpent); 
        }
        SliderPause.SetActive(false); 
    }

    /// <summary>
    /// Proposer au joueur de défaire la pile.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ProposerDefaireLaPile() {
        // On vérifie d'abord que le seul élément dans la pile ne soit pas un changement de phase par exemple. 

        // Necessite l'approbation
        if (getPile().GetComponent<PileAppelEffet>().DefaireNecessiteApprobationJoueur()) {
            ProposerDefairePile.SetActive(true);
            while (defairePile == 0) {
                yield return new WaitForSeconds(0.1f);
            }
            if (defairePile == 1 && GameObject.FindGameObjectWithTag("Pile") != null) {
                GameObject.FindGameObjectWithTag("Pile").GetComponent<PileAppelEffet>().DefaireLaPile();
            }
            else {
                // Sinon on ne fait rien 
            }
            ProposerDefairePile.SetActive(false);
            defairePile = 0;
        } else {
            GameObject.FindGameObjectWithTag("Pile").GetComponent<PileAppelEffet>().DefaireLaPile();
        }
    }

    /// <summary>
    /// Permettre au joueur de choisir s'il veut garder sa première pioche, sa deuxième, sa troisième...
    /// </summary>
    /// <returns></returns>
    public IEnumerator MontrerCartesAChoisirDebut(int nombreCartes = 6) {
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

        StartCoroutine(MontrerCartesAChoisirDebut2(_cartesDebut, nombreCartes, 0)); 

    }

    /// <summary>
    /// Coroutine permettant de réafficher les cartes s'il y a un probleme réseau de récupération. 
    /// </summary>
    /// <param name="_cartesDebut"></param>
    /// <param name="nombreCartes"></param>
    /// <param name="nombreEssais"></param>
    /// <returns></returns>
    public IEnumerator MontrerCartesAChoisirDebut2(List<GameObject> _cartesDebut, int nombreCartes, int nombreEssais) { 

        Transform ParentCartesDebut = ChoixCartesDebut.transform.Find("ChoixCartesDebutFond");

        // On détruit toutes les cartes déjà display piochées auparavant. 
        if (ParentCartesDebut.childCount != 1) {
            for (int i = 0; i < ParentCartesDebut.childCount; ++i) {
                if (ParentCartesDebut.GetChild(i).gameObject != CarteDebutPrefab) {
                    Destroy(ParentCartesDebut.GetChild(i).gameObject); 
                }
            }
        }

        // Passe à true s'il y a eu un Probleme
        bool probleme = false; 
        for (int i = 0; i < nombreCartes; ++i) {
            GameObject newCarte = Instantiate(CarteDebutPrefab);
            newCarte.transform.SetParent(ParentCartesDebut, false);
            if (_cartesDebut[i].GetComponent<Carte>().shortCode.Length < 3) {
                Debug.Log(_cartesDebut[i].GetComponent<Carte>().oID);
                Debug.Log(_cartesDebut[i].GetComponent<Carte>().shortCode);
                Debug.Log(_cartesDebut[i].GetComponent<Carte>().IDCardGame);
                probleme = true; 
                if (nombreEssais > 5) {
                    Debug.LogError("Un probleme ici. On aurait du avoir les cartes");
                    // throw new UnusualBehaviourException("Cette carte n'a pas de shortCode, ce n'est pas normal." +
                    //    _cartesDebut[i].GetComponent<Carte>().oID +  " "  +
                    //    _cartesDebut[i].GetComponent<Carte>().shortCode+ " " +
                    //    _cartesDebut[i].GetComponent<Carte>().IDCardGame
                    //);
                }  else {
                    yield return new WaitForSeconds(0.5f);
                    yield return MontrerCartesAChoisirDebut2(_cartesDebut, nombreCartes, nombreEssais + 1);
                    Debug.LogError("<color=red> Un probleme ici</color>");
                }
            }
            if (!probleme) {
                // Si ça ne marche pas, il FAUT différencier, sort, entité, et assistance. 
                newCarte.GetComponent<CarteDebut>().InfoDebut(_cartesDebut[i].GetComponent<Carte>().shortCode,
                        _cartesDebut[i].GetComponent<Carte>().GetInfoCarte());
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
            // On retient le nombre de cartes choisies pour le vérifier. 
            nombreDeCartesChoisies = nombreCartes;
            CheckBonNombreCartes();

            FindLocalPlayer().GetComponent<Player>().CmdChoixCartesFini(); 
            if (ChoixCartesAdversaire != null) {
                ChoixCartesAdversaire.SetActive(true);
            }

            yield return CheckCartePiocheesOK();
        }
    }

    /// <summary>
    /// Coroutine pour verifier que toutes les cartes ont bien été piochées. 
    /// Sinon on les reinstancient. 
    /// AUTANT DE FOIS QU'IL LE FAUDRA!
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckCartePiocheesOK() {
        // il faut regarder qu'il n'y ait plus d'object Info dans la scène. 
        // Si il en reste quelque chose s'est mal passé. 
        yield return FindLocalPlayer().GetComponent<Player>().Repioche(); 
    }

    /// <summary>
    /// Methode envoyée par l'adversaire pour dire qu'il a fini son choix de cartes. 
    /// </summary>
    public void ChoixAdversaireFini() {
        Destroy(ChoixCartesAdversaire); 
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
    protected void MettreEffetDansLaPile(GameObject ObjetEffet, List<GameObject> Cibles, int numeroEffet, int numeroListEffet = 0, 
        bool ProposerDefairePile=true) {
        Debug.Log("Effet dans la pile");
        GameObject Pile = GameObject.FindGameObjectWithTag("Pile");
        if (Pile == null) {
            // C'est le premier effet de la pile, il faut donc instancier la pile. 
            FindLocalPlayer().GetComponent<Player>().CmdCreerPile();
            StartCoroutine(MettreEffetDansLaPileRoutine(ObjetEffet, Cibles, numeroEffet, numeroListEffet, ProposerDefairePile));
        }
        else {
            // La pile existe déjà, on peut rajouter à la pile. 
            Pile.GetComponent<PileAppelEffet>().AjouterEffetALaPile(ObjetEffet, Cibles, numeroEffet, numeroListEffet, ProposerDefairePile);
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
                                        List<GameObject> Cibles, int numeroEffet, int numeroListEffet = 0, bool ProposerDefairePile=true) {
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

        Pile.GetComponent<PileAppelEffet>().AjouterEffetALaPile(ObjetEffet, Cibles, numeroEffet, numeroListEffet, ProposerDefairePile);
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

    public void ReportBugs() {
        Console.GetComponent<AntinomiaConsole>().addAllBugsToGameSparksDataBase(); 
    }

    /// <summary>
    /// Add a log into the console. 
    /// </summary>
    public void Log(string log) {
        // Debug.Log(log);
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
        // TODO: Que à la main ou toutes les cartes? 
        Transform CartesMain = FindLocalPlayer().transform.Find("MainJoueur").Find("CartesMainJoueur"); 
        for (int i = 0; i < CartesMain.childCount; ++i) {
            GameObject Carte = CartesMain.GetChild(i).gameObject;
            Carte.SendMessage("CartePeutJouer", Phase);
        }
    }

    public void DefairePileOui() {
        defairePile = 1; 
    }

    public void DefairePileNon() {
        defairePile = 2; 
    }

    /// <summary>
    /// Regarder si certaines cartes peuvent jouer un ou plusieurs effet(s).
    /// </summary>
    public void CheckAllEffetsCartes(bool changementDomination = false) {
        StartCoroutine(CheckAllEffetsCartesRoutine(changementDomination)); 
    } 

    /// <summary>
    /// Coroutine verifiant tous les effets 
    /// <see cref="CheckAllEffetsCartes(bool)"/>
    /// </summary>
    /// <param name="changementDomination"></param>
    /// <returns></returns>
    private IEnumerator CheckAllEffetsCartesRoutine(bool changementDomination = false) {
        yield return AttendreEffetsJoueur(); 
        CheckAllEffetsCartes2(changementDomination); 
    }

    /// <summary>
    /// Appelée après <see cref="CheckAllEffetsCartes(bool)"/> vérifie les effets de toutes les cartes. 
    /// </summary>
    /// <param name="changementDomination"></param>
    private void CheckAllEffetsCartes2(bool changementDomination = false) { 
        GameObject[] AllEntites = GameObject.FindGameObjectsWithTag("BoardSanctuaire");

        // On reset d'abord l'eventManager

        ResetEventManager(); 

        for (int i = 0; i < AllEntites.Length; ++i) {
            try {
                if (AllEntites[i].GetComponent<Entite>().isFromLocalPlayer) {
                    AllEntites[i].GetComponent<Entite>().GererEffetsPonctuel(Phase, changementDomination:changementDomination);
                }
            }
            catch (NullReferenceException e) {
                Debug.Log(e);
            }
        }

        GameObject[] AllAssistances = GameObject.FindGameObjectsWithTag("Assistance");
        for (int i = 0; i < AllAssistances.Length; ++i) {
            try {
                if (AllAssistances[i].GetComponent<Assistance>().isFromLocalPlayer &&
                    AllAssistances[i].GetComponent<Assistance>().AssistanceState == Assistance.State.JOUEE) {
                    AllAssistances[i].GetComponent<Assistance>().GererEffetsPonctuel(Phase, changementDomination: changementDomination);
                }
            }
            catch (NullReferenceException e) {
                Debug.Log(e);
            }
        }

        // Une fois qu'on a tout check, on joue tous les effets. 
        JouerEventManager(); 
    }

    /// <summary>
    /// Montrer les carte pour un effet. 
    /// </summary>
    /// <param name="_AllCardsGiven"></param>
    /// <param name="_ObjectAsking"></param>
    /// <param name="stringToDisplay"></param>
    /// <param name="_nombreDeCartesAChoisir"></param>
    public void ChooseCardsForEffect(List<GameObject> _AllCardsGiven, GameObject _ObjectAsking = null, string stringToDisplay = "",
                                    int _nombreDeCartesAChoisir = 1, string EffetToDisplay="") {
        ChooseCardsObject.GetComponent<ChooseCards>().ShowCardsToChoose(_AllCardsGiven, _ObjectAsking, stringToDisplay, _nombreDeCartesAChoisir, 
            deactivateAfter : true, effetToDisplay : EffetToDisplay); 
    }

    /// <summary>
    /// Permet l'interaction de choix des cartes
    /// </summary>
    /// <param name="activate"></param>
    public void ActivateChooseCards(bool activate=true) {
        ChooseCardsObject.SetActive(true);
        Debug.Log("ChooseCards is active"); 
        // On permet au joueur d'interagir. 
        if (activate) {
            ChooseCardsObject.GetComponent<ChooseCards>().PermettreInteraction();
        }
    }

    public void DesactivateChooseCards() {
        ChooseCardsObject.SetActive(false);
    }

    /// <summary>
    /// Montrer des cartes à l'autre joueur
    /// </summary>
    /// <param name="message"></param>
    /// <param name="CardsGiven"></param>
    /// <param name="fermetureManuelle"></param>
    /// <param name="playerEnvoi"></param>
    public void ShowCardsToPlayer(string message, string[] CardsGiven, bool fermetureManuelle=false, int playerEnvoi=1) {
        ShowCardsObject.SetActive(true);
        ShowCardsObject.GetComponent<ShowCards>().ShowCardsToPlayer(message, CardsGiven, fermetureManuelle); 
    }

    /// <summary>
    /// Montrer des cartes à l'autre joueur.
    /// </summary>
    /// <param name="CardsGiven"></param>
    /// <param name="fermetureManuelle"></param>
    /// <param name="playerEnvoi"></param>
    public void ShowCardsToPlayer(string[] CardsGiven, bool fermetureManuelle = false, int playerEnvoi = 1) {
        ShowCardsObject.SetActive(true);
        ShowCardsObject.GetComponent<ShowCards>().ShowCardsToPlayer(CardsGiven, fermetureManuelle, playerEnvoi);
    }

    /// <summary>
    /// Recevoir les cartes choisies de ShowCards
    /// </summary>
    public void CartesChoisies(List<int> AllCardsReturned) {
        _CartesChoisies = new List<GameObject>(); 
        for (int i = 0; i < AllCardsReturned.Count; i++) {
            _CartesChoisies.Add(FindCardWithID(AllCardsReturned[i])); 
        }
        choixCartes = false;
        Debug.Log("FinChoixCartes"); 
    }

    /// <summary>
    /// Lorsque la main est pleine, on attend que le joueur ait choisies les cartes
    /// qu'il veut défausser. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForChoixCartesMainPleine() {
        // On peut faire des actions ici peut-être.
        while (choixCartes) {
            yield return new WaitForSeconds(0.1f); 
        }
        // Maintenant qu'on a récupérer les cartes, il faut les défausser. 
        for (int i = 0; i < _CartesChoisies.Count; i++) {
            _CartesChoisies[i].GetComponent<Carte>().DetruireCarte(); 
        }
        Debug.Log("FinChoixCartes2");
        // Comme cette méthode ne peut avoir été jouée que pour la fin du tour. 
        // On appelle la méthode GoToNextPhase. 
        GoToNextPhase(); 
    }


    /// <summary>
    /// Vérifier que la main du joueur ne comporte pas plus de 8 cartes lors de la fin de son tour. 
    /// </summary>
    /// <param name="Player"></param>
    /// <returns></returns>
    private bool CheckMainPleine(GameObject Player) { 

        if (Player.transform.Find("MainJoueur").Find("CartesMainJoueur").gameObject.GetComponent<MainJoueur>().getNombreCartesMain() > 7) {
            // Le joueur doit avoir 7 cartes ou moins à la fin du tour. 
            List<GameObject> cartesMain =
                Player.transform.Find("MainJoueur").Find("CartesMainJoueur").gameObject.GetComponent<MainJoueur>().getCartesMain();
            ChooseCardsForEffect(
                cartesMain,
                _ObjectAsking : gameObject, 
                stringToDisplay: "Vous avez plus de 7 cartes en main, choisissez celle dont vous voulez vous séparer.", 
                _nombreDeCartesAChoisir : cartesMain.Count - 7
             );
            choixCartes = true;
            StartCoroutine(WaitForChoixCartesMainPleine()); 
            return true; 
        } else {
            return false; 
        }
    }

    public Player.Phases getPhase() {
        return Phase; 
    }

    public int getTour() {
        return Tour; 
    }

    public void ReactivateButtonPhase() {
        NextPhase.SetActive(true);
        Console.GetComponent<AntinomiaConsole>().ReportABug("Probleme de bouton de phase. Reactivé ici."); 
    }

    /// <summary>
    /// Vérifier que le nombre de cartes piochées a été le bon.
    /// </summary>
    /// <param name="nombreCartes"></param>
    public void CheckBonNombreCartes() {
        int nombreCartes = FindLocalPlayer().GetComponent<Player>().GetMainJoueur().gameObject.
            GetComponent<MainJoueur>().getNombreCartesMain(); 
        if (nombreDeCartesChoisies == nombreCartes) {
            Debug.LogWarning("Le nombre de cartes correspond. "); 
        } else {
            Debug.LogWarning("Le nombre de cartes ne correspond pas. "); 
        }
    }

    /// <summary>
    /// Récupérer l'AKA gobal sur le tour, c'est-à-dire sa valeur au début du tour. 
    /// </summary>
    /// <returns>AKATour</returns>
    public int getAKAGlobalTour() {
        return AKATour; 
    }

    /// <summary>
    /// Retourner au menu principal.
    /// </summary>
    public void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu"); 
    }

    /// <summary>
    /// Lors de la fin du jeu on indique au joueur s'il a gagné ou perdu. 
    /// </summary>
    public void EndOfTheGame(bool victory) {
        EndGame.SetActive(true); 
        if (victory) {
            EndGame.transform.Find("Text").GetComponent<Text>().text = "Victoire"; 
        } else {
            EndGame.transform.Find("Text").GetComponent<Text>().text = "Défaite";
        }
    }

    public void AfficherInfoEffetTransmis(string message) {
        StartCoroutine(AfficherInfoEffetTransmisRoutine(message)); 
    }

    private IEnumerator AfficherInfoEffetTransmisRoutine(string message) {
        InfoEffetTransmis.SetActive(true);
        InfoEffetTransmis.GetComponent<Text>().text = message;
        yield return new WaitForSeconds(2f);
        InfoEffetTransmis.SetActive(false); 
    }

    /// <summary>
    /// Ajouter un effet à l'eventManager
    /// </summary>
    /// <param name="ef"></param>
    public void AjouterEffetEventManager(EventEffet ef) {
        eventManager.GetComponent<EventManager>().AjouterEffet(ef); 
    }

    /// <summary>
    /// Reset l'eventManager et en enlever tous les effets. 
    /// </summary>
    public void ResetEventManager() {
        eventManager.GetComponent<EventManager>().Reset(); 
    }

    /// <summary>
    /// Jouer les effets de l'eventManager
    /// </summary>
    public void JouerEventManager() {
        try {
            Debug.Log(eventManager);
            Debug.Log(eventManager.GetComponent<EventManager>());
            eventManager.GetComponent<EventManager>().CreerNouvellePileEvent();
        } catch (NullReferenceException) {
            Debug.Log("Impossible de jouer les effets pour le moment");
        }
    }

    /// <summary>
    /// A appeler à chaque fin d'un effet
    /// </summary>
    public void SetEffetFini() {
        eventManager.GetComponent<EventManager>().EffetFiniOK(); 
    }

    public bool IsAllEffetsFinis() {
        return eventManager.GetComponent<EventManager>().IsAllEffetsFini(); 
    }

    /// <summary>
    /// Changer l'AKA remanent du joueur. 
    /// </summary>
    /// <param name="NewAKARemanent"></param>
    public void SetAKARemanent(int NewAKARemanent) {
        AKARemanent = NewAKARemanent; 
        // Si on appelle cette méthode c'est qu'on est au début du tour, 
        // dans ce cas l'AKA courant est égal à l'AKA rémanent. 
        setPlayerAKAUI(1, NewAKARemanent);
        setPlayerAKAUI(2, NewAKARemanent);
    }

    /// <summary>
    /// Lors de l'appui sur un bouton, on envoie l'information que la 
    /// phase doit être changée. 
    /// </summary>
    /// <param name="newPhase"></param>
    public void ButtonPhaseCallback(int newPhase) {
        // Au debut de la partie, il faut attendre que l'autre joueur ait choisi ses cartes. 
        if (ChoixCartesAdversaire != null && ChoixCartesAdversaire.activeInHierarchy) {
            return; 
        }

        // On verifie que la phase correspond bien. 
        // +7 % 7 car il y a 7 phases.
        if ((newPhase - (int)Phase + 7) % 7 != 1) {
            return; 
        }

        GoToNextPhase(false); 
    }

    /// <summary>
    /// Lorsque le changement de phase a été accepté, 
    /// changer l'UI. 
    /// </summary>
    /// <param name="newPhase"></param>
    private void UpdateButtonsNewPhase(int newPhase) {
        // Tour de notre joueur
        if (Tour == FindLocalPlayerID()) {
            // Sinon on met à jour les boutons. 
            for (int i = 0; i < AllButtonsPhase.transform.childCount; i++) {
                GameObject button = AllButtonsPhase.transform.GetChild(i).gameObject;
                if (i == newPhase) {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorCurrentButton();
                    button.gameObject.GetComponent<Button>().interactable = false;
                }
                else if ((i - 1 + 7) % 7 == newPhase) {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorNextButton();
                    button.gameObject.GetComponent<Button>().interactable = true;
                }
                else {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorDisabledButton();
                    button.gameObject.GetComponent<Button>().interactable = false;
                }
            }
        }
        // Tour de l'autre joueur
        else {
            for (int i = 0; i < AllButtonsPhase.transform.childCount; i++) {
                GameObject button = AllButtonsPhase.transform.GetChild(i).gameObject;
                if (i == newPhase) {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorNoTurnRightPhase();
                    button.gameObject.GetComponent<Button>().interactable = false;
                }
                else {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorNoTurnNoPhase();
                    button.gameObject.GetComponent<Button>().interactable = false;
                }
            }
        }
    }

    /// <summary>
    /// Changer tous les boutons de phase lors d'un changement de phase. 
    /// </summary>
    /// <param name="tour"></param>
    private void ChangeButtonsNewTour(InformationManager.TourJoueur tour) {
        if (tour == InformationManager.TourJoueur.JOUEUR_ADVERSE) {
            foreach (RectTransform button in AllButtonsPhase.GetComponent<RectTransform>()) {
                // On désactive tous les boutons comme ce n'est pas le tour du joueur. 
                // Et on met leur alpha à 0.5
                if (button.name.Equals("Initiation")) {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorNoTurnRightPhase(); 
                    button.gameObject.GetComponent<Button>().interactable = false;
                } else {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorNoTurnNoPhase();
                    button.gameObject.GetComponent<Button>().interactable = false;
                }
            }
        } else {
            Debug.Log(AllButtonsPhase);
            Debug.Log(AllButtonsPhase.GetComponent<RectTransform>()); 
            foreach (RectTransform button in AllButtonsPhase.GetComponent<RectTransform>()) {
                if (button.name.Equals("Initiation")) {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorCurrentButton();
                    button.gameObject.GetComponent<Button>().interactable = false;
                } else if (button.name.Equals("Pioche")) {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorNextButton();
                    button.gameObject.GetComponent<Button>().interactable = true;
                } else {
                    button.gameObject.GetComponent<Button>().colors = ColorBlocksButtons.GetColorDisabledButton(); 
                    button.gameObject.GetComponent<Button>().interactable = false;
                }
            }
        }
    }

}