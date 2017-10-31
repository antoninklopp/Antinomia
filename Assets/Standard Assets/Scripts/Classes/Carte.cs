using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System; 

/// <summary>
/// Classe de base dont héritent toutes les cartes qui en sont dérivées. 
/// Contient les attributs de base communs à toutes les cartes. 
/// 
/// Les cartes filles:
/// <see cref="Sort"/>
/// <see cref="Entite"/>
/// <see cref="Assistance"/>
/// </summary>
public class Carte : NetworkBehaviourAntinomia {
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

    /// <summary>
    /// Quand une pile se délie, les effets des cartes sont executes. 
    /// Dans le cas où cette carte execute un effet, elle peut executer une action, 
    /// cette variable est la pour s'assurer que tous les effets ont bien été tous executes, 
    /// avant de rendre la main à la pile. 
    /// </summary>
    int nombreEffets = 0; 

    /// <summary>
    /// La carte peut-elle attaquer directement le joueur adverse? 
    /// </summary>
    [HideInInspector]
    public bool attaqueDirecte = false; 

    

    // Use this for initialization
    public override void Start () {
        
        Cible = Resources.Load<Sprite>("AutresSprites/hit");
    }
	
	// Update is called once per frame
	public override void Update () {

	}

    /// <summary>
    /// Deplacer la carte. 
    /// La carte suit la souris. 
    /// </summary>
    public void Dragging() {
        /*
		 * Déplacement de la carte qui suit la souris. 
		 */
        Vector3 MousePosition = Input.mousePosition;
        MousePosition.z = 15;
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);
        transform.position = mouseWorldPoint;
    }

    /// <summary>
    /// Mettre la bonne image sur l'objet carte. 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Méthode appelée lorsque la souris entre en contact avec la carte. 
    /// </summary>
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

    /// <summary>
    /// Lorsque la souris est au-dessus de la carte. 
    /// </summary>
    public void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            RightClickOnCarte();
        }
    }

    public virtual void OnMouseDown() {
        if (!isFromLocalPlayer) {
            // return; 
        } 
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().gameIsPaused 
            && !GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().IPausedTheGame) {
            // Pas d'interaction possible, si le joueur n'a pas mis le jeu en pause, mais qu'il est en pause. 
            // DisplayMessage("Votre adversaire a mis le jeu en pause");
            // return; 
        }
    }

    public virtual void checkIfLocalPlayerOnMousEnter() {
        /*
         * Dans les classes enfant cette fonction sert à changer le State en State.ADVERSAIRE
         * dans le cas où la carte n'est pas du joueur local. 
         */ 
    }

    /// <summary>
    /// Lorsque le zoom sur la carte est détruit on fait un effet de dézoom sur la carte normal. 
    /// NON UTILISE DANS LES CHOIX D'AFFICHAGE COURANTS
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Crée une carte zoom. 
    /// NON UTILISE DANS LES CHOIX D'AFFICHAGE COURANTS
    /// </summary>
    /// <param name="messageToDisplay"></param>
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

    /// <summary>
    /// Trouver tous éléments sur le champ de bataille correspondant à un certain élément. 
    /// </summary>
    /// <param name="_element"></param>
    /// <returns>Liste d'objet dont l'élément est égal à l'élément demandé</returns>
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

    /// <summary>
    /// Convertit un string correspondant à la condition et un correspondant à l'entier associé en une condition. 
    /// </summary>
    /// <param name="stringCondition">String qui doit correspondre à un enum de Condition.ConditionEnum</param>
    /// <param name="IntCondition">Entier associé à la condition</param>
    /// <returns>Condition associée</returns>
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

    /// <summary>
    /// Transforme le string donné par la base de données gamesparks 
    /// en un ensemble de conditions nécessaires pour effectuer un effet de la carte
    /// </summary>
    /// <param name="allConditions">string envoyé par la base de données. </param>
    /// <returns>Une liste de conditions. </returns>
    public List<Condition> stringToConditionList(string allConditions) {

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

    /// <summary>
    /// Transforme le string donné par la base de données gamesparks
    /// en un ensemble d'actions effectués par la carte. 
    /// </summary>
    /// <param name="allConditions">string envoyé par la base de données. </param>
    /// <returns>Une liste d'actions. </returns>
    public List<Action> stringToActionList(string allEffets) {

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
                //Debug.Log("Effet 1 : " + stringToAction(AllActionsStringList[i], AllActionsStringList[i + 1]).ActionAction);
            }
        }

        return AllActions;
    }

    /// <summary>
    /// Convertit un string correspondant à la condition et un correspondant à l'entier associé en une action. 
    /// </summary>
    /// <param name="stringCondition">String qui doit correspondre à un enum de Action.ActionEnum</param>
    /// <param name="IntCondition">Entier associé à l'action</param>
    /// <returns>Action associée</returns>
    public Action stringToAction(string stringEffet, string IntEffet) {
        /*
         * Renvoie l'effet associé au string. 
         */
        return new Action((Action.ActionEnum)Enum.Parse(typeof(Action.ActionEnum), value: stringEffet),
            int.Parse(IntEffet));
    }

    /// <summary>
    /// Transforme le string reçu par la base de données en une liste d'effets. 
    /// Ecrit directement dans la variable allEffets.
    /// </summary>
    /// <param name="allEffets">String reçu par la base de données. </param>
    public void stringToEffetList(string allEffets) {
        if (allEffets == "None" || allEffets == "" || allEffets == " "){
            return; 
        }

        string[] AllEffetsStringList = allEffets.Split(':');

        for (int i = 0; i < AllEffetsStringList.Length; ++i) {
            Effet _effet = stringToEffet(AllEffetsStringList[i]);
            AllEffets.Add(_effet);
        }
    }

    /// <summary>
    /// Transform le string reçu par la base de données en un effet qui est constitué d'une liste de conditions 
    /// et d'une liste d'actions. 
    /// </summary>
    /// <param name="_effetString">le string à transformer</param>
    /// <returns>UN effet</returns>
    public Effet stringToEffet(string _effetString) {
        Effet newEffet = new Effet();
        string ConditionList = _effetString.Split('!')[0];
        Debug.Log(Name);
        Debug.Log(_effetString); 
        string ActionList = _effetString.Split('!')[1];

        newEffet.AllActionsEffet = stringToActionList(ActionList);
        newEffet.AllConditionsEffet = stringToConditionList(ConditionList); 

        return newEffet; 
    }

    /// <summary>
    /// Changer la position de la carte zoomée
    /// NON UTILISE DANS LES CHOIX D'AFFICHAGE COURANTS
    /// </summary>
    /// <param name="position">la nouvelle position de la carte</param>
    public void setBigCardPosition(Vector2 position) {
        BigCard.transform.position = position; 
    }

    /// <summary>
    /// Detruire la carte zoomée
    /// NON UTILISE DANS LES CHOIX D'AFFICHAGE COURANTS
    /// </summary>
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

    /// <summary>
    /// Lorsque la carte arrive sur le board ou sur le sanctuaire, 
    /// on regarde si elle n'a pas des effets à jouer. 
    /// Si c'est le cas, ils sont joués. 
    /// </summary>
    public void JouerEffetDeposeCarte() {
        Debug.Log("Les effets sont en cours de traitement"); 
        GererEffets(AllEffets, numeroListEffet:0);
    }

    /// <summary>
    /// A chaque nouvelle phase on vérifie que la carte n'a pas quelque chose à jouer à ce tour. 
    /// </summary>
    /// <param name="_currentPhase">Le nom de la phase en cours</param>
    /// <param name="tour">L'entier du tour en cours. </param>
    public virtual void UpdateNewPhase(Player.Phases _currentPhase, int tour) {

        // A mettre dans entité uniquement. 
        GameManager.AscendanceTerrain currentAscendance = 
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetAscendanceTerrain();

        bool isTourLocalPlayer = (FindLocalPlayer().GetComponent<Player>().PlayerID == tour); 
        if (_currentPhase == Player.Phases.INITIATION) {
            updateNewTurnEffetUtilise(isTourLocalPlayer);
        }

        // Gestion "des effets normaux"
        GererEffets(AllEffets, _currentPhase, nouveauTour:true, numeroListEffet:0);
        
    }

    /// <summary>
    /// Gerer les effets d'une carte
    /// </summary>
    /// <param name="_allEffets">Liste des effets à effectier</param>
    /// <param name="_currentPhase">Phase lors dans laquelle on se trouve</param>
    /// <param name="debut">true, si on la carte vient d'être posée</param>
    /// <param name="Cible">Si c'est un sort qui appelle la méthode, il peut déjà avoir une cible</param>
    /// <param name="nouveauTour">true si on vient de passer à un nouveau tour</param>
    public void GererEffets(List<Effet> _allEffets, Player.Phases _currentPhase=Player.Phases.INITIATION, bool debut=false, 
        bool nouveauTour=false, GameObject Cible=null, int numeroListEffet=0, int deposeCarte=0) {
        if (_allEffets.Count == 0) {
            // La carte n'a aucune capacité/effet lors de tours. 
            return; 
        }
        for (int i = 0; i < _allEffets.Count; ++i) {
            // On regarde les effets un par un. 
            // Si à la fin des conditions effetOk == true, alors on pourra réaliser l'effet.
            bool effetOK = GererConditions(_allEffets[i].AllConditionsEffet, _currentPhase, debut:debut, nouveauTour:nouveauTour,
                                               Cible:Cible, deposeCarte:deposeCarte);
            Debug.Log("<color=orange>" + effetOK.ToString() + "</Color>"); 
            if (effetOK) {
                // Dans le cas où toutes les conditions sont réunies. 
                GererActions(_allEffets[i].AllActionsEffet, CibleDejaChoisie:!(Cible==null), numeroEffet:i, 
                    effetListNumber:numeroListEffet);
            }
            else {
                Debug.Log("<color=orange>L'effet n'a pas pu être joué, pour diverses raisons. </color>" + Name);
            }
        }
    }

    /// <summary>
    /// Lors qu'une carte est détruite, ou que le terrain change d'ascendance, il faut annuler les effets qui
    /// avaient lieu précedemment. 
    /// </summary>
    /// <param name="_allEffets">Liste des effets à annuler.</param>
    public void AnnulerEffets(List<Effet> _allEffets) {
        for (int i = 0; i < _allEffets.Count; ++i) {
            // Les conditions sont automatiquement vérifiées!

            AnnulerActions(_allEffets[i].AllActionsEffet); 
        }
    }

    /// <summary>
    /// Annuler les actions d'un joueur
    /// <seealso cref="AnnulerEffets(List{Effet})"/>
    /// </summary>
    /// <param name="_allActions"></param>
    private void AnnulerActions(List<Action> _allActions) {
        for (int i = 0; i < _allActions.Count; ++i) {
            switch (_allActions[i].ActionAction) {
                // On met toutes les actions pour être sûrs de n'en oublier aucune.
                case Action.ActionEnum.NATURE_EAU:
                case Action.ActionEnum.NATURE:
                case Action.ActionEnum.NATURE_AIR:
                case Action.ActionEnum.NATURE_FEU:
                case Action.ActionEnum.NATURE_TERRE:
                    // Ici on reset la nature de l'entité. 
                    GetComponent<Entite>().ResetCarteElement(); 
                    break;
                case Action.ActionEnum.CHANGER_NATURE_AUTRE_ENTITE:
                    break;
                case Action.ActionEnum.PIOCHER_CARTE:
                    break;
                case Action.ActionEnum.DETRUIRE:
                    break;
                case Action.ActionEnum.CHANGER_POSITION:
                    break;
                case Action.ActionEnum.TERRAIN_ASTRAL:
                    // ici le terrain reste à priori astral. 
                    break;
                case Action.ActionEnum.TERRAIN_MALEFIQUE:
                    // ici le terrain reste à priori malefique.
                    break;
                case Action.ActionEnum.PUISSANCE_EAU_AUGMENTE:
                case Action.ActionEnum.PUISSANCE_TERRE_AUGMENTE:
                case Action.ActionEnum.PUISSANCE_AIR_AUGMENTE:
                case Action.ActionEnum.PUISSANCE_FEU_AUGMENTE:
                    break;
                case Action.ActionEnum.PUISSANCE_AUGMENTE:
                case Action.ActionEnum.PUISSANCE_AUGMENTE_DIRECT:
                    // Si on annule une puissance qui augmente, on diminue la puissance
                    Debug.Log("On annule l'effet qui ajoute : " + _allActions[i].properIntAction.ToString() + " à la carte"); 
                    GetComponent<Entite>().CmdAddStat((-1)*_allActions[i].properIntAction); 
                    break;
                case Action.ActionEnum.PUISSANCE_MULTIPLIE:
                    // On divise la puissance de l'entité.
                    GetComponent<Entite>().CmdMultiplierStat(1 / _allActions[i].properIntAction); 
                    break;
                case Action.ActionEnum.GAIN_AKA_UN_TOUR:
                    break;
                case Action.ActionEnum.ATTAQUE_OBLIGATOIRE:
                    break;
                case Action.ActionEnum.SACRIFIER_CARTE:
                    break;
                case Action.ActionEnum.PLACER_SANCTUAIRE:
                    break;
                case Action.ActionEnum.ATTAQUE_DIRECTE:
                    break;
                case Action.ActionEnum.ATTAQUE_IMPOSSIBLE:
                    break;
                case Action.ActionEnum.REVELER_CARTE:
                    break;
                case Action.ActionEnum.REVELER_CARTE_ADVERSAIRE:
                    break;
                case Action.ActionEnum.DEFAUSSER:
                    break;
                case Action.ActionEnum.ATTAQUE:
                    break;
                case Action.ActionEnum.ATTAQUE_JOUEUR_ADVERSE:
                    break;
                case Action.ActionEnum.FORTE_ENTITE:
                    AnnulerForteFaceA(_allActions[i].properIntAction); 
                    break;
                case Action.ActionEnum.PROCURE_AKA_SUPPLEMENTAIRE:
                    break;
                case Action.ActionEnum.NONE:
                    break;
            }
        }
    }

    /// <summary>
    /// Gerer les conditions avant d'executer les actions d'un effet. 
    /// </summary>
    /// <param name="_conditions">Liste des conditions à vérifier</param>
    /// <param name="_currentPhase">Phase dans laquelle on se trouve</param>
    /// <param name="estMort">Si la carte vient d'être détruite</param>
    /// <param name="debut">Si la carte vient d'être posée</param>
    /// <param name="nouveauTour">true lorsque la fonction est appelée lors du passage à un nouveau tour</param>
    /// <returns>true, si toutes les conditions sont remplies</returns>
    /// <param name="Cible">Si c'est un sort qui appelle la méthode, il peut déjà avoir une cible</param>
    /// <param name="deposeCarte">Si on veut gérer les conditions lorsqu'on dépose une carte, 
    /// 0 si on ne depose pas de cartes
    /// 1 si on dépose une carte sur le sanctuaire
    /// 2 si on depose une carte sur le champ de bataille</param>
    /// <param name="choixJoueur">Si le joueur a fait un clic droit sur la carte et veut intentionellement jouer
    /// un effet lié à cette carte</param>
    public bool GererConditions(List<Condition> _conditions, Player.Phases _currentPhase = Player.Phases.INITIATION,
                                    bool estMort = false, bool debut = false, bool nouveauTour = false, GameObject Cible=null, 
                                    int deposeCarte=0, bool choixJoueur=false) {
        /*
         * Regarde si toutes les conditions sont ok pour un effet. 
         * Retourne true si c'est le cas, false sinon
         * 
         */
        bool effetOK = true;
        for (int j = 0; j < _conditions.Count; ++j) {
            //if (!_conditions[j].dependsOnPhase) {
            //    // Si l'effet ne dépend pas d'une phase, il n'y pas de raison
            //    breakAKA
            //}
            Debug.Log(_conditions[j].dependsOnPhase);
            Debug.Log(_currentPhase);
            Debug.Log(_conditions[j].PhaseCondition); 
            if (_conditions[j].dependsOnPhase &&
                _currentPhase != _conditions[j].PhaseCondition) {
                // La condition de phase n'est pas remplie, donc on passe à l'effet suivant 
                Debug.Log("Condition de phase non remplie");
                return false;
            }
            else if (((_conditions[j].TourCondition == Condition.Tour.TOUR_LOCAL) && (
                  GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Tour !=
                  FindLocalPlayer().GetComponent<Player>().PlayerID)) ||
                  (_conditions[j].TourCondition == Condition.Tour.TOUR_NOT_LOCAL)
                  && (FindLocalPlayer().GetComponent<Player>().PlayerID ==
                  GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Tour)) {
                // La condition de tour n'est pas remplie, on passe donc à l'effet suivant. 
                Debug.Log("Pas le bon tour"); 
                return false;
            }
            else if (debut && (_conditions[j].intCondition > 100)) {
                // L'effet dépend donc d'une phase. 
                Debug.Log("Cette effet dépend d'une phase particulière.");
                return false;
            }
            else if (nouveauTour && !_conditions[j].dependsOnPhase) {
                // une carte dont l'effet ne dépend pas de la phase ne peut pas jouer son effet lors d'un nouveau tour.
                Debug.Log("Le tour n'est pas bon.");
                return false;
            } else if (!_conditions[j].ActionObligatoire && !choixJoueur && j == 0) {
                // Si on est pas sur une obligatoire et que le joueur n'a pas choisi de la jouée, on sort de là. 
                // L'action obligatoire ne sera marquée que sur la première carte
                Debug.Log("L'action n'est pas obligatoire" + Name);
                Debug.Log(_conditions[j].intCondition); 
                return false; 
            }
            else if (_conditions[j].utilisePourCeTour) {
                // DisplayMessage("Cet effet a déjà été utilisé pour ce tour. ");
                Debug.Log("Cet effet a déjà été utilisé pour ce tour."); 
                return false;
            } else if (estMort && (_conditions[j].ConditionCondition != Condition.ConditionEnum.MORT)) {
                // Il faudra TOUJOURS mettre la condition de mort en premier dans la liste de conditiosn lors de
                // la mort d'une carte. 
                Debug.Log("La carte n'a pas d'effets de mort"); 
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
                        if (checkCibleNull(Cible)) { 
                            ShowCardsForChoiceChampBatailleDeuxJoueurs();
                        }
                        break;
                    case Condition.ConditionEnum.CHOIX_ENTITE_CHAMP_BATAILLE_ADVERSAIRE:
                        if (checkCibleNull(Cible)) {
                            ShowCardsForChoice(FindNotLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur"));
                        }
                        break;
                    case Condition.ConditionEnum.CHOIX_ENTITE_CHAMP_BATAILLE_JOUEUR:
                        if (checkCibleNull(Cible)) {
                            ShowCardsForChoice(FindLocalPlayer().transform.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur"));
                        }
                        break;
                    case Condition.ConditionEnum.CHOIX_ENTITE_TERRAIN:
                        if (checkCibleNull(Cible)) {
                            ShowCardsForChoiceAllCartesDeuxJoueurs();
                        }
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
                    case Condition.ConditionEnum.PAYER_COUT_ELEMENTAIRE:
                        // Dans le cas où l'on ne dépose pas une carte sur le sanctuaire
                        // Le cout élémentaire ne peut être payé que lorsqu'on dépose une carte dans le sanctuaire. 
                        Debug.Log("Payer cout elementaire" + deposeCarte.ToString()); 
                        if (deposeCarte != 1) {
                            return false; 
                        } else {
                            // Sinon il faut regarder dans la pile s'il n'y a pas une carte qui veut se déplacer dans le sanctuaire
                            if (!CheckIfCartePayeCoutElementaire(_conditions[j].properIntCondition)) {
                                return false; 
                            }
                        } 
                        break;
                    case Condition.ConditionEnum.SACRIFIER_CARTE:
                        // Lorsque la condition est le sacrifice d'une carte. 
                        // Dans le cas du sacrifice d'une carte, il faut rajouter dans les effets le sacrifice de la carte. 
                        Debug.Log("Une carte est sacrifiée, condition"); 
                        // il faudra vérifier si la carte peut être sacrifiée. 
                        break;
                    case Condition.ConditionEnum.CARTE_SUR_CHAMP_BATAILLE:
                        // Si la carte n'est pas sur le champ de bataille on s'arrête. 
                        if (GetComponent<Entite>().carteState != Entite.State.CHAMPBATAILLE) {
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
    /// Gerer les conditions pour regarder si une carte peut executer un effet. 
    /// </summary>
    /// <param name="_conditions">Liste des conditions à vérifier</param>
    /// <param name="_currentPhase">Phase dans laquelle on se trouve</param>
    /// <param name="estMort">Si la carte vient d'être détruite</param>
    /// <param name="debut">Si la carte vient d'être posée</param>
    /// <param name="nouveauTour">true lorsque la fonction est appelée lors du passage à un nouveau tour</param>
    /// <returns>true, si toutes les conditions sont remplies</returns>
    /// <param name="Cible">Si c'est un sort qui appelle la méthode, il peut déjà avoir une cible</param>
    public bool GererConditionsRechercheCarte(List<Condition> _conditions, Player.Phases _currentPhase = Player.Phases.INITIATION,
                                    bool estMort = false, bool debut = false, bool nouveauTour = false, GameObject Cible = null) {
        /*
         * Regarde si toutes les conditions sont ok pour un effet. 
         * Retourne true si c'est le cas, false sinon
         * 
         */
        bool effetOK = true;
        for (int j = 0; j < _conditions.Count; ++j) {

            if (_conditions[j].dependsOnPhase &&
                _currentPhase != _conditions[j].PhaseCondition) {
                // La condition de phase n'est pas remplie, donc on passe à l'effet suivant 
                Debug.Log("La phase n'est pas la bonne"); 
                return false;
            }
            else if (((_conditions[j].TourCondition == Condition.Tour.TOUR_LOCAL) && (
                  GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Tour !=
                  FindLocalPlayer().GetComponent<Player>().PlayerID)) ||
                  (_conditions[j].TourCondition == Condition.Tour.TOUR_NOT_LOCAL)
                  && (FindLocalPlayer().GetComponent<Player>().PlayerID ==
                  GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Tour)) {
                // La condition de tour n'est pas remplie, on passe donc à l'effet suivant. 
                Debug.Log("La condition de Tour n'est pas bonne"); 
                return false;
            }
            else if (debut && (_conditions[j].intCondition > 100)) {
                // L'effet dépend donc d'une phase. 
                Debug.Log("Cette effet dépend d'une phase particulière.");
                return false;
            }
            else if (nouveauTour && !_conditions[j].dependsOnPhase) {
                // une carte dont l'effet ne dépend pas de la phase ne peut pas jouer son effet lors d'un nouveau tour.
                Debug.Log("Le tour n'est pas bon.s"); 
                return false;
            }
            else if (_conditions[j].utilisePourCeTour) {
                Debug.Log("Deja utilise pour ce Tour"); 
                return false;
            }
            else if (estMort && (_conditions[j].ConditionCondition != Condition.ConditionEnum.MORT)) {
                // Il faudra TOUJOURS mettre la condition de mort en premier dans la liste de conditiosn lors de
                // la mort d'une carte. 
                Debug.Log("La carte n'a pas de condition de mort"); 
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
                    case Condition.ConditionEnum.CHOIX_ENTITE_CHAMP_BATAILLE:
                    case Condition.ConditionEnum.CHOIX_ENTITE_CHAMP_BATAILLE_ADVERSAIRE:
                    case Condition.ConditionEnum.CHOIX_ENTITE_CHAMP_BATAILLE_JOUEUR:
                    case Condition.ConditionEnum.CHOIX_ENTITE_TERRAIN:
                    case Condition.ConditionEnum.DEFAUSSER:
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
    /// <param name="effetListNumber">Utile pour mettre à jour, si un effet a été utilisé ou pas dans les conditions. 
    /// Si égal à 0, on est dans la liste d'effets normale, si = 1, effets astraux, si = à 2, effets maléfiques</param> 
    /// <param name="numeroEffet">Le numero de l'effet dans la liste des effets.</param>
    /// <param name="jouerEffet">Si  true, alors on est en train de défaire la pile d'effet. On execute donc vraiment les effets
    /// Sinon on les ajoute juste à la pile. </param>
    /// <param name="CibleDejaChoisie">true si les paramètres ont été choisis en amont, par exemple pour un sort</param>
    public void GererActions(List<Action> _actions, int numeroEffet=0, int effetListNumber=0, bool jouerEffet=false, 
                                bool CibleDejaChoisie=false) {
        // On doit maintenant gérer les effets.
        for (int j = 0; j < _actions.Count; ++j) {
            // On est obliger de regarder la condition jouerEffet à l'intérieur du switch parce que 
            // certaines méthodes se font uniquement par cartes spécifiques. 
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
                    if (jouerEffet) {
                        StartCoroutine(ChangerPositionEffet(CibleDejaChoisie));
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, CibleDejaChoisie, effetListNumber));
                    }
                    break;
                case Action.ActionEnum.DETRUIRE:
                    /*
                        * Il faut d'abord choisir une carte puis la détruire.
                        */
                    DisplayMessage("Choisissez une carte et détruisez la");
                    if (jouerEffet) {
                        StartCoroutine(DetruireEffet(CibleDejaChoisie));
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, CibleDejaChoisie, effetListNumber)); 
                    }
                    break;
                case Action.ActionEnum.GAIN_AKA_UN_TOUR:
                    if (jouerEffet) {
                        FindLocalPlayer().GetComponent<Player>().subtractAKA(-_actions[j].intAction);
                        DisplayMessage("Ajout de " + _actions[j].intAction.ToString() + "AKA à ce tour");
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, CibleDejaChoisie, effetListNumber));
                    }
                    break;
                case Action.ActionEnum.PIOCHER_CARTE:
                    StartCoroutine(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PiocheMultiple(_actions[j].properIntAction));
                    DisplayMessage("Pioche " + _actions[j].properIntAction.ToString() +  " carte");
                    Debug.Log("<color=purple> Piocher une carte </color>"); 
                    break;
                case Action.ActionEnum.SACRIFIER_CARTE:
                    // Sacrifier cette carte. 
                    DisplayMessage("<color=orange>Sacrifice de cette carte</color>");
                    if (jouerEffet) {
                        SacrifierCarteEntite();
                    } else {
                        InformerSacrifierCarteEntite(); 
                    }
                    break;
                case Action.ActionEnum.PLACER_SANCTUAIRE:
                    if (jouerEffet) {
                        DisplayMessage("On place cette carte dans le sanctuaire");
                        PlacerSanctuaire();
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, CibleDejaChoisie, effetListNumber));
                    }
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
                    //if (jouerEffet) {
                    if (GetComponent<Entite>().hasAttacked != 1) {
                        GetComponent<Entite>().hasAttacked = -1;
                        DisplayMessage("Cette entité ne peut pas attaquer");
                    }
                    //} else {
                    //    StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, CibleDejaChoisie, effetListNumber));
                    //}
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
                case Action.ActionEnum.FORTE_ENTITE:
                     // si une entité est forte face à l'entité d'un autre joueur
                    CarteForteFaceA(_actions[j].properIntAction); 
                    break;
                 case Action.ActionEnum.PROCURE_AKA_SUPPLEMENTAIRE:
                    // Une carte peut procurer de l'AKA supplémentaire.
                    Debug.Log("Cette carte procure un AKA supplémentaire"); 
                    FindLocalPlayer().GetComponent<Player>().addAKA(_actions[j].properIntAction); 
                    break;
                case (Action.ActionEnum.NATURE_AIR):
                    if (!jouerEffet) {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, CibleDejaChoisie, effetListNumber));
                    } else {
                        StartCoroutine(ChangerNatureEffet(Entite.Element.AIR, CibleDejaChoisie));
                    }
                    Debug.Log("Element de la carte changé en AIR");
                    break;
                case (Action.ActionEnum.NATURE_EAU):
                    if (!jouerEffet) {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, CibleDejaChoisie, effetListNumber));
                    } else {
                        StartCoroutine(ChangerNatureEffet(Entite.Element.EAU, CibleDejaChoisie));
                    }
                    Debug.Log("Element de la carte changé en EAU");
                    break;
                case (Action.ActionEnum.NATURE_FEU):
                    if (!jouerEffet) {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, CibleDejaChoisie, effetListNumber));
                    } else {
                        StartCoroutine(ChangerNatureEffet(Entite.Element.FEU, CibleDejaChoisie));
                    }
                    Debug.Log("Element de la carte changé en FEU");
                    break;
                case (Action.ActionEnum.NATURE_TERRE):
                    if (!jouerEffet) {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, CibleDejaChoisie, effetListNumber));
                    } else {
                        StartCoroutine(ChangerNatureEffet(Entite.Element.TERRE, CibleDejaChoisie));
                    }
                    Debug.Log("Element de la carte changé en TERRE"); 
                    break;
                case Action.ActionEnum.PUISSANCE_AIR_AUGMENTE:
                    // La puissance de toutes les cartes air augmente
                    if (jouerEffet) {
                        Debug.Log("Effet PUISSANCE AIR AUGMENTE");
                        ChangeEffetPuissance(Entite.Element.AIR, _actions[j].properIntAction);
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, true, numeroListEffet: effetListNumber)); 
                    }
                    break;
                case Action.ActionEnum.PUISSANCE_TERRE_AUGMENTE:
                    // La puissance de toutes les cartes terre augmente
                    if (jouerEffet) {
                        Debug.Log("Effet PUISSANCE TERRE AUGMENTE");
                        ChangeEffetPuissance(Entite.Element.TERRE, _actions[j].properIntAction);
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, true, numeroListEffet: effetListNumber));
                    }
                    break;
                case Action.ActionEnum.PUISSANCE_EAU_AUGMENTE:
                    // La puissance de toutes les cartes eau augmente
                    if (jouerEffet) {
                        Debug.Log("Effet PUISSANCE EAU AUGMENTE");
                        ChangeEffetPuissance(Entite.Element.EAU, _actions[j].properIntAction);
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, true, numeroListEffet: effetListNumber));
                    }
                    break;
                case Action.ActionEnum.PUISSANCE_FEU_AUGMENTE:
                    // La puissance de toutes les cartes feu augmente
                    if (jouerEffet) {
                        Debug.Log("Effet PUISSANCE FEU AUGMENTE");
                        ChangeEffetPuissance(Entite.Element.FEU, _actions[j].properIntAction);
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, true, numeroListEffet: effetListNumber));
                    }
                    break;
                case Action.ActionEnum.PUISSANCE_AUGMENTE:
                    // La puissance de cette carte augmente (uniquement cette carte). 
                    if (jouerEffet) {
                        GetComponent<Entite>().CmdAddStat(_actions[j].properIntAction); 
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, true, numeroListEffet: effetListNumber));
                    }
                    break;
                case Action.ActionEnum.PUISSANCE_AUGMENTE_DIRECT:
                    // La puissance augmente directement, l'autre joueur ne peut pas répliquer à cette action
                    GetComponent<Entite>().CmdAddStat(_actions[j].properIntAction);
                    break; 
                case Action.ActionEnum.TERRAIN_ASTRAL:
                    // On change l'ascendance du terrain en astral. 
                    if (jouerEffet) {
                        getGameManager().GetComponent<GameManager>().SetAscendanceTerrain(GameManager.AscendanceTerrain.ASTRALE);
                    } else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, true, numeroListEffet: effetListNumber)); 
                    }
                    break;
                case Action.ActionEnum.TERRAIN_MALEFIQUE:
                    // On change l'ascendance du terrain en maléfique. 
                    if (jouerEffet) {
                        getGameManager().GetComponent<GameManager>().SetAscendanceTerrain(GameManager.AscendanceTerrain.MALEFIQUE); 
                    }
                    else {
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, true, numeroListEffet: effetListNumber));
                    }
                    break;
                case Action.ActionEnum.ATTAQUE_JOUEUR_ADVERSE:
                    if (jouerEffet) {
                        Debug.Log("<color=green>CA PART222</color>");
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Attack(true, gameObject, FindNotLocalPlayer(),
                        _actions[j].properIntAction);
                    } else {
                        Debug.Log("<color=green>CA PART</color>"); 
                        StartCoroutine(MettreEffetDansLaPileFromActions(numeroEffet, true, numeroListEffet: effetListNumber));
                    }
                    break; 
                default:
                    Debug.LogWarning("Cet effet n'est pas géré");
                    break;
                }
            updateEffetActive(numeroEffet, effetListNumber); 
        }
    }

    /// <summary>
    /// Gérer les effets à la mort de la carte.
    /// </summary>
    public void GererEffetsMort() {

        bool effetOK = false; 

         for (int i = 0; i < AllEffets.Count; ++i) {
            effetOK = false; 
            for (int j = 0; j < AllEffets[i].AllConditionsEffet.Count; ++j) {
                if (AllEffets[i].AllConditionsEffet[j].ConditionCondition == Condition.ConditionEnum.MORT) {
                    effetOK = GererConditions(AllEffets[i].AllConditionsEffet, estMort: true); 
                }
            }
            if (effetOK) {
                GererActions(AllEffets[i].AllActionsEffet); 
            }
         }
    }

    /// <summary>
    /// Coroutine pour gérer les effets lorsqu'une pile d'effets se defait. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator GererActionsCoroutine(List<Action> _actionsCarte, int numeroEffet, int effetListNumber, 
                                                List<GameObject> Cibles=null) {
        // nombreEffets = _actionsCarte.Count;
        Debug.Log("On gère les actions");
        CartesChoisiesPourEffets = Cibles;

        GererActions(_actionsCarte, numeroEffet, effetListNumber, jouerEffet:true, CibleDejaChoisie:true);

        yield break; 
        //while (nombreEffets != 0) {
        //    yield return new WaitForSeconds(0.05f); 
        //}
    }

    /// <summary>
    /// EFFET: Sacrifier une carte entité.
    /// Override dans la classe entité
    /// </summary>
    public virtual void SacrifierCarteEntite() {
        // Les seules cartes que l'on peut sacrifier sont des entités. 
        // La méthode est donc override dans la classe entité. 
    }

    /// <summary>
    /// EFFET: Informer que le joueur va sacrifier une entite
    /// Override dans la classe entité. 
    /// </summary>
    public virtual void InformerSacrifierCarteEntite() {


    }

    /// <summary>
    /// EFFET: Placer une carte dans le sanctuaire. 
    /// Override dans la classe entité.
    /// </summary>
    public virtual void PlacerSanctuaire() {
        // Les seuls cartes quel'on peut sacrifier sont des entités. 
        // La méthode est donc override dans la classe entité. 
    }

    /// <summary>
    /// On regarde si une carte est en train de payer un coût élémentaire i.e. elle etst en train d'être 
    /// invoquée dans le sanctuaire. 
    /// </summary>
    /// <param name="element">l'élément dont on recherche une carte qui paye un coût élémentaire</param>
    /// <returns>true, si un élément vient de payer ce coût élémentaire, false sinon.</returns>
    protected virtual bool CheckIfCartePayeCoutElementaire(int element) {
        return false;
    }
    
    /// <summary>
    /// Ajouter dans la liste des éléments si les cartes sont fortes face à un autre élément ou pas. 
    /// </summary>
    /// <param name="element">L'élément contre lequel la carte est forte. </param>
    protected virtual void CarteForteFaceA(int element) {

    }
    
    /// <summary>
    /// On annule le fait qu'une carte soit forte face à une autre. 
    /// TODO : La question est de savoir s'il faut rétablir sa précédente force. 
    /// Pour l'instant on part du principe que non. 
    /// </summary>
    /// <param name="element"></param>
    protected virtual void AnnulerForteFaceA(int element) {

    }

    /// <summary>
    /// Lorsqu'il faut choisir des cartes pour un effet, cette méthode est appelée 
    /// pour stocker les cartes choisies dans la variable CartesChoisiesPourEffet. 
    /// </summary>
    /// <param name="AllIDCartesChoosen">Liste des IDCardGame des cartes choisies.</param>
    public void CartesChoisies(List<int> AllIDCartesChoosen) {
        List<GameObject> _allObjectsChoosen = new List<GameObject>();
        for (int i = 0; i < AllIDCartesChoosen.Count; ++i) {
            _allObjectsChoosen.Add(FindCardWithID(AllIDCartesChoosen[i])); 
        }
        CartesChoisiesPourEffets = _allObjectsChoosen; 
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

    /// <summary>
    /// Montrer au joueur les cartes qu'il peut choisir sur les champs de bataille des deux joueurs. 
    /// </summary>
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

    /// <summary>
    /// Montrer au joueur les cartes qu'il peut choisir parmi toutes les cartes (sur le champ de bataille
    /// ou sur la sanctuaire) des deux joueurs. 
    /// </summary>
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

    /// <summary>
    /// Permet de montrer un message sur l'écran, 
    /// notamment pour informer le joueur d'un effet qui vient de se passer
    /// tant qu'il n'en a pas la preuve visuelle.
    /// </summary>
    /// <param name="Message">Le messafe à afficher</param>
    public void DisplayMessage(string Message) {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().DisplayMessage(Message);
    }

    /// <summary>
    /// Récupérer les informations essentielles sur la carte.
    /// </summary>
    public virtual string GetInfoCarte() {
        return ""; 
    }

    /// <summary>
    /// Override de la méthode toString. 
    /// Elle renvoie maintenant <see cref="GetInfoCarte"/>
    /// </summary>
    /// <returns>Les informations essentielles de la carte</returns>
    public override string ToString() {
        return GetInfoCarte(); 
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

    /// <summary>
    /// Propose à l'autre joueur de mettre le jeu en pause. 
    /// </summary>
    public void ProposerMettreJeuEnPause() {
        StartCoroutine(GameObject.Find("GameManager").GetComponent<GameManager>().ProposeToPauseGame()); 
    }

    /// <summary>
    /// Lorsqu'on détecte un clic droit, on permet au joueur de jouer l'effet de la carte. 
    /// </summary>
    protected virtual void RightClickOnCarte() {
#if (!UNITY_ANDROID || !UNITY_IOS)
        // Dans le cas d'un clic droit. 
        Debug.Log("On check" + Name);
        int effetPropose = CheckForEffet(); 
        if (effetPropose != -1) {
            ProposerEffets(effetPropose);
        }
#endif
    }

    /// <summary>
    /// Regarde si une carte peut jouer un de ses effets.
    /// </summary>
    /// <returns>Le numérode l'effet en question. -1 sinon. </returns>
    public virtual int CheckForEffet() {
        return -1; 
    }

    /// <summary>
    /// Proposer au joueur de jouer des effets. 
    /// TODO: Pour l'instant on ne peut proposer qu'un seul effet. Il faut pouvoir choisir entre plusieurs effets, si
    /// le joueur peut potentiellement en jouer plusieurs. 
    /// </summary>
    /// <param name="effetPropose">Numero de l'effet proposé</param>
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

    /// <summary>
    /// Attendre la réponse du joueur après qu'on lui ait proposé de jouer un effet.
    /// </summary>
    /// <param name="effetPropose">Numéro de l'effet proposé</param>
    /// <returns></returns>
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

    /// <summary>
    /// EFFET: Changer la position d'une entité. 
    /// TODO: Gérer le changement de position de plusieurs entités. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangerPositionEffet(bool CibleDejaChoisie = false) {

        if (!CibleDejaChoisie) {
            // Si les cibles n'ont pas déjà été choisies
            yield return WaitForCardsChosen();
        }

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

    /// <summary>
    /// Changer l'élément d'une entité
    /// </summary>
    /// <param name="newElement">Le nouvel élément de l'entité</param>
    /// <param name="CibleDejaChoisie">true si on a déjà choisie la cible (utilisation d'un sort pas exemple)</param>
    /// <returns>None</returns>
    private IEnumerator ChangerNatureEffet(Entite.Element newElement, bool CibleDejaChoisie = false) {

        if (!CibleDejaChoisie) {
            // Si les cibles n'ont pas déjà été choisies
            yield return WaitForCardsChosen();
        }

        if (CartesChoisiesPourEffets.Count != 1) {
            throw new Exception("Erreur dans la transmission des cartes. Cet effet ne permet pas encore" +
                "de gérer plusieurs changements de position. ");
        }
        else {
            CartesChoisiesPourEffets[0].GetComponent<Entite>().CmdChangeElement(newElement, true);
            Debug.Log("L'effet changement de position a été autorisé");
        }
        CartesChoisiesPourEffets = null; 

    }

    /// <summary>
    /// EFFET: Détruire une ou plusieurs cartes. 
    /// </summary>
    /// <returns></returns>
    /// <param name="CibleDejaChoisie">true si les cartes sur lesquelles s'applique l'effet ont déjà été choisies
    /// précedemment</param>
    private IEnumerator DetruireEffet(bool CibleDejaChoisie=false) {
        Debug.Log("DetruireEffet");
        
        if (!CibleDejaChoisie) {
            // Si les cibles n'ont pas déjà été choisies
            yield return WaitForCardsChosen();
        }

        for (int i = 0; i < CartesChoisiesPourEffets.Count; ++i) {
            // On détruit toutes les cartes une par une. 
            Debug.Log("Je détruis la carte");
            Debug.Log(CartesChoisiesPourEffets[i].GetComponent<Entite>().Name); 
            CartesChoisiesPourEffets[i].SendMessage("DetruireCarte");
        }

        CartesChoisiesPourEffets = null; 
    }

    /// <summary>
    /// EFFET: Ajout de l'effet détruire dans la pile d'effet.
    /// On informe l'autre joueur qu'on a joué cet effet.  
    /// </summary>
    /// <param name="CibleDejaChoisie">true si les cartes sur lesquelles s'applique l'effet ont déjà été choisies
    /// précedemment</param>
    /// <param name="numeroEffet">numéro de l'effet dans la liste des effets</param>
    /// <param name="numeroListEffet">numero de la liste des effets (facultatif)</param>
    /// <returns></returns>
    private IEnumerator MettreEffetDansLaPileFromActions(int numeroEffet, bool CibleDejaChoisie = false, int numeroListEffet = 0) {
        Debug.Log(CibleDejaChoisie);
        if (!CibleDejaChoisie) {
            yield return WaitForCardsChosen(); 
        }
        Debug.Log("Nombre de cartes choisies : " + CartesChoisiesPourEffets.Count.ToString());
        MettreEffetDansLaPile(CartesChoisiesPourEffets, numeroEffet, numeroListEffet); 
    }

    /// <summary>
    /// EFFET: Defausser une ou plusieurs cartes.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DefausserEffet(bool CibleDejaChoisie = false) {
        if (!CibleDejaChoisie) {
            // Si les cibles n'ont pas déjà été choisies
            yield return WaitForCardsChosen();
        }
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

    /// <summary>
    /// Certains effets ne pouvant être utilisés qu'une fois par tour, 
    /// on doit écrire dans les conditions qu'ils ont déjà été utilisés. 
    /// </summary>
    protected virtual void updateEffetActive(int nombreEffet, int effetListNumber=0) {
        Debug.Log(effetListNumber);
        Debug.Log(nombreEffet);
        switch (effetListNumber) {
            case 0:
                // Dans le cas où il y a plusieurs conditions, on écrit toujours toutes les variables de timing dans l'int
                // de la première condition
                AllEffets[nombreEffet].AllConditionsEffet[0].setSortUtilisePourCeTour();
                break;
        }
    }

    /// <summary>
    /// Lors de chaque nouveau tour on regarde si les effets peuvent être réinitialisés. 
    /// </summary>
    /// <param name="tourJoueurLocal">true, si c'est le tour du joueur local</param>
    protected virtual void updateNewTurnEffetUtilise(bool tourJoueurLocal) {
        for (int i = 0; i < AllEffets.Count; ++i) {
            AllEffets[i].AllConditionsEffet[0].updateUtilisePourCeTour(tourJoueurLocal); 
        }
    }

    /// <summary>
    /// Rajouter un effet dans la pile.
    /// Crée la pile si elle n'existe pas. 
    /// </summary>
    /// <param name="Cibles">Les Cibles de l'effet</param>
    /// <param name="numeroEffet">La position de l'effet dans la liste d'effets</param>
    /// <param name="numeroListEffet">Le numero de la liste d'effets</param>
    protected void MettreEffetDansLaPile(List<GameObject> Cibles, int numeroEffet, int numeroListEffet=0) {
        Debug.Log("Effet dans la pile"); 
        GameObject Pile = GameObject.FindGameObjectWithTag("Pile"); 
        if (Pile == null) {
            // C'est le premier effet de la pile, il faut donc instancier la pile. 
            FindLocalPlayer().GetComponent<Player>().CmdCreerPile();
            StartCoroutine(MettreEffetDansLaPileRoutine(Cibles, numeroEffet, numeroListEffet));
        } else {
            // La pile existe déjà, on peut rajouter à la pile. 
            Pile.GetComponent<PileAppelEffet>().AjouterEffetALaPile(gameObject, Cibles, numeroEffet, numeroListEffet);
            Debug.Log("Un effet a été ajouté à la pile"); 
        }

    }

    /// <summary>
    /// Rajouter un effet dans la pile.
    /// Ceci est le premier effet ajouté à la pile. 
    /// </summary>
    /// <param name="Cibles">Les Cibles de l'effet</param>
    /// <param name="numeroEffet">La position de l'effet dans la liste d'effets</param>
    /// <param name="numeroListEffet">Le numero de la liste d'effets</param>
    protected IEnumerator MettreEffetDansLaPileRoutine(List<GameObject> Cibles, int numeroEffet, int numeroListEffet = 0) {
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

        Pile.GetComponent<PileAppelEffet>().AjouterEffetALaPile(gameObject, Cibles, numeroEffet, numeroListEffet);
        Debug.Log("Un effet a été ajouté à la pile"); 
    }

    /// <summary>
    /// Lorsqu'on appelle la méthode GérerEffet, elle peut être appelée par un sort qui a déjà choisi sa cible. 
    /// true si la Cible est null, false sinon
    /// Dans le cas où la cible n'est pas nulle, on l'ajoute dans les cartes choisies. 
    /// </summary>
    /// <returns>true si la Cible est null, false sinon</returns>
    private bool checkCibleNull(GameObject Cible) {
        if (Cible == null) {
            return true; 
        } else {
            CartesChoisiesPourEffets = new List<GameObject>();
            CartesChoisiesPourEffets.Add(Cible);
            return false; 
        }
    }
    
    /// <summary>
    /// Si la carte peut jouer un effet, on lui met un filtre bleu. 
    /// </summary>
    /// <param name="_currentPhase">Phase en train d'être jouée</param>
    public virtual void CartePeutJouer(Player.Phases _currentPhase) {
        if (CarteJouerReponseEffet(_currentPhase)) {
            setColor(Color.blue); 
        }
    }

    /// <summary>
    /// La carte peut-elle être jouée en réponse à un effet.
    /// </summary>
    /// <returns>toujours false, la méthode sera override dans les calsses filles. </returns>
    protected virtual bool CarteJouerReponseEffet(Player.Phases _currentPhase, int numeroListe = 0) {
        for (int i = 0; i < AllEffets.Count; ++i) {
            // On regarde les effets un par un. 
            // Si à la fin des conditions effetOk == true, alors on pourra réaliser l'effet.
            bool effetOK = GererConditionsRechercheCarte(AllEffets[i].AllConditionsEffet, _currentPhase);
            if (effetOK && CheckIfActionReponseInAction(AllEffets[i].AllActionsEffet)) {
                return true;
            }
        }
        return false; 
    }

    protected bool CheckIfActionReponseInAction(List<Action> _allActions) {

        for (int i = 0; i < _allActions.Count; i++) {
            switch (_allActions[i].ActionAction) {
                // Liste de tous les effets qui permettent une réponse. 
                case Action.ActionEnum.DETRUIRE:
                case Action.ActionEnum.PUISSANCE_AIR_AUGMENTE:
                case Action.ActionEnum.PUISSANCE_EAU_AUGMENTE:
                case Action.ActionEnum.PUISSANCE_TERRE_AUGMENTE:
                case Action.ActionEnum.PUISSANCE_FEU_AUGMENTE:
                case Action.ActionEnum.PUISSANCE_MULTIPLIE:
                case Action.ActionEnum.TERRAIN_MALEFIQUE:
                case Action.ActionEnum.TERRAIN_ASTRAL:
                case Action.ActionEnum.PUISSANCE_AUGMENTE:
                    return true; 
            }
        }
        return false; 
    }

    /// <summary>
    /// Changer la couleur de la carte (en gros lui mettre un filtre)
    /// Pour montrer au joueur qu'un effet est en cours sur cette carte. 
    /// </summary>
    /// <param name="color">La couleur qu'on veut donner à la carte.</param>
    public void setColor(Color color) {
        GetComponent<SpriteRenderer>().color = color; 
    }

    /// <summary>
    /// Remettre la couleur/filtre de la carte à blanc. 
    /// </summary>
    public void resetColor() {
        GetComponent<SpriteRenderer>().color = Color.white; 
    }

    /// <summary>
    /// Il se peut qu'après certains évênements tels que la pose d'une carte, 
    /// on veuille vérifier si une carte n'a pas d'effets à jouer. 
    /// Dans ce cas là, on appelle cette méthode. 
    /// Cette méthode est override dans la classe Entite <seealso cref="Entite.GererEffetsPonctuel"/>
    /// </summary>
    public virtual void GererEffetsPonctuel(Player.Phases phase = Player.Phases.INITIATION, bool debut=false) {
        GererEffets(AllEffets, _currentPhase:phase, debut:debut, numeroListEffet:0); 
    }

    /// <summary>
    /// Changer la puissance de TOUTES LES CARTES
    /// </summary>
    /// <param name="_element"></param>
    /// <param name="_intToAdd"></param>
    void ChangeEffetPuissance(Entite.Element _element, int _intToAdd) {
        /*
         * Changer la puissance de toutes les cartes d'un certain élément. 
         * A AJOUTER DANS GERER ACTIONS. 
         */
        // La puissance de toutes les entités air sur le champ de bataille augmente de intEffet. 
        List<GameObject> carteElement = FindAllEntiteChampBatailleElement(_element);
        Debug.Log("Changer effet puissance" + carteElement.Count.ToString());
        for (int i = 0; i < carteElement.Count; ++i) {
            Debug.Log("Une carte affectée");
            Entite _carte = carteElement[i].GetComponent<Entite>();
            if (_carte.isFromLocalPlayer) {
                // on peut directement changer la STAT depuis la carte. 
                _carte.CmdAddStat(_intToAdd);
            }
            else {
                FindLocalPlayer().GetComponent<Player>().CmdEnvoiMethodToServerCarteWithIntParameter(_carte.IDCardGame,
                    "CmdAddStat", _intToAdd, FindLocalPlayer().GetComponent<Player>().PlayerID);
            }
        }
    }

    /// <summary>
    /// Repositionner une carte avec un effet sur le board. 
    /// </summary>
    /// <param name="speed">La vitesse de repositionnement de la carte, nombre de pas de 0.05f</param>
    /// <param name="newPosition">La nouvelle position de la carte</param>
    public IEnumerator RepositionnerCarteRoutine(int speed, Vector3 newPosition) {
        Vector3 currentPosition = NetworkBehaviourAntinomia.Copy(transform.localPosition);
        int stepCount = 0; 
        Vector3 stepVector = new Vector3(-(currentPosition.x - newPosition.x)/speed,
                                          -(currentPosition.y - newPosition.y) / speed,
                                          -(currentPosition.z - newPosition.z) / speed);

        Debug.Log(stepVector);
        Debug.Log(newPosition);

        while (stepCount < speed) {
            transform.localPosition = new Vector3(transform.localPosition.x + stepVector.x,
                                             transform.localPosition.y + stepVector.y, 
                                             transform.localPosition.z + stepVector.z);
            yield return new WaitForSeconds(0.05f);
            stepCount++; 
        }
    }

    public void RepositionnerCarte(int speed, Vector3 newPosition) {
        StartCoroutine(RepositionnerCarteRoutine(speed, newPosition)); 
    }

    /// <summary>
    /// Renvoie true si la carte est dans la main, false sinon. 
    /// </summary>
    /// <returns>false par défaut</returns>
    public virtual bool isCarteInMain() {
        return false; 
    }

}
