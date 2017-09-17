using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 
using System; 
using UnityEngine.UI;
using GameSparks;
using GameSparks.Core; 

public class Entite : Carte {
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

    // Définition des enums nécessaires à la carte, à mettre ailleurs? 
    public enum Ascendance { ASTRALE, NEUTRE, MALEFIQUE, ELEMENTAIRE };
    public enum Element { AIR, EAU, FEU, TERRE, AUCUN };
    public enum ConditionActive { };
    public enum ConditionPassive { };
    public enum State { MAIN, CHAMPBATAILLE, CIMETIERE, DECK, BIGCARD, SANCTUAIRE, ADVERSAIRE };

    public enum EffetMalefique {
        AJOUT_AKA,
        ATTAQUE_DIRECTE,
        ATTAQUE_IMPOSSIBLE,
        NONE
    };
    public enum EffetAstral {
        AJOUT_AKA,
        ATTAQUE_DIRECTE,
        ATTAQUE_IMPOSSIBLE,
        NONE
    };

    /* ==================================
	 * Définition des éléments de la classe
	 * ====================================
	 */
     
    [SyncVar(hook ="ChangeStat")]
    public int STAT;
    public int CoutAKA;
    [SyncVar(hook = "ChangeElement")]
    public Element carteElement;
    public Ascendance carteAscendance;
    // Ceci est une classe.
    public Capacite carteCapacite;
    public EffetAstral carteEffetAstral;
    public EffetMalefique carteEffetMalefique;
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

    public Entite(string _Name, int _ID, int _STAT, int _CoutAKA, Element _carteElement,
        Ascendance _carteAscendance, Capacite _carteCapacite,
        EffetMalefique _carteEffetMalefique, EffetAstral _carteEffetAstral) {
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
        carteCapacite = _carteCapacite;
        carteEffetAstral = _carteEffetAstral;
        carteEffetMalefique = _carteEffetMalefique;
        canGoBig = true;
    }

    public Entite() {
        /* On peut créer la carte sans aucun attribut
		* Pour que ce soit plus clair lors de la création.
		*/
    }

    //public int Attack; 
    //public int Defense;
    public State carteState = State.DECK;
    // Track si l'utilisateur a cliqué sur la carte ou pas. 
    public bool clicked = false;

    public bool hasAttacked;

    // On a besoin de cette information pour pouvoir savoir si on touche la carte dans le cadre d'une phase. 
    public Player.PhasesCapacites CurrentPhaseCapacite = Player.PhasesCapacites.NONE;


    // Définition de la récupération de ces éléments.

    public State getState() {
        return carteState;
    }

    public void setState(string newState) {
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


    //[ServerCallback]
    // Appelé sur le serveur.
    void Update() {

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


    public override void OnMouseDown() {
        /* lors d'un clique sur la carte. 
		 * 
		 * On commence le fait de pouvoir la bouger physiquement. 
		 * Si un deuxième clique toujours le board, on la place sur le board. 
		 * 
		 * TODO: Implémenter également un drag and drop. 
		 */

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;

        ResetLocalScale();

        if (GameObject.Find("GameManager").GetComponent<GameManager>().Tour != FindLocalPlayer().GetComponent<Player>().PlayerID) {
            // On ne peut pas interagir avec ses cartes si ce n'est pas son tour!
            return;
        }

        // S'il y a un sort en cours. 
        if (GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours != null) {
            GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours.SendMessage("RecupererCarteJouerSort", gameObject);
            Debug.Log("Le sort a été joué.");
            return; 
        }

        if (GameObject.Find("GameManager").GetComponent<GameManager>().choixCartes) {
            if (!isFromLocalPlayer) {
                return;
            }
            // On envoie le gameObject au gameManager, puis on arrête la fonction, pour ne pas faire la suite
            GameObject.Find("GameManager").SendMessage("CarteChoisie", gameObject);
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
                clicked = !clicked;
                canGoBig = false;
            } else if (!verrouillee) {
                CheckCardOverlap();
                // Il faut bien regarder que le carte ne soit pas verrouillée!
                ChangePosition(currentPhase);
                clicked = false;
                canGoBig = true;
            }
        } else if (GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.COMBAT) {
            /*
			 * Lors de la phase de combat. 
			 * 
			 * Lors du premier clique: On sélectionne l'unité qui va attaquer, 
			 * Lors du deuxième clique on sélectionne l'unité à attaquer. 
			 */
            Debug.Log("Ceci est une phase de combats");
            if (hasAttacked) {
                GameObject.Find("GameManager").SendMessage("DisplayMessage", "Cette carte ne peut pas/plus attaquer");
            }
            if (isFromLocalPlayer && !hasAttacked) {
                // pour pouvoir attaquer il faut que la carte n'ait pas déjà attaqué. 
                if (carteState != State.CHAMPBATAILLE) {
                    GameObject.Find("GameManager").SendMessage("DisplayMessage", "Une carte qui attaque doit etre sur le champ de bataille");
                } else {
                    GameObject.Find("GameManager").SendMessage("AttackMyPlayer", gameObject);
                }
            } else {
                if (carteState != State.CHAMPBATAILLE) {
                    GameObject.Find("GameManager").SendMessage("DisplayMessage", "Vous ne pouvez attaquer qu'une carte sur le champ de bataille!");
                } else {
                    GameObject.Find("GameManager").SendMessage("AttackOtherPlayer", gameObject);
                }
            }
            return;
        }
    }


    void OnMouseDrag() {




    }

    void OnMouseUp() {
        /*
		 * Lorsque la carte est relachée par l'utilisateur. 
		 * 
		 * TODO: Implémenter cette phase lors du drap and drop. 
		 */


    }

    public override void CreateBigCard(string messageToDisplay="") {

        base.CreateBigCard(
            Name + "\n" +
            "STAT" + STAT.ToString() + "\n" +
            "Nature" + carteElement.ToString());

        BigCard.GetComponent<Entite>().carteState = State.BIGCARD;

    }

    public override void DisplayInfoCarteGameManager(string shortCode = "", string messageToDisplay = "") {
        base.DisplayInfoCarteGameManager(this.shortCode,
            "<color=red>" + Name + "</color>" + "\n" +
            "STAT : " + STAT.ToString() + "\n" +
            "Nature : " + carteElement.ToString() + "\n" + 
            "Effets : " + AllEffetsStringToDisplay + "\n" + 
            "Astral : "  + AllEffetsAstralStringToDisplay + "\n" +
            "Malefique : " + AllEffetsMalefiqueStringToDisplay);
    }

    void ChangePosition(Player.Phases currentPhase) {
        /*
		 * Changer la position de la carte lors d'un reclique. 
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
                } else if (transform.position.y < yMaxSanctuaire && transform.position.y > yMinSanctuaire) {
                    // Si la carte est déposée sur le board. 
                    // On vérifie qu'il y a moins de 2 cartes sur le sanctuaire.
                    MoveToSanctuaire(currentPhase);
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

        if (!transform.parent.parent.parent.gameObject.GetComponent<Player>().isServer) {
            // Command juste sur le client. 
            CmdChangePosition(carteState);
        } else {
            RpcChangePosition(carteState);
        }
    }

    void MoveToSanctuaire(Player.Phases currentPhase) {
        // Lorsque qu'une carte est mise sur le sanctuaire
        // Le state est changé par l'objet sanctuaire
        if (Sanctuaire.GetComponent<Sanctuaire>().getNumberCardsSanctuaire() < 2) {
            // On ne peut invoquer d'entités que lors des phases principales
            if (carteState == State.MAIN && (currentPhase == Player.Phases.PRINCIPALE1 || currentPhase == Player.Phases.PRINCIPALE2)) {
                // Le joueur ne peut pas invoquer plus d'une carte avec un coût élémentaire par tour. 
                // On peut aussi payer des niveaux de couts différents selon l'élément de l'entité. 
                if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().nombreInvocationsSanctuaire >= 1) {
                    return;
                } else {
                    Debug.Log("INVOCATION ELEMENTAIRE");
                    if (carteAscendance == Ascendance.ELEMENTAIRE) {
                        int x = 1;
                        switch (coutElementaire) {
                            case 1:
                                x = 1;
                                break;
                            case 2:
                                x = 3;
                                break;
                            case 3:
                                x = 5;
                                break;
                        }
                        switch (carteElement) {
                            case Element.FEU:
                                // Enlever 10x points de vie au joueur. 
                                if (FindLocalPlayer().GetComponent<Player>().PlayerPV < 10 * x) {
                                    // Si le joueur ne peut pas payer le cout
                                    GameObject.Find("GameManager").SendMessage("DisplayMessage", "Vous ne pouvez pas payer ce cout " + (10 * x).ToString() + " points de vie");
                                    return;
                                } else {
                                    GameObject.Find("GameManager").SendMessage("DisplayMessage", "Vous perdez " + (10 * x).ToString() + " points de vie");
                                    FindLocalPlayer().GetComponent<Player>().AttackPlayer(10 * x);
                                }
                                break;
                            case Element.EAU:
                                // Donner 10x points de vie à l'adversaire. 
                                GameObject.Find("GameManager").SendMessage("DisplayMessage", "Votre adversaire gagne " + (10 * x).ToString() + "points de vie");
                                // Comme le HealPlayer ne marche que sur le joueur local, sinon pas d'autorité
                                // On fait une attaque à l'envers.
                                FindNotLocalPlayer().GetComponent<Player>().AttackPlayer(-10 * x);
                                break;
                            case Element.AIR:
                                gameObject.tag = "BoardSanctuaire";
                                GameObject.Find("GameManager").SendMessage("InvocationElementaireAir", x);
                                break;
                            case Element.TERRE:
                                // Detruire les x premières cartes du deck.
                                GameObject.Find("GameManager").SendMessage("DisplayMessage", x.ToString() + "cartes de votre deck ont été détruites");
                                FindLocalPlayer().SendMessage("DetruireCartesDessusDeck", x);
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
        } else {
            // On envoie un message à l'utilisateur disant qu'il y a plus de 2 cartes sur le sanctuaire.
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "Deja 2 cartes dans le sanctuaire");
        }
    }

    void MoveToChampBataille(Player.Phases currentPhase) {
        // Lorsqu'une carte est mise sur le board. 
        // Le state est changé par l'objet board, à changer ici? 
        if (ChampBataille.GetComponent<CartesBoard>().getNumberCardsChampBataille() < 5) {
            // On ne peut invoquer des entités que lors des phases principales
            if (carteState == State.MAIN && (currentPhase == Player.Phases.PRINCIPALE1 || currentPhase == Player.Phases.PRINCIPALE2)) {
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
                    GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "AKA utilisé: " + CoutAKA.ToString());
                    FindLocalPlayer().SendMessage("subtractAKA", CoutAKA);

                    if ((carteAscendance == Ascendance.MALEFIQUE) || (carteAscendance == Ascendance.ASTRALE)) {
                        CmdChangeAscendanceTerrain(carteAscendance);
                    }
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
            ChampBataille.SendMessage("CmdCarteDeposee", gameObject);
            gameObject.tag = "BoardSanctuaire";
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

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;
        Cimetiere = transform.parent.parent.parent.Find("Cimetiere").Find("CartesCimetiere").gameObject;

        carteState = newCarteState;
        Debug.Log(carteState);
        // il faut maintenant mettre à jour les position. 
        switch (carteState) {
            case State.CHAMPBATAILLE:
                // Delete Card de la main ne sert à rien. 
                Main.SendMessage("DeleteCard", gameObject);
                ChampBataille.SendMessage("CmdCarteDeposee", gameObject);
                Main.SendMessage("ReordonnerCarte");
                gameObject.tag = "BoardSanctuaire";
                break;
            case State.SANCTUAIRE:
                Main.SendMessage("DeleteCard", gameObject);
                Sanctuaire.SendMessage("CmdCarteDeposee", gameObject);
                Sanctuaire.SendMessage("ReordonnerCarte");
                gameObject.tag = "BoardSanctuaire";
                break;
            case State.CIMETIERE:
                Cimetiere.SendMessage("CmdCarteDeposee", gameObject);
                Sanctuaire.SendMessage("ReordonnerCarte");
                ChampBataille.SendMessage("CmdReordonnerCarte");
                gameObject.tag = "Cimetiere";
                break;
        }

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
                break;
            case State.SANCTUAIRE:
                Main.SendMessage("DeleteCard", gameObject);
                Sanctuaire.SendMessage("CmdCarteDeposee", gameObject);
                Sanctuaire.SendMessage("ReordonnerCarte");
                gameObject.tag = "BoardSanctuaire";
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

    void MortCarte() {
        /*
		 * TODO:
		 * Mort de la carte lorsque la carte a 0 points de vie. 
		 * Effets de particule? 
		 * 
		 * 
		 */


    }

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
        }

        yield return new WaitForSeconds(0.1f);

        if (carteState != State.MAIN) {
            yield break;
        }

        Debug.Log(netId.ToString() + hasAuthority.ToString());

        // l'ID de la carte a la même valeur que l'ID réseau donnée par unity.
        IDCardGame = (int)netId.Value;

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
    }

    IEnumerator SetCardOnClient(string ID) {
        //RpcsetoID1 (ID); 
        yield return new WaitForSeconds(1f);
        //RpcsetoID2 (ID); 

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
        IDCardGame = _ID;
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

    void DetruireCarte() {
        /*
		 * On change juste la carte de State pour pouvoir laisser au joueur la possibilité de la récupérer ensuite. 
         * Et pouvoir l'afficher dans le cimetiere. 
		 */

        if (GetComponent<EntiteAssocieeAssistance>() != null) {
            // Si l'entité meurt et qu'elle est liée à une assistance. 
            GetComponent<EntiteAssocieeAssistance>().EntiteDetruite(); 
        }

        Cimetiere = transform.parent.parent.parent.Find("Cimetiere").Find("CartesCimetiere").gameObject;
        Debug.Log(Cimetiere);

        setState("CIMETIERE");
        Cimetiere.SendMessage("CmdCarteDeposee", gameObject);
        Sanctuaire.SendMessage("ReordonnerCarte");
        ChampBataille.SendMessage("CmdReordonnerCarte");

        Debug.Log(transform.parent.parent.parent.gameObject);
        if (transform.parent.parent.parent.gameObject.GetComponent<Player>().isLocalPlayer) {
            /*
			 * Si on est pas dans le cas d'un player local, on ne peut pas envoyer de command. 
			 * 
			 */
            if (!transform.parent.parent.parent.gameObject.GetComponent<Player>().isServer) {
                // Command juste sur le client. 
                // TODO : A changer, faire d'abord l'appel de la command puis l'appel RPC
                CmdChangePosition(carteState);
            } else {
                RpcChangePosition(carteState);
            }
        } else {
            // Si on est pas sur le player local. 
            Debug.Log("N'est pas le local Player");
            GameObject LocalPlayer = FindLocalPlayer();
            LocalPlayer.SendMessage("DetruireCarte", gameObject.GetComponent<Entite>().IDCardGame);
        }

        if ((carteAscendance == Ascendance.MALEFIQUE) || (carteAscendance == Ascendance.ASTRALE)) {
            CmdChangeAscendanceTerrain(Ascendance.NEUTRE); 
        }

        GererEffetsMort(); 
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
        //Debug.Log("carteCapacite : " + carteCapacite.ToString()); 
        Debug.Log("carteEffetAstral : " + carteEffetAstral.ToString());
        Debug.Log("carteEffetMalefique : " + carteEffetMalefique.ToString());

        Debug.Log(" ---------- FIN INFO CARTE ------------");
    }


    // TOUT CE QUI CONCERNE LA GESTION DES CAPACITES : COMPLIQUE A DEPLACER DANS LA CLASSE CAPACITE (a cause des messages à envoyer aux objets). 


    void DoCapacite() {
        /*
		 * Méthode la plus importante, doit faire la capacite. 
		 * 
		 */
        // Premièrement on écrit au debug le nom de la capacite pour voir si on fait le bon effet. 
        WriteCapacite.GetComponent<Text>().text = carteCapacite.name.ToString();

        // Ensuite on fait une chose différente selon chaque capacité. 
        switch (carteCapacite.name) {
            case Capacite.CapaciteName.PIOCHE:
                GameObject.Find("GameManager").SendMessage("PiocheMultiple", carteCapacite.capaciteInt);
                break;
            case Capacite.CapaciteName.MODIFIER_PUISSANCE:
                ModifierPuissance(carteCapacite.capaciteInt, carteCapacite.coutCapacite);
                break;
            case Capacite.CapaciteName.DEGATS:
                StartCoroutine(CartesDegats(carteCapacite.capaciteInt, 1));
                break;
            case Capacite.CapaciteName.DEFAUSSER:
                StartCoroutine(Defausser(carteCapacite.capaciteInt));
                break;










        }

    }

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
        List<GameObject> CartesChoisies = GameManagerObject.GetComponent<GameManager>().CartesChoisies;
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

    void ModifierPuissance(int intModificationPuissance, int coutAKA) {
        /*
		 * On modifie la puissance de la carte, 
		 * Si le cout en AKA est de 0, on modifie directement la puissance,
		 * Sinon on propose au joueur de payer le cout en AKA, pour modifier sa puissance.
		 */
        if (coutAKA == 0) {
            STAT += carteCapacite.capaciteInt;
        } else {
            GameObject GameManagerObject = GameObject.Find("GameManager");

        }
    }

    void resetHasAttacked() {
        /*
		 * A chaque début de tour, toutes les cartes peuvent de nouveau attaquer. 
		 */
        hasAttacked = false;
    }

    void setHasAttacked(bool newHasAttacked = true) {
        hasAttacked = newHasAttacked;
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

    void resetCarteElement() {
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

    void ChangeStat(int newStat) {
        /*
         * Fonction appelée après un changement de stat sur le réseau
         * (hook)
         */
        STAT = newStat;
    }

    [Command]
    public void CmdChangeStat(int newStat) {
        /*
         * Fonction command
         * Changer la stat d'une carte sur le réseau. 
         */
        STAT = newStat;
    }

    [Command]
    public void CmdAddStat(int _statToAdd) {
        /*
         *  Même fonction que la précédente mais au lieu de changer la stat directement
         *  on ajoute simplement le nombre _statToAdd à la stat courante. 
         */
        STAT += _statToAdd;
        RpcChangeFromCardBase(); 
    }

    [Command]
    public void CmdMultiplierStat(int _multiplicateur) {
        STAT *= _multiplicateur;
        Debug.Log("Multiplication des valeurs de la carte1");
        RpcChangeFromCardBase();
        Debug.Log("Multiplication des valeurs de la carte2");
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

    [Command]
    void CmdChangeAscendanceTerrain(Entite.Ascendance _carteAscendance) {
        RpcChangeAscendanceTerrain(_carteAscendance); 
    }

    [ClientRpc]
    void RpcChangeAscendanceTerrain (Entite.Ascendance _carteAscendance){
        // Si la carte est astrale ou maléfique, on montre l'effet de terrain. 
        GameObject.FindGameObjectWithTag("GameManager").SendMessage("EffetTerrain", _carteAscendance);
    }

    public override void SacrifierCarteEntite() {
        base.SacrifierCarteEntite();
        DetruireCarte(); 
    }

    public override void PlacerSanctuaire() {
        base.PlacerSanctuaire();
        CmdChangePosition(State.SANCTUAIRE); 
    }

    public override void UpdateNewPhase(Player.Phases _currentPhase) {
        clicked = false;
        base.UpdateNewPhase(_currentPhase);

        GameManager.AscendanceTerrain _ascendanceTerrain = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetAscendanceTerrain(); 
        switch (_ascendanceTerrain) {
            case GameManager.AscendanceTerrain.NONE:
                return;
            case GameManager.AscendanceTerrain.MALEFIQUE:
                GererEffets(AllEffetsMalefique, _currentPhase); 
                break;
            case GameManager.AscendanceTerrain.ASTRALE:
                GererEffets(AllEffetsAstral, _currentPhase);
                break; 
        }
    }

    public void stringToEffetAstral(string allEffets) {
        if (allEffets == "None") {
            return; 
        }
        string[] AllEffetsStringList = allEffets.Split(':');

        for (int i = 0; i < AllEffetsStringList.Length; ++i) {
            Effet _effet = stringToEffet(AllEffetsStringList[i]);
            AllEffetsAstral.Add(_effet);
            Debug.Log("Effet créé");
        }
    }

    public void stringToEffetMalefique(string allEffets) {
        if (allEffets == "None") {
            return; 
        }
        string[] AllEffetsStringList = allEffets.Split(':');

        for (int i = 0; i < AllEffetsStringList.Length; ++i) {
            Effet _effet = stringToEffet(AllEffetsStringList[i]);
            AllEffetsMalefique.Add(_effet);
            Debug.Log("Effet créé");
        }
    }

}
