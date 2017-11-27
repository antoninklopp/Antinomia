using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Hérite de la classe Carte. 
/// Décrit une carte entité.
/// </summary>
public class Entite : Carte, ICarte {
    /*
	 * Classe carte décrivant toutes les spécificités d'une carte.
	 * Ce script ne devrait jamais être directement utilisé dans un script? Car il vaudrait mieux utiliser sort ou créature? 
	 * 
	 * TODO: A continuer. 
	 */
     

    // Sert à récupérer sur les réseau les informations sur la carte. 
    GameObject ObjectInfo;

    // pour le debug, on écrira à un endroit le nom de la capacite
    private GameObject WriteCapacite;

    /// <summary>
    /// L'ascendance de la carte. 
    /// </summary>
    public enum Ascendance {
        ASTRALE,
        NEUTRE,
        MALEFIQUE,
        ELEMENTAIRE
    };

    /// <summary>
    /// L'élement de l'entité.
    /// </summary>
    public enum Element {
        AIR,
        EAU,
        FEU,
        TERRE,
        AUCUN
    };

    /// <summary>
    /// L'état de l'entité. 
    /// </summary>
    public enum State {
        /// <summary>
        /// Carte dans la main
        /// </summary>
        MAIN,
        /// <summary>
        /// Carte posée sur le champ de bataille
        /// </summary>
        CHAMPBATAILLE,
        /// <summary>
        /// Carte morte, dans le cimetière
        /// </summary>
        CIMETIERE,
        /// <summary>
        /// Carte dans le deck
        /// </summary>
        DECK,
        /// <summary>
        /// Carte zoomée
        /// </summary>
        BIGCARD,
        /// <summary>
        /// Carte dans le sanctuaire
        /// </summary>
        SANCTUAIRE,
        /// <summary>
        /// Carte adversaire
        /// </summary>
        ADVERSAIRE,
        /// <summary>
        /// Carte dans le ban.
        /// </summary>
        BAN
    };

    /* ==================================
	 * Définition des éléments de la classe
	 * ====================================
	 */
     
    /// <summary>
    /// STAT initiale de la carte. 
    /// </summary>
    [SyncVar(hook ="ChangeStat")]
    public int STAT;
    public int CoutAKA;
    [SyncVar(hook = "ChangeElement")]
    public Element carteElement;
    public Ascendance carteAscendance;

    /// <summary>
    /// Puissance de l'entité.
    /// </summary>
    private PuissanceEntite puissance; 
    
    // Le nombre d'attaques peut être changé par capacité d'une carte. 
    public int nombre_attaque = 1;
    // On peiut verrouiller la carte pour qu'elle ne puisse pas changer de position
    public bool verrouillee = false;
    public int coutElementaire;

    // Liste des effets Maléfiques et astraux.
    public List<Effet> AllEffetsMalefique = new List<Effet>();
    public List<Effet> AllEffetsAstral = new List<Effet>();

    /// <summary>
    /// Reçu par la bdd Gamesparks "Malefique". C'est le string à décortiquer pour créer la liste d'effets maléfiques. 
    /// </summary>
    public string AllEffetsMalefiqueString = "";

    /// <summary>
    /// Reçu par la bdd Gamesparks "Astral". C'est le string à décortiquer pour créer la liste d'effets astrals. 
    /// </summary>
    public string AllEffetsAstralString = "";

    /// <summary>
    /// Reçu par la bdd Gamesparks "MalefiqueString". C'est le string à afficher "en français" pour que 
    /// l'utilisateur comprenne l'effet maléfique en question. 
    /// </summary>
    public string AllEffetsMalefiqueStringToDisplay = "";

    /// <summary>
    /// Reçu par la bdd Gamesparks "AstralString". C'est le string à afficher "en français" pour que 
    /// l'utilisateur comprenne l'effet maléfique en question. 
    /// </summary>
    public string AllEffetsAstralStringToDisplay = "";

    /// <summary>
    /// Certaines cartes peuvent être fortes face à certains éléments. 
    /// si la liste est vide aucune force, 
    /// si un chiffre est négatif, l'entité est FAIBLE face à l'élement. 
    /// si un chiffre est positif, l'entité est FORTE face à l'élément.
    /// 1 - AIR
    /// 2 - EAU
    /// 3 - FEU
    /// 4 - TERRE
    /// 5 - NEUTRE
    /// </summary>
    public List<int> ForteFaceAElement = new List<int>();

    /// <summary>
    /// Definition de la classe entité
    /// Ce CONSTRUCTEUR N'EST JAMAIS UTILISE
    /// </summary>
    /// <param name="_Name">Nom de l'entité</param>
    /// <param name="_ID">ID de l'entité</param>
    /// <param name="_STAT">STAT de la carte</param>
    /// <param name="_CoutAKA">Cout en AKA de la carte</param>
    /// <param name="_carteElement">élément de la carte</param>
    /// <param name="_carteAscendance">ascendance de la carte</param>
    public Entite(string _Name, int _ID, int _STAT, int _CoutAKA, Element _carteElement,
        Ascendance _carteAscendance) {
        /*
		 * Définition d'une nouvelle Carte.
		*/
        STAT = _STAT;
        IDCardGame = _ID;
        Name = _Name;
        CoutAKA = _CoutAKA;
        carteElement = _carteElement;
        carteAscendance = _carteAscendance;
        // Pour l'instant chaque carte n'a qu'une seule capacité. 
        canGoBig = true;
    }

    /// <summary>
    /// Définition de la classe entité sans paramètre? 
    /// </summary>
    public Entite() {
        /* On peut créer la carte sans aucun attribut
		* Pour que ce soit plus clair lors de la création.
		*/
    }
    
    /// <summary>
    /// State de la carte
    /// </summary>
    public State carteState = State.DECK;

    /// <summary>
    /// Track si l'utilisateur a cliqué sur la carte ou pas. 
    /// </summary>
    public bool clicked = false;

    /// <summary>
    /// Track si l'utiliseur drag la carte ou pas. 
    /// </summary>
    public bool dragging = false;

    private Vector3 PositionBeforeDragging; 

    /// <summary>
    /// Valeur permettant de savoir si un joueur a attaqué. 
    /// -1: Le joueur ne pourra JAMAIS attaquer. 
    /// 0 : Le joueur n'a pas encore attaqué à ce tour mais pourra attaquer. 
    /// 1 : Le joueur a attaqué à ce tour et ne peut donc plus attaquer à ce tour. 
    /// </summary>
    public int hasAttacked;


    // Définition de la récupération de ces éléments.

    public State getState() {
        return carteState;
    }

    /// <summary>
    /// Changer le state d'une carte
    /// </summary>
    /// <param name="newState"></param>
    public virtual void setState(string newState) {
        switch (newState) {
            case "MAIN":
                carteState = State.MAIN;
                break;
            case "BOARD":
                carteState = State.CHAMPBATAILLE;
                // On remet la bonne image pour la carte, dans le cas où la carte était de dos.
                GetComponent<ImageCardBattle>().setImage(shortCode);
                break;
            case "SANCTUAIRE":
                carteState = State.SANCTUAIRE;
                GetComponent<ImageCardBattle>().setImage(shortCode);
                break;
            case "CIMETIERE":
                carteState = State.CIMETIERE;
                break;
            case "DECK":
                carteState = State.DECK;
                break;
            case "BAN":
                carteState = State.BAN;
                break; 
        }
    }

    public bool isClicked() {
        return clicked;
    }

    public void setClicked(bool newClicked) {
        clicked = newClicked;
    }

    // ====================


#pragma warning disable CS0169 // Le champ 'Carte._animator' n'est jamais utilisé
    Animator _animator;
#pragma warning restore CS0169 // Le champ 'Carte._animator' n'est jamais utilisé

    public Sprite dosCarte;

    public override void Start() {
        base.Start();
        //_animator = GetComponent<Animator> ();
        // Attention à ces déclarations!!

        localScaleCard = Mathf.Abs(transform.localScale.x);
        canGoBig = true;
        WriteCapacite = GameObject.Find("Capacite_Effet");
        
        // On ne fait ça que pour la première instantiation. 

        //try{
        if (GetComponent<RectTransform>() == null) {
            // Si l'element est UI pas besoin de setUp la carte
            StartCoroutine(SetUpCard());
        }
        //} catch(NullReferenceException e){
        //	Debug.Log ("PROBLEME" + e.ToString()); 
        //}
    }

    public override void Update() {
        base.Update(); 

        if (clicked) {
            // Si on a cliqué sur la carte. 
            // On ne peut la déplacer que pendant la phase de préparation. (depuis le sanctuaire ou le board)
            // On ne peut l'invoquer que pendant les phases principales. 
            Dragging();
        } 
    }

    public override void checkIfLocalPlayerOnMousEnter() {

        if (!isFromLocalPlayer) {
            carteState = State.ADVERSAIRE;
        }
    }

    public override void OnMouseExit() {
        /*
		 * Lorsque la souris qui "passait" sur la carte la quitte. 
		 * A ce moment là on détruit le zoom qui avait été créé sur la carte. 
		 * 
		 */
        base.OnMouseExit(); 

        //GetComponent<SpriteRenderer>().enabled = true;
        //Destroy(BigCard);
        //// On lance la petite animation de fin. 
        //StartCoroutine(AnimationFinBigCard());
        //// On réactive les objets enfants lors de la destruction de la grosse carte. 
        //for (int i = 0; i < transform.childCount; ++i) {
        //    transform.GetChild(i).gameObject.SetActive(true);
        //}
    }

    /// <summary>
    /// Lors d'un clique sur la carte
    /// </summary>
    public override void OnMouseDown() {
        /* lors d'un clique sur la carte. 
		 * 
		 * On commence le fait de pouvoir la bouger physiquement. 
		 * Si un deuxième clique toujours le board, on la place sur le board. 
		 * 
		 * TODO: Implémenter également un drag and drop. 
		 */
        Debug.Log("DOWN"); 

        base.OnMouseDown(); 

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;

        Vector3 MousePosition = Input.mousePosition;
        MousePosition.z = 15;
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);
        // transform.position = mouseWorldPoint;

        PositionBeforeDragging = mouseWorldPoint; 

#if (UNITY_ANDROID || UNITY_IOS)
        // InformationsSurLaCarte(); 
#else
        CliqueSimpleCarte(false);
#endif
    }

    /// <summary>
    /// Clique simple sur une carte
    /// </summary>
    private void CliqueSimpleCarte(bool drag=false) {
        ResetLocalScale();

        if (carteState == State.CIMETIERE) {
            return;
        }

        // S'il y a un sort en cours. 
        Debug.Log(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SortEnCours);
        if (GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours != null) {
            GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours.SendMessage("RecupererCarteJouerSort", gameObject);
            Debug.Log("Le sort a été joué.");
            return;
        }

        if (GameObject.Find("GameManager").GetComponent<GameManager>().Tour != FindLocalPlayer().GetComponent<Player>().PlayerID) {
            // On ne peut pas interagir avec ses cartes si ce n'est pas son tour!
#if (UNITY_ANDROID || UNITY_IOS)
            clicked = false;
            Main.SendMessage("ReordonnerCarte"); 
#endif
            return;
        }

        if (GameObject.Find("GameManager").GetComponent<GameManager>().choixCartes) {
            if (!isFromLocalPlayer) {
#if (UNITY_ANDROID || UNITY_IOS)
                clicked = false;
                Main.SendMessage("ReordonnerCarte");
#endif
                return;
            }
            // On envoie le gameObject au gameManager, puis on arrête la fonction, pour ne pas faire la suite
            GameObject.Find("GameManager").SendMessage("CarteChoisie", gameObject);
#if (UNITY_ANDROID || UNITY_IOS)
            clicked = false;
            Main.SendMessage("ReordonnerCarte");
#endif
            return;
        }

        Player.Phases currentPhase = GameObject.Find("GameManager").GetComponent<GameManager>().Phase;
        if (currentPhase == Player.Phases.PREPARATION ||
            currentPhase == Player.Phases.PRINCIPALE1 ||
            currentPhase == Player.Phases.PRINCIPALE2) {
            /* Le déplacement ne peut s'effectuer que lors de la phase de préparation. 
			 * 
			 * Phase Principale: le joueur peut INVOQUER des entités. 
			 * Phase préparation : Le joueur peut CHANGER LA POSITION des entités.
			 * 
			 */
            if (!isFromLocalPlayer) {
                return;
            }
            if (!clicked && (((GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PREPARATION &&
                    (carteState == State.CHAMPBATAILLE || carteState == State.SANCTUAIRE)))
                    || ((GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PRINCIPALE1 ||
                    GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PRINCIPALE2) && carteState == State.MAIN))) {
                GetComponent<SpriteRenderer>().enabled = true;
                Destroy(BigCard);
                clicked = true;
                PositionBeforeDragging = new Vector3(transform.position.x, 
                                                     transform.position.y, 
                                                     transform.position.z); 
                canGoBig = false;
            }
            else if (!verrouillee) {
                CheckCardOverlap();
                // Il faut bien regarder que le carte ne soit pas verrouillée!
                Debug.Log("On change la position"); 
                ChangePosition(currentPhase);
                clicked = false;
                canGoBig = true;
            } 
        }
        else if (GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.COMBAT) {
            /*
			 * Lors de la phase de combat. 
			 * 
			 * Lors du premier clique: On sélectionne l'unité qui va attaquer, 
			 * Lors du deuxième clique on sélectionne l'unité à attaquer. 
			 */
            Debug.Log("Ceci est une phase de combats");
            if (hasAttacked == 1) {
                GameObject.Find("GameManager").SendMessage("DisplayMessage", "Cette carte ne peut pas/plus attaquer");
                return;
            }
            else if (hasAttacked == -1) {
                DisplayMessage("Cette carte ne peut pas attaquer");
                return;
            }
            if (isFromLocalPlayer && hasAttacked == 0) {
                // pour pouvoir attaquer il faut que la carte n'ait pas déjà attaqué. 
                if (carteState != State.CHAMPBATAILLE) {
                    GameObject.Find("GameManager").SendMessage("DisplayMessage", "Une carte qui attaque doit etre sur le champ de bataille");
                }
                else {
                    GameObject.Find("GameManager").SendMessage("AttackMyPlayer", gameObject);
                }
            }
            else {
                if (carteState != State.CHAMPBATAILLE) {
                    GameObject.Find("GameManager").SendMessage("DisplayMessage", "Vous ne pouvez attaquer qu'une carte sur le champ de bataille!");
                }
                else {
                    GameObject.Find("GameManager").SendMessage("AttackOtherPlayer", gameObject);
                }
            }
            return;
        } else {
#if (UNITY_ANDROID || UNITY_IOS)
            Main.SendMessage("ReordonnerCarte");
            AntinomiaLog("Pas la bonne phase");
            clicked = false;
            // dragging = false;
#endif
        }
        // dragging = false;
    }

    /// <summary>
    /// Lors du drag de la carte
    /// </summary>
    public override void OnMouseDrag() {

        Vector3 MousePosition = Input.mousePosition;
        MousePosition.z = 15;
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);

#if (UNITY_ANDROID || UNITY_IOS)
        if (!dragging && Vector2.Distance(mouseWorldPoint, PositionBeforeDragging) > 0.5f) {
            clicked = false; 
            CliqueSimpleCarte(true);
            dragging = true; 
        } 
#endif
    }

    /// <summary>
    /// Lorsqu'on relache le clic
    /// </summary>
    public void OnMouseUp() {
        /*
		 * Lorsque la carte est relachée par l'utilisateur. 
		 * 
		 * TODO: Implémenter cette phase lors du drap and drop. 
		 */
        Vector3 MousePosition = Input.mousePosition;
        MousePosition.z = 15;
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);

         if (Vector2.Distance(PositionBeforeDragging, mouseWorldPoint) > 0.5f) {
            CliqueSimpleCarte(true);
            PositionBeforeDragging = transform.position; 
         } else {
#if (UNITY_ANDROID || UNITY_IOS)
            InformationsSurLaCarte();
            clicked = false;
            Main.SendMessage("ReordonnerCarte"); 
#endif
         }
         // Sinon on n'appelle pas la fonction
         dragging = false;
    }

    /// <summary>
    /// Création de la carte zoomée. 
    /// </summary>
    /// <param name="messageToDisplay"></param>
    public override void CreateBigCard(string messageToDisplay="") {

        base.CreateBigCard(
            Name + "\n" +
            "STAT" + STAT.ToString() + "\n" +
            "Nature" + carteElement.ToString());

        BigCard.GetComponent<Entite>().carteState = State.BIGCARD;

    }

    public override void DisplayInfoCarteGameManager(string shortCode = "", string messageToDisplay = "") {
        base.DisplayInfoCarteGameManager(this.shortCode,
            GetInfoCarte());
    }

    /// <summary>
    /// Changer la position d'une carte. 
    /// </summary>
    /// <param name="currentPhase">Phase en cours. </param>
    protected virtual void ChangePosition(Player.Phases currentPhase) {
        /*
		 * Changer la position de la carte lors d'un re - clic. 
		 * 
		 * Change position:
		 * Comprend les invocations des phases principales ET
		 * les changements de position de la phase de préparation
		 */


        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;

        Debug.Log("POSITION");
        Debug.Log(carteState);
        float yMinChampBataille = ChampBataille.transform.position.y - ChampBataille.GetComponent<BoxCollider2D>().size.y / 2;
        float yMaxChampBataille = ChampBataille.transform.position.y + ChampBataille.GetComponent<BoxCollider2D>().size.y / 2;

        float yMinSanctuaire = Sanctuaire.transform.position.y - ChampBataille.GetComponent<BoxCollider2D>().size.y / 2;
        float yMaxSanctuaire = Sanctuaire.transform.position.y + ChampBataille.GetComponent<BoxCollider2D>().size.y / 2;

        switch (carteState) {
            case State.MAIN:
                if (transform.position.y < yMaxChampBataille && transform.position.y > yMinChampBataille) {
                    // Si la carte est déposée sur le board, depuis la main.
                    // On vérifie qu'il y a moins de 5 cartes sur le board.
                    MoveToChampBataille(currentPhase);
                    // ProposerMettreJeuEnPause(); 
                } else if (transform.position.y < yMaxSanctuaire && transform.position.y > yMinSanctuaire) {
                    // Si la carte est déposée sur le board. 
                    // On vérifie qu'il y a moins de 2 cartes sur le sanctuaire.
                    MoveToSanctuaire(currentPhase);
                    // ProposerMettreJeuEnPause(); 
                }
                // Si la carte n'est pas déposée, elle retrouve sa position grâce à la main qui est répordonnée.
                Main.SendMessage("ReordonnerCarte");
                break;
            case State.SANCTUAIRE:
                if (transform.position.y < yMaxChampBataille && transform.position.y > yMinChampBataille) {
                    // Si la carte est déposée sur le board. 
                    // On vérifie qu'il y a moins de 5 cartes sur le board.
                    MoveToChampBataille(currentPhase);
                }
                Sanctuaire.SendMessage("ReordonnerCarte");
                break;
            case State.CHAMPBATAILLE:
                // Si on veut replacer la carte sur le sanctuaire. 
                // il est bien sûr impossible de la replacer dans la main. 
                if (transform.position.y < yMaxSanctuaire && transform.position.y > yMinSanctuaire) {
                    MoveToSanctuaire(currentPhase);
                }
                ChampBataille.SendMessage("CmdReordonnerCarte");
                break;
        }
        
        CmdChangePosition(carteState);
    }

    public void MoveToSanctuaire(Player.Phases currentPhase=Player.Phases.INITIATION, bool defairePile=false) {
        // Lorsque qu'une carte est mise sur le sanctuaire
        // Le state est changé par l'objet sanctuaire
        if (Sanctuaire.GetComponent<Sanctuaire>().getNumberCardsSanctuaire() < 2) {
            // On ne peut invoquer d'entités que lors des phases principales
            // Lorsqu'on défait la pile on ne s'occupe pas des phases parce qu'elles ont été traitées lors de la création
            // de "l'effet" dans la pile. 
            if (carteState == State.MAIN && (currentPhase == Player.Phases.PRINCIPALE1 || currentPhase == Player.Phases.PRINCIPALE2 || defairePile)) {
                // Le joueur ne peut pas invoquer plus d'une carte avec un coût élémentaire par tour. 
                // On peut aussi payer des niveaux de couts différents selon l'élément de l'entité. 
                if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().nombreInvocationsSanctuaire >= 1) {
                    return;
                } else {
                    Debug.Log("INVOCATION ELEMENTAIRE");
                    if (carteAscendance == Ascendance.ELEMENTAIRE) {
                        int x = coutElementaire; 
                        switch (carteElement) {
                            case Element.FEU:
                                // Enlever 10x points de vie au joueur. 
                                if (FindLocalPlayer().GetComponent<Player>().PlayerPV < 40 * x) {
                                    // Si le joueur ne peut pas payer le cout
                                    GameObject.Find("GameManager").SendMessage("DisplayMessage", "Vous ne pouvez pas payer ce cout " + (10 * x).ToString() + " points de vie");
                                    return;
                                } else {
                                    if (!defairePile) {
                                        MettreEffetDansLaPile(new List<GameObject>(), -3);
                                        return; 
                                    }
                                    else {
                                        GameObject.Find("GameManager").SendMessage("DisplayMessage", "Vous perdez " + (40 * x).ToString() + " points de vie");
                                        FindLocalPlayer().GetComponent<Player>().AttackPlayer(40 * x);
                                    }
                                }
                                break;
                            case Element.EAU:
                                if (!defairePile) {
                                    MettreEffetDansLaPile(new List<GameObject>(), -3);
                                    return; 
                                }
                                else {
                                    // Donner 10x points de vie à l'adversaire. 
                                    GameObject.Find("GameManager").SendMessage("DisplayMessage", "Votre adversaire gagne " + (40 * x).ToString() + "points de vie");
                                    // Comme le HealPlayer ne marche que sur le joueur local, sinon pas d'autorité
                                    // On fait une attaque à l'envers.
                                    FindNotLocalPlayer().GetComponent<Player>().AttackPlayer(-40 * x);
                                }
                                break;
                            case Element.AIR:
                                if (!defairePile) {
                                    MettreEffetDansLaPile(new List<GameObject>(), -3);
                                    return;
                                }
                                else {
                                    gameObject.tag = "BoardSanctuaire";
                                    GameObject.Find("GameManager").SendMessage("InvocationElementaireAir", 2*x);
                                }
                                break;
                            case Element.TERRE:
                                if (!defairePile) {
                                    MettreEffetDansLaPile(new List<GameObject>(), -3);
                                    return;
                                }
                                else {
                                    // Detruire les x premières cartes du deck.
                                    GameObject.Find("GameManager").SendMessage("DisplayMessage", x.ToString() + "cartes de votre deck ont été détruites");
                                    FindLocalPlayer().SendMessage("DetruireCartesDessusDeck", x);
                                }
                                break;
                            default:
                                throw new Exception("La carte élémentaire devrait avoir un élément assigné"); 
                        }
                    } else {
                        // Si la carte n'est pas élémentaire, on ne peut l'invoquer de cette façon. 
                        return;
                    }
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().nombreInvocationsSanctuaire = 1;
                }
            } else if (currentPhase == Player.Phases.PRINCIPALE1 || currentPhase == Player.Phases.PRINCIPALE2) {
                // Si la phase en cours est la phase principale et la carte ne vient pas de la main, on fait return, 
                // Car on ne peut pas bouger de cartes pendant les phases principales
                GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "On ne peut pas bouger de cartes pendant la phase principale");
                return;
            } else if (currentPhase == Player.Phases.PREPARATION && carteState == State.MAIN) {
                GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "On ne peut pas invoquer de cartes pendant la phase de preparation");
                return;
            }

            Main.SendMessage("DeleteCard", gameObject);
            Sanctuaire.SendMessage("CmdCarteDeposee", gameObject);
            gameObject.tag = "BoardSanctuaire";

            CmdChangePosition(State.SANCTUAIRE);

            // Il faut aussi vérifier si les autres cartes ont un effet à jouer
            // GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().CheckAllEffetsCartes(); 
        }
        else {
            // On envoie un message à l'utilisateur disant qu'il y a plus de 2 cartes sur le sanctuaire.
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "Deja 2 cartes dans le sanctuaire");
        }
    }

    public void MoveToChampBataille(Player.Phases currentPhase = Player.Phases.INITIATION, bool defairePile=false) {
        // Lorsqu'une carte est mise sur le board. 
        // Le state est changé par l'objet board, à changer ici? 
        if (ChampBataille.GetComponent<CartesBoard>().getNumberCardsChampBataille() < 5) {
            // On ne peut invoquer des entités que lors des phases principales
            // Lorsqu'on défait la pile on ne s'occupe pas des phases parce qu'elles ont été traitées lors de la création
            // de "l'effet" dans la pile. 
            if (carteState == State.MAIN && (currentPhase == Player.Phases.PRINCIPALE1 || currentPhase == Player.Phases.PRINCIPALE2) || defairePile) {
                if (carteAscendance == Ascendance.MALEFIQUE && cardAstraleOnChampBatailleOrSanctuaire()) {
                    GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "Il y a une carte astrale sur le board");
                } else if (carteAscendance == Ascendance.ASTRALE && cardMalefiqueOnChampBatailleOrSanctuaire()) {
                    GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "Il y a une carte maléfique sur le board");
                } else if (FindLocalPlayer().GetComponent<Player>().PlayerAKA < CoutAKA) {
                    // Si le joueur n'a pas assez d'AKA pour mettre la carte sur le board
                    GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "AKA demandé: " + CoutAKA.ToString());
                    return;
                } else {
                    // La carte est instanciée. 
                    // Lorsqu'on ne défait pas la pile
                    if (!defairePile) {
                        MettreEffetDansLaPile(new List<GameObject>(), -2);
                        return; 
                    }
                    // Lorsqu'on défait la pile.
                    else {
                        GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "AKA utilisé: " + CoutAKA.ToString());
                        FindLocalPlayer().SendMessage("subtractAKA", CoutAKA);

                        if ((carteAscendance == Ascendance.MALEFIQUE) || (carteAscendance == Ascendance.ASTRALE)) {
                            CmdChangeAscendanceTerrain(carteAscendance);
                        }
                    }
                }
                // On regarde les effets de la carte. 
                Debug.Log("<color=purple>Effets de la carte</color>"); 
                GererEffets(AllEffets, debut:true);
            } else if (currentPhase == Player.Phases.PRINCIPALE1 || currentPhase == Player.Phases.PRINCIPALE2) {
                // Si la phase en cours est la phase principale et la carte ne vient pas de la main, on fait return, 
                // Car on ne peut pas bouger de cartes pendant les phases principales
                GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "On ne peut pas bouger de cartes pendant la phase principale");
                return;
            } else if (currentPhase == Player.Phases.PREPARATION && carteState == State.MAIN) {
                GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "On ne peut pas invoquer de cartes pendant la phase de preparation");
                return;
            }
            Main.SendMessage("DeleteCard", gameObject);
            ChampBataille.SendMessage("CmdCarteDeposee", gameObject);
            gameObject.tag = "BoardSanctuaire";

            CmdChangePosition(State.CHAMPBATAILLE);
        } else {
            // TODO: Envoyer un message à l'utilisateur disant qu'il y a plus de 5 cartes sur le board. 
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "Deja 5 cartes sur le board");
        }
    }

    [Command]
    void CmdChangePosition(State newCarteState) {

        /* 
		 * TODO: NE pas faire ça!!
		 * Problème lors de l'instanciation de la carte, qui ne permet pas de récuérer toutes les références!
		 * Peut-être une coroutine qui le fait 0.5 sec après le OnStartAuthority? 
		 * 
		 * Solution moyenne, temporaire
		 */

        //ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        //Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        //Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;
        //Cimetiere = transform.parent.parent.parent.Find("Cimetiere").Find("CartesCimetiere").gameObject;

        //carteState = newCarteState;
        //Debug.Log(carteState);
        //// il faut maintenant mettre à jour les position. 
        //switch (carteState) {
        //    case State.CHAMPBATAILLE:
        //        // Delete Card de la main ne sert à rien. 
        //        Main.SendMessage("DeleteCard", gameObject);
        //        ChampBataille.SendMessage("CmdCarteDeposee", gameObject);
        //        Main.SendMessage("ReordonnerCarte");
        //        gameObject.tag = "BoardSanctuaire";
        //        break;
        //    case State.SANCTUAIRE:
        //        Main.SendMessage("DeleteCard", gameObject);
        //        Sanctuaire.SendMessage("CmdCarteDeposee", gameObject);
        //        Sanctuaire.SendMessage("ReordonnerCarte");
        //        gameObject.tag = "BoardSanctuaire";
        //        break;
        //    case State.CIMETIERE:
        //        Cimetiere.SendMessage("CmdCarteDeposee", gameObject);
        //        Sanctuaire.SendMessage("ReordonnerCarte");
        //        ChampBataille.SendMessage("CmdReordonnerCarte");
        //        gameObject.tag = "Cimetiere";
        //        break;
        //}

        RpcChangePosition(newCarteState); 

    }

    [ClientRpc]
    void RpcChangePosition(State newCarteState) {

        /* 
		 * TODO: NE pas faire ça!!
		 * Problème lors de l'instanciation de la carte, qui ne permet pas de récuérer toutes les références!
		 * Peut-être une coroutine qui le fait 0.5 sec après le OnStartAuthority? 
		 * 
		 * Solution moyenne, temporaire
		 */

        AntinomiaLog("On change la position de la carte vers " + newCarteState.ToString()); 

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;
        Cimetiere = transform.parent.parent.parent.Find("Cimetiere").Find("CartesCimetiere").gameObject;

        carteState = newCarteState;
        // il faut maintenant mettre à jour les position. 
        switch (carteState) {
            case State.CHAMPBATAILLE:
                Main.SendMessage("DeleteCard", gameObject);
                ChampBataille.SendMessage("CmdCarteDeposee", gameObject);
                Main.SendMessage("ReordonnerCarte");
                gameObject.tag = "BoardSanctuaire";
                // On offre au joueur adverse la possibilité de réagir. 
                // StartCoroutine(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ProposeToPauseGame()); 
                break;
            case State.SANCTUAIRE:
                Main.SendMessage("DeleteCard", gameObject);
                Sanctuaire.SendMessage("CmdCarteDeposee", gameObject);
                Sanctuaire.SendMessage("ReordonnerCarte");
                gameObject.tag = "BoardSanctuaire";

                // On offre au joueur adverse la possibilité de réagir. 
                // StartCoroutine(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ProposeToPauseGame());
                break;
            case State.CIMETIERE:
                Cimetiere.SendMessage("CmdCarteDeposee", gameObject);
                Sanctuaire.SendMessage("ReordonnerCarte");
                ChampBataille.SendMessage("CmdReordonnerCarte");
                gameObject.tag = "Cimetiere";
                break;
        }
    }

    void CheckCardOverlap() {
        /*
		 * Regarder quelle carte en touche une autre, lors d'une attaque. 
		 */

    }

    /// <summary>
    /// Set up de la carte. 
    /// Lors du spawn sur le client, on envoie les informations à l'objet. 
    /// </summary>
    /// <returns>None</returns>
    public IEnumerator SetUpCard() {
        /*
		 * On attend de savoir si le state de la carte est Main ou pas. 
		 * Latence réseau. 
		 */

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        if (((Players[0].GetComponent<Player>().isLocalPlayer && Players[0].GetComponent<Player>().isServer) ||
            (Players[1].GetComponent<Player>().isLocalPlayer && Players[1].GetComponent<Player>().isServer)) && netId.Value != 0) {
            // Dans le cas d'une instantiation d'une carte sur le réseau.
            RpcsetoID1(IDCardGame, oID, Name, shortCode, carteAscendance, carteElement, STAT, CoutAKA, coutElementaire, 
                AllEffetsString, AllEffetsStringToDisplay, AllEffetsMalefiqueString, AllEffetsMalefiqueStringToDisplay, 
                AllEffetsAstralString, AllEffetsAstralStringToDisplay);
            // Inutile normalement.
            // RpcChangeParent (); 
        } // else {
        //    yield break; 
        //}

        yield return new WaitForSeconds(0.1f);

        if (carteState != State.MAIN) {
            Debug.Log("<color=purple>Probleme pas de state Main, et pas sorti </color>"); 
        }

        Debug.Log(netId.ToString() + hasAuthority.ToString());

        // l'ID de la carte a la même valeur que l'ID réseau donnée par unity.
        IDCardGame = (int)netId.Value;

        Debug.Log("<color=red>Cette carte " + Name + "a autorité" + hasAuthority.ToString() + "</color>"); 
        if (hasAuthority) {
            // C'est à dire que la carte vient du local Player. 
            for (int i = 0; i < Players.Length; ++i) {
                if (Players[i].GetComponent<Player>().isLocalPlayer) {
                    gameObject.transform.SetParent(Players[i].transform.Find("MainJoueur").Find("CartesMainJoueur"));
                    break;
                }
            }
        } else {
            for (int i = 0; i < Players.Length; ++i) {
                if (!Players[i].GetComponent<Player>().isLocalPlayer) {
                    gameObject.transform.SetParent(Players[i].transform.Find("MainJoueur").Find("CartesMainJoueur"));
                    break;
                }
            }
        }

        transform.parent.gameObject.SendMessage("ReordonnerCarte");


        // Player est le parent au deuxième degré. 
        // Le joueur ne peut grossir que ses propres cartes! 
        // Et ne peut pas toucher aux cartes de ses adversaires. 
        if (transform.parent.parent.localScale.y == -1) {
            transform.localScale = new Vector3(1, -1, 1);
        }

        StartCoroutine(setImageCarte());

        isFromLocalPlayer = transform.parent.parent.parent.gameObject.GetComponent<Player>().isLocalPlayer;

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;
        Cimetiere = transform.parent.parent.parent.Find("Cimetiere").Find("CartesCimetiere").gameObject;

        puissance = new PuissanceEntite(STAT);
    }

    [ClientRpc]
    public void RpcChangeParent() {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < Players.Length; ++i) {
            if (Players[i].GetComponent<Player>().isLocalPlayer && Players[i].GetComponent<Player>().isServer) {
                return;
            }
            if (!Players[i].GetComponent<Player>().isLocalPlayer) {
                gameObject.transform.SetParent(Players[i].transform.Find("MainJoueur").Find("CartesMainJoueur"));
                break;
            }
        }

        transform.parent.gameObject.SendMessage("ReordonnerCarte");

        Debug.Log("bliblablou");
        Debug.Log(shortCode);
        GetComponent<ImageCardBattle>().setImage(shortCode);

    }

    [ClientRpc]
    void RpcsetoID1(int _ID, string _oID, string _Name, string _shortCode, Entite.Ascendance _ascendance, Entite.Element _element, int _STAT,
                                    int _coutAKA, int _coutElementaire, string _AllEffetsString, string _AllEffetsStringToDisplay, 
                                    string _AllEffetsMalefiques, string _AllEffetsMalefiquesToDisplay, 
                                    string _AllEffetsAstrals, string _AllEffetsAstralToDisplay) {

        // On peut peut-être tout faire passer par les arguments. 
        // IDCardGame = _ID;
        oID = _oID;
        Debug.Log(_oID);
        Name = _Name;
        shortCode = _shortCode;
        carteState = State.MAIN;
        carteElement = _element;
        carteAscendance = _ascendance;
        STAT = _STAT;
        CoutAKA = _coutAKA;
        coutElementaire = _coutElementaire;
        AllEffetsAstralString = _AllEffetsAstrals;
        AllEffetsAstralStringToDisplay = _AllEffetsAstralToDisplay;
        AllEffetsString = _AllEffetsString;
        AllEffetsStringToDisplay = _AllEffetsStringToDisplay;
        AllEffetsMalefiqueString = _AllEffetsMalefiques;
        AllEffetsMalefiqueStringToDisplay = _AllEffetsMalefiquesToDisplay;
        stringToEffetList(_AllEffetsString);
        stringToEffetAstral(_AllEffetsAstrals);
        stringToEffetMalefique(_AllEffetsMalefiques); 
    }


    [ClientRpc]
    void RpcsetoID2(string ID) {
        ObjectInfo = ObjectInfo.GetComponent<GetPlayerInfoGameSparks>().GetCardoID();
        Debug.Log(ObjectInfo.GetComponent<Entite>().Name);
    }

    /// <summary>
    /// Detruire une carte. 
    /// On change son state, pour l'envoyer au cimetière. 
    /// </summary>
    public override void DetruireCarte() {
        /*
		 * On change juste la carte de State pour pouvoir laisser au joueur la possibilité de la récupérer ensuite. 
         * Et pouvoir l'afficher dans le cimetiere. 
		 */

        if (GetComponent<EntiteAssocieeAssistance>() != null) {
            // Si l'entité meurt et qu'elle est liée à une assistance. 
            GetComponent<EntiteAssocieeAssistance>().EntiteDetruite(); 
        }

        Cimetiere = transform.parent.parent.parent.Find("Cimetiere").Find("CartesCimetiere").gameObject;
        AntinomiaLog(Cimetiere);
        AntinomiaLog("Carte detruite" + IDCardGame.ToString()); 

        setState("CIMETIERE");
        Cimetiere.SendMessage("CmdCarteDeposee", gameObject);
        Sanctuaire.SendMessage("ReordonnerCarte");
        ChampBataille.SendMessage("CmdReordonnerCarte");
        Main.SendMessage("ReordonnerCarte");

        AntinomiaLog(transform.parent.parent.parent.gameObject);
        if (transform.parent.parent.parent.gameObject.GetComponent<Player>().isLocalPlayer) {
            /*
			 * Si on est pas dans le cas d'un player local, on ne peut pas envoyer de command. 
			 */
            CmdChangePosition(carteState);
        } else {
            // Si on est pas sur le player local. 
            Debug.Log("N'est pas le local Player");
            GameObject LocalPlayer = FindLocalPlayer();
            LocalPlayer.SendMessage("DetruireCarte", IDCardGame);
        }

        if ((carteAscendance == Ascendance.MALEFIQUE) || (carteAscendance == Ascendance.ASTRALE)) {
            CmdChangeAscendanceTerrain(Ascendance.NEUTRE); 
        }

        // GererEffetsMort(); 
    }

    public void printCarte() {
        /* 
		 * Afficher les infos de la carte dans le debugger. 
		 */

        Debug.Log("--------------- INFO CARTE -----------------");
        Debug.Log("Name : " + Name);
        Debug.Log("STAT : " + STAT.ToString());
        Debug.Log("ID : " + IDCardGame.ToString());
        Debug.Log("AKA : " + CoutAKA.ToString());
        Debug.Log("carteElement : " + carteElement.ToString());
        Debug.Log("carteAscendance : " + carteAscendance.ToString());

        Debug.Log(" ---------- FIN INFO CARTE ------------");
    }


    // TOUT CE QUI CONCERNE LA GESTION DES CAPACITES : COMPLIQUE A DEPLACER DANS LA CLASSE CAPACITE (a cause des messages à envoyer aux objets). 

    public void VerrouillerCarte(bool verrouiller) {
        /*
		 * Verrouiller ou déverrouiller une carte. 
		 */
        verrouillee = verrouiller;
    }

    IEnumerator CartesDegats(int nombreCartes, int nombreDeDegats) {
        /*
		 * infliger des degats à certaines cartes
		 */
        // Attention il est possible que cette recherche ne fonctionne pas sur des objets spawn sur le réseau. 
        GameObject GameManagerObject = GameObject.Find("GameManager");
        GameManagerObject.GetComponent<GameManager>().DebutChoixCartes(nombreCartes);
        yield return GameManagerObject.GetComponent<GameManager>().WaitForChoixCartes(nombreCartes);
        // Normalement on devrait avoir le résultat à ce moment la . 
        List<GameObject> CartesChoisies = GameManagerObject.GetComponent<GameManager>()._CartesChoisies;
        for (int i = 0; i < nombreCartes; ++i) {
            CartesChoisies[i].SendMessage("TakeDamage", (-1) * nombreDeDegats);
        }
        GameManagerObject.GetComponent<GameManager>().FinChoixCartes();
    }

    IEnumerator Defausser(int nombreCartes) {
        /*
		 * Defausser un certain nombre de cartes. 
		 * faire défausser une ou des cartes d'une main
		 * 
		 */

        yield return null;

    }

    /// <summary>
    /// A chaque début de tour, toutes les cartes peuvent de nouveau attaquer
    /// </summary>
    void resetHasAttacked() {
        // Si hasAttacked == -1, alors la carte ne peut pas attaquer, jamais. 
        if (hasAttacked == -1) {
            return;
        }
        else {
            hasAttacked = 0;
        }
    }

    /// <summary>
    /// Dire que la carte a attaque
    /// </summary>
    /// <param name="newHasAttacked">Par defaut, on dit que la carte a attaqué, mais on peut imaginer,
    /// remettre à 0 l'attaque de cette carte</param>
    void setHasAttacked(bool newHasAttacked = true) {
        hasAttacked = 1;
    }

    GameObject[] GetAllCardsPlayed() {
        return GameObject.FindGameObjectsWithTag("BoardSanctuaire");
    }

    bool cardAstraleOnChampBatailleOrSanctuaire() {
        /*
		 * retourne true s'il y a une carte astrale sur le sanctuaire ou le champ de bataille
		 * retourne false s'il y n'y en a pas. 
		 */
        GameObject[] AllCardsPlayed = GetAllCardsPlayed();
        for (int i = 0; i < AllCardsPlayed.Length; ++i) {
            if (AllCardsPlayed[i].GetComponent<Entite>().carteAscendance == Ascendance.ASTRALE) {
                return true;
            }
        }
        return false;
    }

    bool cardMalefiqueOnChampBatailleOrSanctuaire() {
        /*
		 * retourne true s'il y a une carte astrale sur le sanctuaire ou le champ de bataille
		 * retourne false s'il y n'y en a pas. 
		 */
        GameObject[] AllCardsPlayed = GetAllCardsPlayed();
        for (int i = 0; i < AllCardsPlayed.Length; ++i) {
            if (AllCardsPlayed[i].GetComponent<Entite>().carteAscendance == Ascendance.MALEFIQUE) {
                return true;
            }
        }
        return false;
    }

    void ChangeNatureEntite(Element _newElement, int nombreTours = 1) {
        /*
         * Par un sort ou un effet, 
         * on peut changer la nature d'une carte
         * pendant un nombre de tours plus ou moins long. 
         * 
         * TODO : Mettre un marqueur sur une carte qui est "sous l'emprise" d'un effet. 
         */

        // Il faut envoyer cette information sur le réseau. 
        CmdChangeElement(_newElement, true); 
        // Il faut ensuite commencer le compte à rebours. 
    }

    /// <summary>
    /// Changer l'élément d'une carte
    /// </summary>
    /// <param name="_newElement">Le nouvel élément que l'on associe à la carte.</param>
    /// <param name="change">true, si on change de la carte de base, false si on reset l'élément.</param>
    [Command]
    public void CmdChangeElement(Element _newElement, bool change) {
        /*
         * Envoi de l'information sur le serveur lorsque l'élément de la carte change. 
         * le booléen change est true lorsque la carte change 
         * il est false si l'on revient à la carte de base. 
         */ 
        carteElement = _newElement;
        // On indique visuellement que la carte a changé.
        if (change) {
            RpcChangeFromCardBase();
        } else {
            RpcResetToCardBase(); 
        }
    }

    void ChangeElement(Element newElement) {
        /*
         * Méthode appelée par la syncvar
         * lors d'un changement sur la valeur de Element carteElement
         * hook
         */
        carteElement = newElement;
        // TODO: Montrer au joueur que la carte est changée. 
    }

    /// <summary>
    /// Remettre l'élément, la nature d'une carte, à son état naturel.
    /// </summary>
    public void ResetCarteElement() {
        /*
         * Après la fin d'un effet qui change l'element de la carte, 
         * il faut remettre le "bon" element. 
         */
        //FindLocalPlayer().GetComponent<GetPlayerInfoGameSparks>().WaitForCardoID
        new GameSparks.Api.Requests.LogEventRequest()
           .SetEventKey("getElementFromCardByID")
           .SetEventAttribute("ID", oID)
           .Send((response) => {
               if (!response.HasErrors) {
                   carteElement = GetPlayerInfoGameSparks.stringToElement(response.ScriptData.GetString("Element")); 
               } else {
                   throw new Exception("Impossible de reset l'element de la carte. "); 
               }

           });
        CmdChangeElement(carteElement, false); 
    }

    /// <summary>
    /// Changer la STAT de la carte. 
    /// FONCTION JAMAIS APPELEE
    /// </summary>
    /// <param name="newStat"></param>
    void ChangeStat(int newStat) {
        /*
         * Fonction appelée après un changement de stat sur le réseau
         * (hook)
         */
        STAT = newStat;
    }

    /// <summary>
    /// Changer la stat d'une carte
    /// FONCTION JAMAIS APPELEE
    /// </summary>
    /// <param name="newStat">Nouvelle stat</param>
    [Command]
    public void CmdChangeStat(int newStat) {
        /*
         * Fonction command
         * Changer la stat d'une carte sur le réseau. 
         */
        STAT = newStat;
    }

    /// <summary>
    /// Changer la stat d'une carte, lui ajouter de la valeur
    /// </summary>
    /// <param name="_statToAdd">Valeur à ajouter à la stat. </param>
    [Command]
    public void CmdAddStat(int _statToAdd) {
        /*
         *  Même fonction que la précédente mais au lieu de changer la stat directement
         *  on ajoute simplement le nombre _statToAdd à la stat courante. 
         */

        RpcAddStat(_statToAdd, 0); 
        // STAT += _statToAdd;
        // On regarde si la carte revient à son état initial (on remet la carte en blanc)
        // Ou si elle change (on la met en vert). 

        // REDEFINITION DE LA METHODE APRES CHANGEMENT DE LA NOTION DE PUISSANCE 27/11/2017
        //new GameSparks.Api.Requests.LogEventRequest()
        //   .SetEventKey("getSTATFromCardByID")
        //   .SetEventAttribute("ID", oID)
        //   .Send((response) => {
        //       if (!response.HasErrors) {
        //           // La stat récupérée
        //           int getSTAT = response.ScriptData.GetInt("STAT").Value;
        //           if (getSTAT == STAT) {
        //               // Dans ce cas on a remis la carte dans son état de base
        //               RpcResetToCardBase(); 
        //           } else {
        //               RpcChangeFromCardBase();
        //           }
        //       }
        //       else { 
        //           // Dans le cas où ça bug on ne fait rien pour l'instant.
        //           // TODO: A implémenter. 
        //           throw new Exception("Impossible de reset l'element de la carte. ");
        //       }

        //   });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_statToAdd"></param>
    /// <param name="IDCardGameChanged">IDCardGame de la carte dont provient l'effet qui a créé un changement de puissance.</param>
    [Command]
    public void CmdAddStat(int _statToAdd, int IDCardGameChanged) {
        RpcAddStat(_statToAdd, IDCardGameChanged); 
    }


    [ClientRpc]
    public void RpcAddStat(int _statToAdd, int IDCardGameChanged) {
        puissance.AjouterChangementPuissance(new ChangementPuissance(ChangementPuissance.Type.MULTIPLICATION, 
            _statToAdd, IDCardGameChanged)); 
        if (puissance.RecupererPuissanceEntite() == STAT) {
            RpcResetToCardBase(); 
        } else {
            RpcChangeFromCardBase(); 
        }
    }


    /// <summary>
    /// Changer la valeur de la stat, la multiplier. 
    /// </summary>
    /// <param name="_multiplicateur">Valeur par laquelle multiplier la stat.</param>
    [Command]
    public void CmdMultiplierStat(int _multiplicateur) {
        RpcMultiplierStat(_multiplicateur, 0);
        // STAT *= _multiplicateur;
        // Debug.Log("Multiplication des valeurs de la carte1");
        // RpcChangeFromCardBase();
        // Debug.Log("Multiplication des valeurs de la carte2");
    }

    [Command]
    public void CmdMultiplierStat(int _multiplicateur, int IDCardGameChanged) {
        RpcMultiplierStat(_multiplicateur, 0);
    }

    [ClientRpc]
    public void RpcMultiplierStat(int _multiplicateur, int IDCardGameChanged) {
        puissance.AjouterChangementPuissance(new ChangementPuissance(ChangementPuissance.Type.MULTIPLICATION, _multiplicateur,
            IDCardGameChanged));
        if (puissance.RecupererPuissanceEntite() == STAT) {
            RpcResetToCardBase(); 
        } else {
            RpcChangeFromCardBase(); 
        }
    }

    [ClientRpc]
    void RpcChangeFromCardBase() {
        /*
         * Lorsqu'une carte change de la carte de base à cause d'un effet,
         * on met une sorte de filtre vert sur la carte pour l'indiquer. 
         */
        Debug.Log("Changement de couleur de la carte"); 
        GetComponent<SpriteRenderer>().color = Color.green; 
    }

    [ClientRpc]
    void RpcResetToCardBase() {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    /// <summary>
    /// Changer l'ascendance du terrain. 
    /// Fonction executée sur le serveur. 
    /// </summary>
    /// <param name="_carteAscendance">Ascendance de la carte qui change l'ascendance du terrain</param>
    [Command]
    void CmdChangeAscendanceTerrain(Entite.Ascendance _carteAscendance) {
        RpcChangeAscendanceTerrain(_carteAscendance); 
    }

    /// <summary>
    /// <see cref="CmdChangeAscendanceTerrain(Ascendance)"/>
    /// </summary>
    /// <param name="_carteAscendance"></param>
    [ClientRpc]
    void RpcChangeAscendanceTerrain (Entite.Ascendance _carteAscendance){
        // Si la carte est astrale ou maléfique, on montre l'effet de terrain. 
        GameObject.FindGameObjectWithTag("GameManager").SendMessage("EffetTerrain", _carteAscendance);
    }

    /// <summary>
    /// Sacrifier une entité
    /// </summary>
    public override void SacrifierCarteEntite() {
        base.SacrifierCarteEntite();
        DetruireCarte(); 
    }

    public override void InformerSacrifierCarteEntite(int numeroEffet, int numeroListEffet=0) {
        base.InformerSacrifierCarteEntite(numeroEffet, numeroListEffet);
        MettreEffetDansLaPile(new List<GameObject>() { gameObject }, numeroEffet, numeroListEffet); 
    }

    /// <summary>
    /// Placer une entité dans le sanctuaire. 
    /// </summary>
    public override void PlacerSanctuaire() {
        base.PlacerSanctuaire();
        CmdChangePosition(State.SANCTUAIRE); 
    }

    /// <summary>
    /// <see cref="Carte.UpdateNewPhase(Player.Phases, int)"/>
    /// </summary>
    /// <param name="_currentPhase">La nouvelle phase</param>
    /// <param name="tour">Le tour</param>
    public override void UpdateNewPhase(Player.Phases _currentPhase, int tour) {
        clicked = false;
        base.UpdateNewPhase(_currentPhase, tour);

        GameManager.AscendanceTerrain _ascendanceTerrain = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetAscendanceTerrain(); 
        switch (_ascendanceTerrain) {
            case GameManager.AscendanceTerrain.NONE:
                return;
            case GameManager.AscendanceTerrain.MALEFIQUE:
                GererEffets(AllEffetsMalefique, _currentPhase, numeroListEffet:2); 
                break;
            case GameManager.AscendanceTerrain.ASTRALE:
                GererEffets(AllEffetsAstral, _currentPhase, numeroListEffet:1);
                break; 
        }
    }

    /// <summary>
    /// Transforme un strin en effet astral. 
    /// </summary>
    /// <param name="allEffets">Le string donné par la base de données gamesparks. </param>
    public void stringToEffetAstral(string allEffets) {
        if (allEffets == "None" || allEffets == "" || allEffets == " ") {
            return;
        }
        string[] AllEffetsStringList = allEffets.Split(':');

        for (int i = 0; i < AllEffetsStringList.Length; ++i) {
            Effet _effet = stringToEffet(AllEffetsStringList[i]);
            AllEffetsAstral.Add(_effet);
        }
    }

    public void stringToEffetMalefique(string allEffets) {
        if (allEffets == "None" || allEffets == "" || allEffets == " ") {
            return;
        }
        string[] AllEffetsStringList = allEffets.Split(':');

        for (int i = 0; i < AllEffetsStringList.Length; ++i) {
            Effet _effet = stringToEffet(AllEffetsStringList[i]);
            AllEffetsMalefique.Add(_effet);
        }
    }

    /// <summary>
    /// <see cref="Carte.GetInfoCarte"/>
    /// </summary>
    /// <returns></returns>
    public override string GetInfoCarte() {
        string stringToReturn = "<color=red>" + Name + "</color>" + "\n" +
            "STAT : " + getPuissance().ToString() + "\n" + "AKA : " + CoutAKA.ToString() + "\n"; 
        if (carteElement == Element.AUCUN) {
            stringToReturn += "Ascendance : " + carteAscendance + "\n"; 
        } else {
            stringToReturn += "Ascendance : ELEMENTAIRE" + "\n" +
                "Element : " + carteElement + "\n"; 
        }

        List<string> noneStrings = new List<string>(){"None", " ", ""};
        // Si le string n'est pas dans la liste des noneStrings;
        if (!(noneStrings.FindIndex(o=>string.Equals(AllEffetsStringToDisplay, o, StringComparison.OrdinalIgnoreCase)) > -1)) {
            stringToReturn += "Effets : " + AllEffetsStringToDisplay + "\n"; 
        } 

        if (!(noneStrings.FindIndex(o => string.Equals(AllEffetsAstralStringToDisplay, o, StringComparison.OrdinalIgnoreCase)) > -1)) {
            stringToReturn += "Astral : " + AllEffetsAstralStringToDisplay + "\n";
        }
        if (!(noneStrings.FindIndex(o => string.Equals(AllEffetsMalefiqueStringToDisplay, o, StringComparison.OrdinalIgnoreCase)) > -1)) {
            stringToReturn += "Malefique : " + AllEffetsMalefiqueStringToDisplay;
        }


        return stringToReturn;
    }

    /// <summary>
    /// Cette fonction est appelée après un effet qui permet à un joueur de changer la position d'une carte. 
    /// </summary>
    public void ChangerPosition() {
        clicked = true; 
    }

    /// <summary>
    /// Regarde si une carte peut jouer un de ses effets.
    /// </summary>
    public override List<EffetPlusInfo> CheckForEffet() {
        // Il faut que l'entité soit dans le main ou dans le sanctuaire pour pouvoir utiliser son effet. 
        List<EffetPlusInfo> effetPlayable = new List<EffetPlusInfo>(); 
        Debug.Log("On check 2 "); 
        if (carteState == State.CHAMPBATAILLE || carteState == State.SANCTUAIRE) {
            // On vérifie les effets que le joueur peut jouer, lors de son tour ou lors du tour de son adversaire uniquement.
            // Pas les effets en réponse à des actions. 
            Debug.Log("On vérifie les effets normaux"); 
            for (int i = 0; i < AllEffets.Count; ++i) {
                if (AllEffets[i].AllConditionsEffet[0].TourCondition != Condition.Tour.NONE) {
                    // S'il vérifie les conditions
                    if (GererConditions(AllEffets[i].AllConditionsEffet, choixJoueur:true)) {
                        // Si l'effet n'a pas déjà été joué à ce tour. 
                        Debug.Log("<color=red>Il y a ici une condition qui peut être jouée</color>");
                        effetPlayable.Add(new EffetPlusInfo(AllEffets[i], i, 0));
                    } 
                } else {
                    Debug.Log("<color=red>" + AllEffets[i].AllConditionsEffet[0].TourCondition.ToString() + "</color>"); 
                }
            }

            Debug.Log("Ici");
            Debug.Log(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetAscendanceTerrain()); 
            if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetAscendanceTerrain() ==
                GameManager.AscendanceTerrain.ASTRALE) {
                Debug.Log("On vérifie les effets astraux"); 
                for (int i = 0; i < AllEffetsAstral.Count; ++i) {
                    if (AllEffetsAstral[i].AllConditionsEffet[0].TourCondition != Condition.Tour.NONE) {
                        // S'il vérifie les conditions

                        if (GererConditions(AllEffetsAstral[i].AllConditionsEffet, choixJoueur: true)) {
                            // Si l'effet n'a pas déjà été joué à ce tour. 
                            Debug.Log("<color=red>Il y a ici une condition qui peut être jouée</color>");
                            effetPlayable.Add(new EffetPlusInfo(AllEffetsAstral[i], i, 1));
                        } else {
                            Debug.Log("Impossible"); 
                        }
                    }
                    else {
                        Debug.Log("<color=red>" + AllEffetsAstral[i].AllConditionsEffet[0].TourCondition.ToString() + "</color>");
                    }
                }
            }

            if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetAscendanceTerrain() ==
                GameManager.AscendanceTerrain.MALEFIQUE) {
                for (int i = 0; i < AllEffetsMalefique.Count; ++i) {
                    if (AllEffetsMalefique[i].AllConditionsEffet[0].TourCondition != Condition.Tour.NONE) {
                        // S'il vérifie les conditions
                        if (GererConditions(AllEffetsMalefique[i].AllConditionsEffet, choixJoueur: true)) {
                            // Si l'effet n'a pas déjà été joué à ce tour. 
                            Debug.Log("<color=red>Il y a ici une condition qui peut être jouée</color>");
                            effetPlayable.Add(new EffetPlusInfo(AllEffetsMalefique[i], i, 2));
                        }
                    }
                    else {
                        Debug.Log("<color=red>" + AllEffetsMalefique[i].AllConditionsEffet[0].TourCondition.ToString() + "</color>");
                    }
                }
            }

        }

        return effetPlayable;
    }

    public override void ProposerEffets(List<EffetPlusInfo> effetPropose) {
        base.ProposerEffets(effetPropose);
    }

    /// <summary>
    /// Update les effets lorsque le terrain change d'ascendance.
    /// </summary>
    /// <param name="_ascendance">la nouvelle ascendance du terrain. </param>
    public void updateChangementAscendaceTerrain(GameManager.AscendanceTerrain _ascendance, 
                                                    GameManager.AscendanceTerrain _previousAscendance=GameManager.AscendanceTerrain.NONE) {
        Debug.Log(_ascendance);
        Debug.Log(_previousAscendance);
        switch (_ascendance) {
            case GameManager.AscendanceTerrain.MALEFIQUE:
                GererEffets(AllEffetsMalefique, debut:true, numeroListEffet:2); 
                break;
            case GameManager.AscendanceTerrain.ASTRALE:
                GererEffets(AllEffetsAstral, debut:true, numeroListEffet:1);
                break;
            case GameManager.AscendanceTerrain.NONE:
                if (_previousAscendance == GameManager.AscendanceTerrain.ASTRALE) {
                    Debug.Log("Ascendance précédente astrale"); 
                    AnnulerEffets(AllEffetsAstral);
                } else {
                    Debug.Log("Ascendance précédente maléfique"); 
                    AnnulerEffets(AllEffetsMalefique);
                }
                break; 
        }

    }

    /// <summary>
    /// <see cref="Carte.updateEffetActive(int, int)"/>
    /// </summary>
    /// <param name="nombreEffet"></param>
    /// <param name="effetListNumber"></param>
    protected override void updateEffetActive(int nombreEffet, int effetListNumber = 0) {
        base.updateEffetActive(nombreEffet, effetListNumber);
        switch (effetListNumber) {
            case 1:
                // Dans le cas où il y a plusieurs conditions, on écrit toujours toutes les variables de timing dans l'int
                // du premier
                AllEffetsAstral[nombreEffet].AllConditionsEffet[0].setSortUtilisePourCeTour();
                break;
            case 2:
                // Dans le cas où il y a plusieurs conditions, on écrit toujours toutes les variables de timing dans l'int
                // du premier
                AllEffetsMalefique[nombreEffet].AllConditionsEffet[0].setSortUtilisePourCeTour();
                break;
        }
    }

    protected override void updateNewTurnEffetUtilise(bool tourJoueurLocal) {
        base.updateNewTurnEffetUtilise(tourJoueurLocal);

        // Dans la classe entité on met aussi à jour les effets astraux et maléfiques. 
        for (int i = 0; i < AllEffetsAstral.Count; ++i) {
            AllEffetsAstral[i].AllConditionsEffet[0].updateUtilisePourCeTour(tourJoueurLocal);
        }

        for (int i = 0; i < AllEffetsMalefique.Count; ++i) {
            AllEffetsMalefique[i].AllConditionsEffet[0].updateUtilisePourCeTour(tourJoueurLocal);
        }
    }
    
    /// <summary>
    /// Lors d'un clic droit sur la carte. 
    /// </summary>
    protected override void RightClickOnCarte() {
        if (clicked) {
            // Si un joueur fait un clic droit alors qu'il tient la carte en main, on remet la carte d'où elle vient. 
            clicked = false;
            // on remet la carte à sa bonne place. 
            RpcChangePosition(carteState);
            return;
        }
        base.RightClickOnCarte();
    }

    /// <summary>
    /// <see cref="Carte.CartePeutJouer(Player.Phases)"/>
    /// </summary>
    /// <param name="_currentPhase"></param>
    public override void CartePeutJouer(Player.Phases _currentPhase) {
        GameManager.AscendanceTerrain _ascendanceTerrain = getGameManager().GetComponent<GameManager>().ascendanceTerrain; 

        base.CartePeutJouer(_currentPhase);

        if (_ascendanceTerrain == GameManager.AscendanceTerrain.ASTRALE) {
            if (CarteJouerReponseEffet(_currentPhase, 1)) {
                setColor(Color.blue); 
            }
        } else if (_ascendanceTerrain == GameManager.AscendanceTerrain.MALEFIQUE) {
            if (CarteJouerReponseEffet(_currentPhase, 2)) {
                setColor(Color.blue);
            }
        }
    }

    /// <summary>
    /// <see cref="Carte.CarteJouerReponseEffet(Player.Phases, int)"/>
    /// </summary>
    /// <param name="_currentPhase"></param>
    /// <param name="numeroListe"></param>
    /// <returns></returns>
    protected override bool CarteJouerReponseEffet(Player.Phases _currentPhase, int numeroListe = 0) {
        List<Effet> EffetsUtilises = new List<Effet>(); 
        switch (numeroListe) {
            case 0:
                EffetsUtilises = AllEffets;
                break; 
            case 1:
                EffetsUtilises = AllEffetsAstral;
                break;
            case 2:
                EffetsUtilises = AllEffetsMalefique;
                break;
        }

        for (int i = 0; i < EffetsUtilises.Count; ++i) {
            // On regarde les effets un par un. 
            // Si à la fin des conditions effetOk == true, alors on pourra réaliser l'effet.
            bool effetOK = GererConditionsRechercheCarte(EffetsUtilises[i].AllConditionsEffet, _currentPhase);
            if (effetOK && CheckIfActionReponseInAction(EffetsUtilises[i].AllActionsEffet)) {
                return true;
            }
        }
        return false;
    }




    /// <summary>
    /// TODO : utiliser la propriété int des enums? 
    /// element to int:
    /// 1 : AIR
    /// 2 : EAU
    /// 3 : FEU
    /// 4 : TERRE
    /// 
    /// On regarde si au moment où l'on check les conditions une carte paye un cout élémentaire. 
    /// Et la carte doit appartenir au joueur dans PAYER_COUT_ELEMENTAIRE
    /// </summary>
    protected override bool CheckIfCartePayeCoutElementaire(int element) {
        try {
            Debug.Log("On regarde si un cout elementaire a été payé"); 
            if (GameObject.FindGameObjectWithTag("Pile") != null) {
                GameObject Pile = GameObject.FindGameObjectWithTag("Pile");
                List<GameObject> allEffetsInPile = Pile.GetComponent<PileAppelEffet>().GetPileEffets();
                for (int i = 0; i < allEffetsInPile.Count; ++i) {
                    Debug.Log("On regarde les effets"); 
                    // Si l'effet est un déplacement vers le sanctuaire
                    if (allEffetsInPile[i].GetComponent<EffetInPile>().numeroEffet == -3
                        && allEffetsInPile[i].GetComponent<EffetInPile>().ObjetEffet.GetComponent<Carte>().isFromLocalPlayer) {
                        Debug.Log("Ici c'est bon"); 
                        switch (allEffetsInPile[i].GetComponent<EffetInPile>().ObjetEffet.GetComponent<Entite>().carteElement) {
                            case Element.AIR:
                                if (element == 1) {
                                    return true;
                                }
                                break;
                            case Element.EAU:
                                if (element == 2) {
                                    return true;
                                }
                                break;
                            case Element.FEU:
                                if (element == 3) {
                                    return true;
                                }
                                break;
                            case Element.TERRE:
                                if (element == 4) {
                                    return true;
                                }
                                break;
                            default:
                                Debug.LogWarning("On ne devrait pas passer par là. ");
                                break;
                        }
                    }
                }
            }
            return false;
        } catch (NullReferenceException e) {
            Debug.LogWarning("Probleme ICI" + e.ToString());
            return false; 
        }
    }

    /// <summary>
    /// On regarde si une carte ne déclare pas d'attaque.
    /// </summary>
    /// <returns></returns>
    protected override bool CheckIfCarteDeclareAttaque() {
        Debug.Log("On regarde si une attaque a été déclarée");
        if (GameObject.FindGameObjectWithTag("Pile") != null) {
            GameObject Pile = GameObject.FindGameObjectWithTag("Pile");
            List<GameObject> allEffetsInPile = Pile.GetComponent<PileAppelEffet>().GetPileEffets();
            for (int i = 0; i < allEffetsInPile.Count; ++i) {
                if (allEffetsInPile[i].GetComponent<EffetInPile>().numeroEffet == -1 && 
                    allEffetsInPile[i].GetComponent<EffetInPile>().PlayerIDAssocie == FindLocalPlayer().GetComponent<Player>().PlayerID) {
                    // On a trouvé dans la pile un effet du joueur qui est une attaque. 
                    return true; 
                }
            }
        }
        return false; 
    }

    /// <summary>
    /// <see cref="Carte.CarteForteFaceA(int)"/>
    /// </summary>
    /// <param name="element"><see cref="CarteForteFaceA(int)"/></param>
    protected override void CarteForteFaceA(int element) {
        // On regarde si la carte n'est pas déjà forte ou faible face à cet élémen
        for (int i = 0; i < ForteFaceAElement.Count; ++i) {
            if (Mathf.Abs(ForteFaceAElement[i]) == element) {
                // Si c'est le cas on udpdate sa force ou sa faiblesse. 
                // Puis on sort de la fonction car l'élément ne peut pas être présent 
                // deux fois dans la liste
                ForteFaceAElement[i] = element;
                return; 
            }
        }
        // S'il n'est pas présent, on ajoute l'élément en bout de liste
        ForteFaceAElement.Add(element); 
    }

    /// <summary>
    /// <see cref="Carte.AnnulerForteFaceA(int)"/>
    /// </summary>
    /// <param name="element"></param>
    protected override void AnnulerForteFaceA(int element) {
        ForteFaceAElement.Remove(element); 
    }

    /// <summary>
    /// On teste si notre entité est forte face à une autre. 
    /// </summary>
    /// <param name="AutreEntite">element de l'autre entité</param>
    /// <returns>un int 
    /// 0 si ni faible ni forte
    /// 1 si forte
    /// -1 si faible</returns>
    public int estForteFaceA(Element _elementAutreEntite, Ascendance _ascendanceAutreEntite) {
        if (ForteFaceAElement.Count == 0) {
            return 0; 
        } else {
            for (int i = 0; i < ForteFaceAElement.Count; ++i) {
                // Les elements sont dans le bon ordre, 
                // AIR, EAU, FEU, TERRE
                if (Mathf.Abs(ForteFaceAElement[i]) == ((int)(_elementAutreEntite) - 1)) {
                    if (ForteFaceAElement[i] > 0) {
                        return 1; 
                    } else {
                        return -1; 
                    }
                } else if (ForteFaceAElement[i] == 5 && _ascendanceAutreEntite == Ascendance.NEUTRE) {
                    return 1; 
                } else if (ForteFaceAElement[i] == -5 && _ascendanceAutreEntite == Ascendance.NEUTRE) {
                    return -1; 
                }
            }
        }
        return 0;
    }

    public int estForteFaceA(GameObject AutreEntite) {
        return estForteFaceA(AutreEntite.GetComponent<Entite>().carteElement, AutreEntite.GetComponent<Entite>().carteAscendance); 
    }

    /// <summary>
    /// <see cref="Carte.GererEffetsPonctuel"/>
    /// </summary>
    public override void GererEffetsPonctuel(Player.Phases phase = Player.Phases.INITIATION, bool debut=false) {
        base.GererEffetsPonctuel();

        GameManager.AscendanceTerrain _ascendanceTerrain = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetAscendanceTerrain();
        switch (_ascendanceTerrain) {
            case GameManager.AscendanceTerrain.NONE:
                return;
            case GameManager.AscendanceTerrain.MALEFIQUE:
                GererEffets(AllEffetsMalefique, _currentPhase: phase, debut:debut, numeroListEffet:2);
                break;
            case GameManager.AscendanceTerrain.ASTRALE:
                GererEffets(AllEffetsAstral, _currentPhase: phase, debut:debut, numeroListEffet:1);
                break;
        }
    }

    /// <summary>
    /// Gérer les effets d'une carte lorsqu'on dépose une carte sur le champ de bataille ou le sanctuaire
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="debut"></param>
    /// <param name="deposeCarte"></param>
    public void GererEffetsPonctuel(Player.Phases phase = Player.Phases.INITIATION, bool debut=false, int deposeCarte=0) {
        GererEffets(AllEffets, _currentPhase: phase, debut: debut, numeroListEffet: 0, deposeCarte:deposeCarte);

        GameManager.AscendanceTerrain _ascendanceTerrain = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetAscendanceTerrain();
        switch (_ascendanceTerrain) {
            case GameManager.AscendanceTerrain.NONE:
                return;
            case GameManager.AscendanceTerrain.MALEFIQUE:
                GererEffets(AllEffetsMalefique, _currentPhase: phase, debut:debut, numeroListEffet:2, deposeCarte:deposeCarte);
                break;
            case GameManager.AscendanceTerrain.ASTRALE:
                GererEffets(AllEffetsAstral, _currentPhase: phase, debut:debut, numeroListEffet:1, deposeCarte:deposeCarte);
                break;
        }
    }

    /// <summary>
    /// <see cref="Carte.isCarteInMain"/>
    /// </summary>
    /// <returns></returns>
    public override bool isCarteInMain() {
        if (carteState == State.MAIN) {
            return true; 
        } else {
            return false;
        }
    }

    public int getPuissance() {
        return puissance.RecupererPuissanceEntite();
    }

}
