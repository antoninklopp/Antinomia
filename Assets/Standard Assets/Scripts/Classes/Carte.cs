using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System; 

public class Carte : NetworkBehaviour {
    /*
     * Parent des cartes de base : entite, sort, assistance.  
     * 
     */

    /// <summary>
    /// Identifiant unique de la carte sur le serveur gameSparks. Chaque instance de la carte aura la même oID, deux cartes
    /// avec des noms différents auront des oID différentes. DANS LA META COLLECTION
    /// </summary>
    public string oID;
    /// <summary>
    /// L'ID est le numéro d'identification de la carte sur la partie. La première carte instanciée sur le serveur aura un ID de 1, la suivante de 2, etc...
    /// </summary>
    public int IDCardGame;
    public string shortCode;
    public string Name;
    public string description;

    /// <summary> 
    /// Le numéro d'identification de la carte dans l'ensemble des cartes du joueur. Utile pour ajouter une carte à un deck et voir si la carte
    /// en question n'est pas en deux fois dans le même deck.
    /// </summary>
    public int IDAllCards; 

    // Grosse carte (zoom) associé... 
    // TODO: Meilleur moyen de le faire? A mettre dans le parent main? 
    [HideInInspector]
    public GameObject BigCard;
    // On ne peut pas utiliser le prefab car si on instancie l'objet depuis le serveur 
    // l'objet public sera clean et on aura un unassigned exception. 
    [HideInInspector]
    public GameObject BigCardText;

    [HideInInspector]
    public GameObject ChampBataille;
    [HideInInspector]
    public GameObject Main;
    [HideInInspector]
    public GameObject Sanctuaire;
    [HideInInspector]
    public GameObject Cimetiere; 

    [HideInInspector]
    public bool isFromLocalPlayer = false;
    [HideInInspector]
    public bool canGoBig = true;

    [HideInInspector]
    // Definition de la carte à l'écran. 
    public float localScaleCard;

    public Sprite Cible;

    // La liste de tous les effets de la carte. 
    public List<Effet> AllEffets = new List<Effet>();
    /// <summary>
    /// Reçu par la bdd Gamesparks "Effet". C'est le string à décortiquer pour créer la liste d'effets. 
    /// </summary>
    public string AllEffetsString = ""; 
    /// <summary>
    /// Reçu par la bdd GameSparks "EffetString". C'est le string à afficher "en français" pour que 
    /// l'utilisateur comprenne l'effet en question. 
    /// </summary>
    public string AllEffetsStringToDisplay = "";

    private List<GameObject> CartesChoisiesPourEffets = new List<GameObject>();

    /// <summary>
    /// Lorsque le joueur a fait un clic droit sur la carte, on lui propose les effets possibles de la carte.
    /// S'il accepte de jouer l'effet, reponseDemandeEffet = 2; sinon 1; pas de reponse = 0
    /// Cette variable permet aussi de savoir si le joueur a déjà executé l'effet de sa carte. 
    /// Dans le cas où la carte n'a le droit de le jouer qu'une fois par tour. 
    /// </summary>
    int reponseDemandeEffet = 0;

    // La carte peut-elle attaquer directement le joueur adverse? 
    public bool attaqueDirecte = false; 



    // Use this for initialization
    public virtual void Start () {
        Cible = Resources.Load<Sprite>("AutresSprites/hit");
    }
	
	// Update is called once per frame
	public virtual void Update () {

	}

    public GameObject FindLocalPlayer() {
        /*
		 * Trouver le joueur local, pour lui faire envoyer les fonctions [Command]
		 */
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (Players[0].GetComponent<Player>().isLocalPlayer) {
            return Players[0];
        }
        else {
            return Players[1];
        }
    }

    public GameObject FindNotLocalPlayer() {
        /*
		 * Trouver le joueur local, pour lui faire envoyer les fonctions [Command]
		 */
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (!Players[0].GetComponent<Player>().isLocalPlayer) {
            return Players[0];
        }
        else {
            return Players[1];
        }
    }

    public void Dragging() {
        /*
		 * Déplacement de la carte qui suit la souris. 
		 */
        Vector3 MousePosition = Input.mousePosition;
        MousePosition.z = 15;
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);
        transform.position = mouseWorldPoint;
    }

    public IEnumerator setImageCarte() {
        /*
		 * On attend un délai le temps que l'information soit arrivée sur le réseau. 
		 * TODO: Attention, il faudrait vérifier l'état du réseau avant d'instancier la carte sur le serveur. 
		 */
        yield return new WaitForSeconds(0.05f);
        if (hasAuthority) {
            // Si ça vient du joueur local, on affiche la carte
            GetComponent<ImageCardBattle>().setImage(shortCode);
        }
        else {
            // Sinon on affiche le dos de la carte. 
            GetComponent<ImageCardBattle>().setDosCarte();
        }
    }

    public void OnMouseEnter() {
        /*
		 * Lorsque la souris rencontre la carte (sans clique). 
		 * Animation pour grossir la carte si elle est dans la main. 
		 * On crée le zoom sur la carte. 
		 */


        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;

        isFromLocalPlayer = transform.parent.parent.parent.gameObject.GetComponent<Player>().isLocalPlayer;

        checkIfLocalPlayerOnMousEnter(); 

        if (!isFromLocalPlayer) {
            return;
        }

        // Si un sort est en train d'être joué, on ne veut pas grossir la carte. 
        if (GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours != null) {
            return;
        }

        if (canGoBig) {
            //CreateBigCard();
            //GetComponent<SpriteRenderer>().enabled = false;
            //// On désactive les objets enfants lors de la création de la grosse carte. 
            //for (int i = 0; i < transform.childCount; ++i) {
            //    transform.GetChild(i).gameObject.SetActive(false);
            //}

            DisplayInfoCarteGameManager(); 
        }
    }

    public virtual void OnMouseExit() {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().HideInfoCarte();
    }

    public void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            RightClickOnCarte();
        }
    }

    public virtual void OnMouseDown() {
        if (!isFromLocalPlayer) {
            return; 
        } 
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().gameIsPaused 
            && !GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().IPausedTheGame) {
            // Pas d'interaction possible, si le joueur n'a pas mis le jeu en pause, mais qu'il est en pause. 
            DisplayMessage("Votre adversaire a mis le jeu en pause");
            return; 
        }
    }

    public virtual void checkIfLocalPlayerOnMousEnter() {
        /*
         * Dans les classes enfant cette fonction sert à changer le State en State.ADVERSAIRE
         * dans le cas où la carte n'est pas du joueur local. 
         */ 
    }

    public IEnumerator AnimationFinBigCard() {
        /*
		 * Petite animation de fin lors de la sortie de la souris de la carte. 
		 * La carte "dézoome un tout petit peu"
		 */
        transform.localScale *= 1.2f;
        for (int i = 0; i <= 10; ++i) {
            // La carte dézoome
            transform.localScale -= Vector3.one * localScaleCard * 0.02f;
            yield return new WaitForSeconds(0.001f);
        }
        ResetLocalScale();
    }


    public void ResetLocalScale() {
        transform.localScale = new Vector3(localScaleCard, localScaleCard, 1f);
    }

    public virtual void CreateBigCard(string messageToDisplay="") {
        /*
		 * Création du zoom sur la carte. Lorsque la souris passe sur la carte. 
		 * TODO : A mettre dans la classe sort. Faire hériter les deux objets d'une autre classe. 
		 */
        BigCard = Instantiate(gameObject);
        BigCard.GetComponent<CarteType>().enabled = false;
        BigCard.transform.SetParent(gameObject.transform.parent);
        // On désactive le box collider pour que le mouvement de la souris n'affecte pas la carte. 
        BigCard.GetComponent<BoxCollider2D>().enabled = false;
        BigCard.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        BigCard.transform.localScale = new Vector3(2f * localScaleCard, 2f * localScaleCard, 1f);
        // BigCard.transform.rotation.eulerAngles = new Vector3 (0, 0, 0);
        BigCard.GetComponent<SpriteRenderer>().sortingLayerName = "BigCarte";

        // On crée maintenant les informations que l'on va écrire à droite de la carte. 
        BigCardText = GameObject.Find("BigCardText");
        GameObject _text = Instantiate(BigCardText);
        _text.transform.SetParent(BigCard.transform, false);
        _text.transform.position = new Vector2(BigCard.transform.position.x + 2f, BigCard.transform.position.y + 1.5f);
        _text.GetComponent<TextMesh>().text = messageToDisplay; 
    }

    public virtual void DisplayInfoCarteGameManager(string shortCode="", string messageToDisplay = "") {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ShowCarteInfo(shortCode, messageToDisplay); 
    }

    public List<GameObject> FindAllEntiteChampBatailleElement(Entite.Element _element) {
        /*
         * Renvoie toutes les cartes jouées sur le champ de bataille qui sont d'élément _element. 
         * 
         */
        List<GameObject> AllEntiteElement = new List<GameObject>();
        GameObject[] AllEntiteJoues = GameObject.FindGameObjectsWithTag("BoardSanctuaire");
        for (int i = 0; i < AllEntiteJoues.Length; ++i) {
            if (AllEntiteJoues[i].GetComponent<Entite>().carteElement == _element
                && AllEntiteJoues[i].GetComponent<Entite>().carteState == Entite.State.CHAMPBATAILLE) {
                AllEntiteElement.Add(AllEntiteJoues[i]);
            }
        }
        return AllEntiteElement;
    }

    public Condition stringToCondition(string stringCondition, string IntCondition) {
        /*
         * renvoie la condition sort associée au string.
         * on essaie de convertir le string en enum grâce à la méthode enum.parse. 
         * 
         */
        //Sort.ConditionSort conditionSort;
        return new Condition((Condition.ConditionEnum)Enum.Parse(typeof(Condition.ConditionEnum), value: stringCondition),
            int.Parse(IntCondition));

    }

    public List<Condition> stringToConditionList(string allConditions) {
        /*
         * Transforme le string donné par la base de données gamesparks 
         * en un ensemble de conditions nécessaires pour effectuer un sort
         */
        // On split la chaine de caractères reçue en entrée

        List<Condition> AllConditions = new List<Condition>();
        if (allConditions != "") {
            string[] AllConditionsStringList = allConditions.Split(';');
            // On vérifie la longueur
            if (AllConditionsStringList.Length % 2 != 0) {
                throw new Exception("La longueur de cette liste doit être un multiple de 2" + AllConditionsStringList[0]);
            }
            for (int i = 0; i < AllConditionsStringList.Length; i += 2) {
                // On ajoute toutes les conditions du sort. 
                Condition sort = stringToCondition(AllConditionsStringList[i], AllConditionsStringList[i + 1]);
                AllConditions.Add(sort);
            }
        }
        return AllConditions;
    }

    public List<Action> stringToActionList(string allEffets) {
        /*
        * Transforme le string donné par la base de données gamesparks 
        * en un ensemble d'effets effectués par le sort. 
        */

        List<Action> AllActions = new List<Action>();

        string[] AllActionsStringList = allEffets.Split(';');
        if (AllActionsStringList.Length % 2 != 0) {
            throw new Exception("La longueur de la liste doit être un multiple de deux");
        }
        else if (AllActionsStringList.Length < 2) {
            throw new Exception("Il doit y avoir au moins un effet pour ce sort!" + "\n" + "La carte");
        }
        else {
            for (int i = 0; i < AllActionsStringList.Length; i += 2) {
                Action sort = stringToAction(AllActionsStringList[i], AllActionsStringList[i + 1]);
                AllActions.Add(sort);
                Debug.Log("Effet 1 : " + stringToAction(AllActionsStringList[i], AllActionsStringList[i + 1]).ActionAction);
            }
        }

        return AllActions;
    }

    public Action stringToAction(string stringEffet, string IntEffet) {
        /*
         * Renvoie l'effet associé au string. 
         */
        return new Action((Action.ActionEnum)Enum.Parse(typeof(Action.ActionEnum), value: stringEffet),
            int.Parse(IntEffet));
    }

    public void stringToEffetList(string allEffets) {
        if (allEffets == "None"){
            return; 
        }
        string[] AllEffetsStringList = allEffets.Split(':');

        for (int i = 0; i < AllEffetsStringList.Length; ++i) {
            Effet _effet = stringToEffet(AllEffetsStringList[i]);
            AllEffets.Add(_effet);
            Debug.Log("Effet créé");
        }
    }

    public Effet stringToEffet(string _effetString) {
        Effet newEffet = new Effet();
        string ConditionList = _effetString.Split('!')[0];
        string ActionList = _effetString.Split('!')[1];

        newEffet.AllActionsEffet = stringToActionList(ActionList);
        newEffet.AllConditionsEffet = stringToConditionList(ConditionList); 

        return newEffet; 
    }

    public void setBigCardPosition(Vector2 position) {
        BigCard.transform.position = position; 
    }

    public void DestroyBigCard() {
        GetComponent<SpriteRenderer>().enabled = true;
        Destroy(BigCard);
        // On lance la petite animation de fin. 
        // StartCoroutine(AnimationFinBigCard());
        // On réactive les objets enfants lors de la destruction de la grosse carte. 
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void JouerEffetDeposeCarte() {
        Debug.Log("Les effets sont en cours de traitement"); 
        GererEffets(AllEffets);
    }

    public virtual void UpdateNewPhase(Player.Phases _currentPhase) {
        /*
         * A chaque nouvelle phase on vérifie que la carte n'a pas quelque chose à jouer à ce tour. 
         * 
         * Dans ses capacités. 
         */
        // A mettre dans entité uniquement. 
        GameManager.AscendanceTerrain currentAscendance = 
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetAscendanceTerrain();

        // Gestion "des effets normaux"
        GererEffets(AllEffets, _currentPhase); 
    }

    /// <summary>
    /// Gerer les effets d'une carte
    /// </summary>
    /// <param name="_allEffets">Liste des effets à effectier</param>
    /// <param name="_currentPhase">Phase lors dans laquelle on se trouve</param>
    /// <param name="debut">true, si on la carte vient d'être posée</param>
    public void GererEffets(List<Effet> _allEffets, Player.Phases _currentPhase=Player.Phases.INITIATION, bool debut=false) {
        if (_allEffets.Count == 0) {
            // La carte n'a aucune capacité/effet lors de tours. 
        }
        for (int i = 0; i < _allEffets.Count; ++i) {
            // On regarde les effets un par un. 
            // Si à la fin des conditions effetOk == true, alors on pourra réaliser l'effet.
            bool effetOK = GererConditions(_allEffets[i].AllConditionsEffet, _currentPhase, debut:debut);
            Debug.Log("<color=orange>" + effetOK.ToString() + "</Color>"); 
            if (effetOK) {
                // Dans le cas où toutes les conditions sont réunies. 
                GererActions(_allEffets[i].AllActionsEffet);
            }
            else {
                Debug.Log("<color=orange>L'effet n'a pas pu être joué, pour diverses raisons. </color>");
            }
        }
    }

    /// <summary>
    /// Lors qu'une carte est détruite, ou que le terrain change d'ascendance, il faut annuler les effets qui
    /// avaient lieu précedemment . 
    /// </summary>
    /// <param name="_allEffets">Liste des effets à annuler.</param>
    public void AnnulerEffets(List<Effet> _allEffets) {



    }

    /// <summary>
    /// Gerer les conditions avant d'executer les actions d'un effet. 
    /// </summary>
    /// <param name="_conditions">Liste des conditions à vérifier</param>
    /// <param name="_currentPhase">Phase dans laquelle on se trouve</param>
    /// <param name="estMort">Si la carte vient d'être détruite</param>
    /// <param name="debut">Si la carte vient d'être posée</param>
    /// <returns>true, si toutes les conditions sont remplies</returns>
    public bool GererConditions(List<Condition> _conditions, Player.Phases _currentPhase=Player.Phases.INITIATION, 
                                    bool estMort=false, bool debut=false) {
        /*
         * Regarde si toutes les conditions sont ok pour un effet. 
         * Retourne true si c'est le cas, false sinon
         * 
         */
        bool effetOK = true;
        for (int j = 0; j < _conditions.Count; ++j) {
            //if (!_conditions[j].dependsOnPhase) {
            //    // Si l'effet ne dépend pas d'une phase, il n'y pas de raison
            //    break;
            //}
            if (_conditions[j].dependsOnPhase &&
                _currentPhase != _conditions[j].PhaseCondition) {
                // La condition de phase n'est pas remplie, donc on passe à l'effet suivant 
                return false; 
            }
            else if (((_conditions[j].TourCondition == Condition.Tour.TOUR_LOCAL) && (
                  GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Tour !=
                  FindLocalPlayer().GetComponent<Player>().PlayerID)) ||
                  (_conditions[j].TourCondition == Condition.Tour.TOUR_NOT_LOCAL)
                  && (FindLocalPlayer().GetComponent<Player>().PlayerID ==
                  GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Tour)) {
                // La condition de tour n'est pas remplie, on passe donc à l'effet suivant. 
                return false; 
            }
            else if (debut && (_conditions[j].intCondition > 100)) {
                // L'effet dépend donc d'une phase. 
                return false; 
            }
            else {
                Debug.Log("<color=green>Verification de conditions </color>"); 
                // if (_conditions[j].ActionObligatoire) {
                    // Dans le cas d'une action obligatoire. 
                    switch (_conditions[j].ConditionCondition) {
                        case Condition.ConditionEnum.NONE:
                            // Dans le cas où il n'y a aucune condition. 
                            return true; 
                        case Condition.ConditionEnum.CARTES_CIMETIERE:
                            if (FindLocalPlayer().transform.Find("Cimetiere").
                                Find("CartesCimetiere").gameObject.GetComponent<Cimetiere>().NombreDeCartesDansCimetiere() <
                                _conditions[j].properIntCondition) {
                                return false; 
                            }
                            break;
                        case Condition.ConditionEnum.CHOIX_ELEMENT:
                            // Permettre au joueur de choisir un élément.
                            break;
                        case Condition.ConditionEnum.CHOIX_ENTITE_CHAMP_BATAILLE:
                            ShowCardsForChoiceChampBatailleDeuxJoueurs();
                            break;
                        case Condition.ConditionEnum.CHOIX_ENTITE_CHAMP_BATAILLE_ADVERSAIRE:
                            ShowCardsForChoice(FindNotLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur"));
                            break;
                        case Condition.ConditionEnum.CHOIX_ENTITE_CHAMP_BATAILLE_JOUEUR:
                            ShowCardsForChoice(FindLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur"));
                            break;
                        case Condition.ConditionEnum.CHOIX_ENTITE_TERRAIN:
                            ShowCardsForChoiceAllCartesDeuxJoueurs(); 
                            break;
                        case Condition.ConditionEnum.DEFAUSSER:
                            ShowCardsForChoice(FindLocalPlayer().transform.Find("MainJoueur").Find("CartesMainJoueur"));
                            break;
                        case Condition.ConditionEnum.DELTA:
                            // La condition delta est par rapport à TOUTES les cartes du terrain. 
                            if (_conditions[j].properIntCondition + FindAllCartesLocal().Count >
                                FindAllNotLocalCartes().Count) {
                                return false; 
                            }
                            break;
                        case Condition.ConditionEnum.MORT:
                            if (!estMort) {
                                return false;
                            }
                            break;
                        default:
                            Debug.LogWarning("<color=rouge>Cette capacité n'est pas encore gérée par le code</color>");
                            break;
                    //}
                }
                //else {
                    // Si l'action n'est pas obligatoire, on n'oblige pas le joueur à jouer cet effet à ce moment précis. 
                    // On fait donc un break et passe à l'effet suivant. 
                 //   break;
                //}
                Debug.Log("<color=gree>effet est OK</color>");
            }
        }
        Debug.Log(effetOK);
        return effetOK; 

    }


    /// <summary>
    /// Gérer les actions d'un effet. 
    /// </summary>
    /// <param name="_actions">Liste des actions à appliquer.</param>
    public void GererActions(List<Action> _actions) {
        // On doit maintenant gérer les effets.
        for (int j = 0; j < _actions.Count; ++j) {
            switch (_actions[j].ActionAction) {
                case Action.ActionEnum.ATTAQUE_OBLIGATOIRE:
                    Debug.LogWarning("Cet effet ne peut pas être joué ici. A vérifier.");
                    break;
                case Action.ActionEnum.CHANGER_POSITION:
                    /*
                     * Il faut d'abord choisir une carte puis changer sa position.
                     * La carte aura été choisie précédemment. 
                     */

                    DisplayMessage("Choisissez une carte et changez sa position");
                    StartCoroutine(ChangerPositionEffet()); 
                    
                    break;
                case Action.ActionEnum.DETRUIRE:
                    /*
                     * Il faut d'abord choisir une carte puis la détruire.
                     */
                    DisplayMessage("Choisissez uen carte et détruisez la");
                    StartCoroutine(DetruireEffet()); 
                    
                    break;
                case Action.ActionEnum.GAIN_AKA_UN_TOUR:
                    FindLocalPlayer().GetComponent<Player>().subtractAKA(-_actions[j].intAction);
                    DisplayMessage("Ajout de " + _actions[j].intAction.ToString() + "AKA à ce tour"); 
                    break;
                case Action.ActionEnum.PIOCHER_CARTE:
                    StartCoroutine(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PiocheMultiple(_actions[j].properIntAction));
                    DisplayMessage("Pioche " + _actions[j].properIntAction.ToString() +  " carte");
                    Debug.Log("<color=purple> Piocher une carte </color>"); 
                    break;
                case Action.ActionEnum.SACRIFIER_CARTE:
                    // Sacrifier cette carte. 
                    DisplayMessage("<color=orange>Sacrifice de cette carte</color>"); 
                    SacrifierCarteEntite();
                    break;
                case Action.ActionEnum.PLACER_SANCTUAIRE:
                    DisplayMessage("On place cette carte dans le sanctuaire"); 
                    PlacerSanctuaire();
                    break;
                case Action.ActionEnum.PUISSANCE_MULTIPLIE:
                    // Multiplier la puissance des entités SUR LE CHAMP DE BATAILLE. 
                    Debug.Log("Multiplication de la puissance"); 
                    List<GameObject> CartesLocales = FindAllCartesLocal(); 
                    for (int i = 0; i < CartesLocales.Count; ++i) {
                        Debug.Log(3); 
                        if (CartesLocales[i].GetComponent<Entite>() != null) {
                            Debug.Log(CartesLocales[i].GetComponent<Entite>().Name);
                            if (CartesLocales[i].GetComponent<Entite>().carteState == Entite.State.CHAMPBATAILLE) {
                                CartesLocales[i].GetComponent<Entite>().CmdMultiplierStat(_actions[j].properIntAction); 
                            }
                        }
                    }
                    break;
                case Action.ActionEnum.ATTAQUE_IMPOSSIBLE:
                    GetComponent<Entite>().hasAttacked = -1;
                    DisplayMessage("Cette entité ne peut pas attaquer"); 
                    break;
                case Action.ActionEnum.DEFAUSSER:
                    // On propose de défausser. 
                    // Il faut d'abord choisir une carte de la main du joueur. 
                    ShowCardsForChoice(Main.transform);
                    StartCoroutine(DefausserEffet()); 
                    break;
                case Action.ActionEnum.REVELER_CARTE:
                    // Le joueur révèle une carte de sa main.
                    StartCoroutine(RevelerCarteEffet(_actions[j].properIntAction));
                    break;
                case Action.ActionEnum.REVELER_CARTE_ADVERSAIRE:
                    // L'adversaire doit révéler une carte. 
                    Debug.Log("<color=purple>Reveler carte adversaire</color>"); 
                    FindLocalPlayer().GetComponent<Player>().CmdEnvoiMethodToServerCarteWithIntParameter(IDCardGame, 
                        "RevelerCarteEffet", _actions[j].properIntAction, FindLocalPlayer().GetComponent<Player>().PlayerID); 
                    break;
                case Action.ActionEnum.ATTAQUE_DIRECTE:
                    attaqueDirecte = true; 
                    break; 
                default:
                    Debug.LogWarning("Cet effet n'est pas géré");
                    break;
            }
        }
    }

    /// <summary>
    /// Gérer les effets à la mort de la carte.
    /// </summary>
    public void GererEffetsMort() {
        /*
         * Gérer les effets à la mort de la carte. 
         * 
         */

        bool effetOK = false; 

         for (int i = 0; i < AllEffets.Count; ++i) {
            effetOK = false; 
            for (int j = 0; j < AllEffets[i].AllConditionsEffet.Count; ++j) {
                if (AllEffets[i].AllConditionsEffet[j].ConditionCondition == Condition.ConditionEnum.MORT) {
                    effetOK = GererConditions(AllEffets[i].AllConditionsEffet, estMort: false); 
                }
            }
            if (effetOK) {
                GererActions(AllEffets[i].AllActionsEffet); 
            }
         }
    }

    public virtual void SacrifierCarteEntite() {
        // Les seules cartes que l'on peut sacrifier sont des entités. 
        // La méthode est donc override dans la classe entité. 
    }

    public virtual void PlacerSanctuaire() {
        // Les seuls cartes quel'on peut sacrifier sont des entités. 
        // La méthode est donc override dans la classe entité. 
    }

    public void CartesChoisies(List<int> AllIDCartesChoosen) {
        List<GameObject> _allObjectsChoosen = new List<GameObject>();
        for (int i = 0; i < AllIDCartesChoosen.Count; ++i) {
            _allObjectsChoosen.Add(FindCardWithID(AllIDCartesChoosen[i])); 
        }
        CartesChoisiesPourEffets = _allObjectsChoosen; 
    }

    GameObject FindCardWithID(int _ID_) {
        /*
		 * Trouver la carte avec la bonne ID. 
         * Doit être la même méthode que dans player (à relier). 
		 */
        CarteType[] AllCartesType = FindObjectsOfType(typeof(CarteType)) as CarteType[];
        List<GameObject> AllCartes = new List<GameObject>();

        for (int i = 0; i < AllCartesType.Length; ++i) {
            GameObject NewCarte = AllCartesType[i].gameObject;
            if (NewCarte.GetComponent<CarteType>().instanciee) {
                AllCartes.Add(NewCarte);
            }
        }

        Debug.Log("AllCartes" + AllCartes.Count.ToString());
        for (int i = 0; i < AllCartes.Count; ++i) {
            // On cherche la carte avec le bon ID
            switch (AllCartes[i].GetComponent<CarteType>().thisCarteType) {
                case CarteType.Type.ASSISTANCE:
                    if (AllCartes[i].GetComponent<Assistance>().IDCardGame == _ID_) {
                        return AllCartes[i];
                    }
                    break;
                case CarteType.Type.SORT:
                    if (AllCartes[i].GetComponent<Sort>().IDCardGame == _ID_) {
                        return AllCartes[i];
                    }
                    break;
                case CarteType.Type.ENTITE:
                    if (AllCartes[i].GetComponent<Entite>().IDCardGame == _ID_) {
                        return AllCartes[i];
                    }
                    break;
            }
        }
        throw new Exception("La carte n'a pas été trouvée");
        // return null; 
    }


    /// <summary>
    /// Montrer au joueur les cartes qu'il peut choisir à partir d'un objet parent (Transform). 
    /// </summary>
    void ShowCardsForChoice(Transform _parent) {
        List<GameObject> AllCardsToChoose = new List<GameObject>();
        for (int k = 0; k < _parent.childCount; ++k) {
            AllCardsToChoose.Add(_parent.GetChild(k).gameObject);
        }
        GameObject.FindGameObjectWithTag("GameManager").transform.Find("ShowCards").gameObject.GetComponent<ShowCards>().ShowCardsToChoose(
            AllCardsToChoose, gameObject);
    }

    void ShowCardsForChoiceChampBatailleDeuxJoueurs() {
        List<GameObject> AllCardsToChoose = new List<GameObject>();
        for (int k = 0; k < FindNotLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").childCount; ++k) {
            AllCardsToChoose.Add(FindNotLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").GetChild(k).gameObject);
        }
        for (int k = 0; k < FindLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").childCount; ++k) {
            AllCardsToChoose.Add(FindLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").GetChild(k).gameObject);
        }
        GameObject.FindGameObjectWithTag("GameManager").transform.Find("ShowCards").gameObject.GetComponent<ShowCards>().ShowCardsToChoose(
            AllCardsToChoose, gameObject);
    }

    void ShowCardsForChoiceAllCartesDeuxJoueurs() {
        List<GameObject> AllCardsToChoose = new List<GameObject>();
        GameObject[] AllCardsBoardSanctuaire = GameObject.FindGameObjectsWithTag("BoardSanctuaire"); 
        for (int i = 0; i < AllCardsBoardSanctuaire.Length; ++i) {
            AllCardsToChoose.Add(AllCardsBoardSanctuaire[i]); 
        }
        GameObject.FindGameObjectWithTag("GameManager").transform.Find("ShowCards").gameObject.GetComponent<ShowCards>().ShowCardsToChoose(
            AllCardsToChoose, gameObject);
    }
    

    /// <summary>
    /// Renvoie toutes les cartes du joueur local. 
    /// </summary>
    /// <returns>
    /// Liste de toutes les cartes du joueur local.
    /// </returns>
    List<GameObject> FindAllCartesLocal() {
        List<GameObject> CartesLocal = new List<GameObject>(); 
        GameObject[] AllCartes = GameObject.FindGameObjectsWithTag("BoardSanctuaire");
        for (int i = 0; i < AllCartes.Length; ++i) {
            if (AllCartes[i].GetComponent<Carte>().isFromLocalPlayer && 
                ((AllCartes[i].GetComponent<Entite>().carteState == Entite.State.SANCTUAIRE)
                || AllCartes[i].GetComponent<Entite>().carteState == Entite.State.CHAMPBATAILLE)) {
                CartesLocal.Add(AllCartes[i]); 
            }
        }

        GameObject[] AllAssistances = GameObject.FindGameObjectsWithTag("Assistance");
        for (int i = 0; i < AllAssistances.Length; ++i) {
            if (AllAssistances[i].GetComponent<Carte>().isFromLocalPlayer &&
                (AllAssistances[i].GetComponent<Assistance>().assistanceState == Assistance.State.ASSOCIE_A_CARTE)) {
                CartesLocal.Add(AllAssistances[i]);
            }
        }
        return CartesLocal; 
    }

    /// <summary>
    /// Renvoie toutes les cartes du joueur qui n'est pas le joueur local. 
    /// </summary>
    /// <returns>
    /// Liste de toutes les cartes du joueur qui n'est pas le joueur local.
    /// </returns>
    List<GameObject> FindAllNotLocalCartes() {
        List<GameObject> CartesLocal = new List<GameObject>();
        GameObject[] AllCartes = GameObject.FindGameObjectsWithTag("Carte");
        for (int i = 0; i < AllCartes.Length; ++i) {
            if (!AllCartes[i].GetComponent<Carte>().isFromLocalPlayer &&
                ((AllCartes[i].GetComponent<Entite>().carteState == Entite.State.SANCTUAIRE)
                || AllCartes[i].GetComponent<Entite>().carteState == Entite.State.CHAMPBATAILLE)) {
                CartesLocal.Add(AllCartes[i]);
            }
        }

        GameObject[] AllAssistances = GameObject.FindGameObjectsWithTag("Assistance");
        for (int i = 0; i < AllAssistances.Length; ++i) {
            if (!AllAssistances[i].GetComponent<Carte>().isFromLocalPlayer &&
                (AllAssistances[i].GetComponent<Assistance>().assistanceState == Assistance.State.ASSOCIE_A_CARTE)) {
                CartesLocal.Add(AllAssistances[i]);
            }
        }
        return CartesLocal;
    }


    public void DisplayMessage(string Message) {
        /*
         * Permet de montrer un message sur l'écran, 
         * notamment pour informer le joueur d'un effet qui vient de se passer
         * tant qu'il n'en a pas la preuve visuelle. 
         */ 
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().DisplayMessage(Message);
    }

    /// <summary>
    /// Récupérer les informations essentielles sur la carte.
    /// </summary>
    public virtual string GetInfoCarte() {
        return ""; 
    } 

    /// <summary>
    /// Cette coroutine permet d'attendre des cartes choisies pour qu'on éxécute un effet.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForCardsChosen() {
        CartesChoisiesPourEffets = null; 
        while (CartesChoisiesPourEffets == null) {
            yield return new WaitForSeconds(0.2f); 
        }
    }

    public void ProposerMettreJeuEnPause() {
        StartCoroutine(GameObject.Find("GameManager").GetComponent<GameManager>().ProposeToPauseGame()); 
    }

    /// <summary>
    /// Lorsqu'on détecte un clic droit, on permet au joueur de jouer l'effet de la carte. 
    /// </summary>
    public void RightClickOnCarte() {
#if (!UNITY_ANDROID || !UNITY_IOS)
        // Dans le cas d'un clic droit. 
        Debug.Log("On check" + Name);
        int effetPropose = CheckForEffet(); 
        if (effetPropose != -1) {
            ProposerEffets(effetPropose);
        }
#endif
    }

    public virtual int CheckForEffet() {
        return -1; 


    }

    public virtual void ProposerEffets(int effetPropose) {
        // Proposer au joueur de pouvoir jouer un effet. 
        string toDisplay = ""; 
        for (int j = 0; j < AllEffets[effetPropose].AllActionsEffet.Count; ++j) {
            Debug.Log("<color=green>" + AllEffets[effetPropose].AllActionsEffet[j].ActionAction.ToString() + "</color>"); 
            toDisplay += AllEffets[effetPropose].AllActionsEffet[j].ActionAction.ToString(); 
        }
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ProposerEffetJoueur(toDisplay, gameObject);
        StartCoroutine(WaitForResponseEffetPropose(effetPropose)); 
    }

    private IEnumerator WaitForResponseEffetPropose(int effetPropose) {
        while (reponseDemandeEffet == 0) {
            yield return new WaitForSeconds(0.2f); 
        }
        if (reponseDemandeEffet == 1) {
            Debug.Log("<color=orange>Reponse Pas Ok</color>"); 
        } else {
            Debug.Log("<color=orange> Reponse OK </color>");
            GererActions(AllEffets[effetPropose].AllActionsEffet); 
        }
        // L'effet a été joué à ce tour.
        reponseDemandeEffet = -1; 
    }

    private void ReponseEffet(int response) {
        reponseDemandeEffet = response; 
    }

    private IEnumerator ChangerPositionEffet() {

        yield return WaitForCardsChosen();
        if (CartesChoisiesPourEffets.Count != 1) {
            throw new Exception("Erreur dans la transmission des cartes. Cet effet ne permet pas encore" +
                "de gérer plusieurs changements de position. ");
        }
        else {
            CartesChoisiesPourEffets[0].GetComponent<Entite>().ChangerPosition();
            Debug.Log("L'effet changement de position a été autorisé");
        }
        CartesChoisiesPourEffets = null; 
    }

    private IEnumerator DetruireEffet() {
        yield return WaitForCardsChosen(); 
        for (int i = 0; i < CartesChoisiesPourEffets.Count; ++i) {
            // On détruit toutes les cartes une par une. 
            CartesChoisiesPourEffets[i].SendMessage("DetruireCarte");
        }
        CartesChoisiesPourEffets = null; 
    }

    private IEnumerator DefausserEffet() {
        yield return WaitForCardsChosen(); 
        for (int i = 0; i < CartesChoisiesPourEffets.Count;  ++i) {
            // On détruit les cartes choisies 
            CartesChoisiesPourEffets[i].SendMessage("DetruireCarte"); 
        }
        CartesChoisiesPourEffets = null; 
    }

    /// <summary>
    /// Révéler des cartes à son adversaire. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator RevelerCarteEffet(int nombreCartes) {

        Debug.Log("On revele une carte à l'adversaire. ");
        ShowCardsForChoice(Main.transform);
        
        yield return WaitForCardsChosen();
        string[] AllCartesChoisiesString = new string[CartesChoisiesPourEffets.Count];
        for (int i = 0; i < CartesChoisiesPourEffets.Count; ++i) {
            string stringCartei = ""; 
            switch (CartesChoisiesPourEffets[i].GetComponent<CarteType>().thisCarteType) {
                case CarteType.Type.ENTITE:
                    stringCartei = CartesChoisiesPourEffets[i].GetComponent<Entite>().shortCode;
                    break;
                case CarteType.Type.ASSISTANCE:
                    stringCartei = CartesChoisiesPourEffets[i].GetComponent<Assistance>().shortCode;
                    break;
                case CarteType.Type.SORT:
                    stringCartei = CartesChoisiesPourEffets[i].GetComponent<Sort>().shortCode;
                    break;
            }
            AllCartesChoisiesString[i] = stringCartei;
        }
        FindLocalPlayer().GetComponent<Player>().CmdSendCards(AllCartesChoisiesString); 
        CartesChoisiesPourEffets = null; 
    }

}
