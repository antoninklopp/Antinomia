using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using AntinomiaException; 

/// <summary>
/// Classe pour modéliser un sort. 
/// Hérite de la classe carte. 
/// </summary>
public class Sort : Carte, ICarte {

     /// <summary>
     /// Niveau du sort. 
     /// Pour pouvoir être joué, un sort doit avoir un niveau inférieur ou égal à l'AKA rémanent calculé au début du tour.
     /// </summary>
    public int Niveau;
    public string ConditionSort;
    public int CoutAKA;

    private bool dragging;

    private Vector3 positionBeforeDragging;

    /// <summary>
    /// Constructeur de la classe 
    /// </summary>
    public Sort() {


    }

    /// <summary>
    /// Création d'un sort. 
    /// On essaiera d'instancier les sorts avec cette définition de classe pour pouvoir 
    /// appliquer les méthodes sur les string des effets et des conditions.
    /// </summary>
    /// <param name="_shortCode"></param>
    /// <param name="_Name"></param>
    /// <param name="_description"></param>
    /// <param name="_Niveau"></param>
    /// <param name="_Condition"></param>
    /// <param name="_Effet"></param>
    public Sort(string _shortCode, string _Name, string _description, int _Niveau, string _Condition, 
        string _Effet) {

        shortCode = _shortCode;
        Name = _Name;
        description = _description;
        Niveau = _Niveau;
        AllEffetsString = _Effet;
        stringToEffetList(_Effet);

    }


    public int coutCarte; 

    // Contrairement à une carte "normale", on peut avoir plusieurs clics différents
    int clicked = 0;

    Ray ray;
#pragma warning disable CS0169 // Le champ 'Sort.CarteCibleeRayCast' n'est jamais utilisé
    RaycastHit CarteCibleeRayCast;
#pragma warning restore CS0169 // Le champ 'Sort.CarteCibleeRayCast' n'est jamais utilisé
    GameObject CarteCiblee;

    /// <summary>
    /// "Etat de la carte"
    /// </summary>
    public enum State {
        /// <summary>
        /// La carte est dans le deck. 
        /// </summary>
        DECK,
        /// <summary>
        /// La carte est dans la main
        /// </summary>
        MAIN,
        /// <summary>
        /// La carte a été jouée.
        /// </summary>
        JOUE,
        /// <summary>
        /// La carte est zoomée. 
        /// </summary>
        BIGCARD,
        /// <summary>
        /// La carte est à l'adversaire
        /// </summary>
        ADVERSAIRE, 
        /// <summary>
        /// La carte est au cimetière.
        /// </summary>
        CIMETIERE
    };

    /// <summary>
    /// Etat courant de la carte. 
    /// </summary>
    public State sortState = State.MAIN; 
    
    /// <summary>
    /// Appelé lors du spawn du sort. 
    /// </summary>
    public override void Start() {
        base.Start(); 
        localScaleCard = Mathf.Abs(transform.localScale.x);
        canGoBig = true;

        Def = new SortDefinition(); 

        if (GetComponent<RectTransform>() == null) {
            // Si l'element est UI pas besoin de setUp la carte
            StartCoroutine(SetUpCard());
        }
    }

    public override void Update() {
        base.Update();
        if (clicked != 0) {
            Dragging(); 
        } 
    }


    /// <summary>
    /// Lors d'un clic sur la carte. 
    /// </summary>
    public override void OnMouseDown() {
        /*
         * Lors d'un clic sur la carte
         */

        // TODO : Empecher les interactions quand elles doivent être empechees. 
        base.OnMouseDown();

        if (!isFromLocalPlayer && sortState == State.MAIN) {
            return;
        }

        if (sortState == State.CIMETIERE) {
            return;
        }

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;

#if (UNITY_ANDROID || UNITY_IOS)
        // InformationsSurLaCarte(); 
        if (!isFromLocalPlayer && (getState() == State.JOUE)) {
            InformationsSurLaCarte(); 
        }
#else
        CliqueSimpleCarte(); 
#endif
    }

    /// <summary>
    /// Lors d'un clique simple sur la carte.
    /// </summary>
    /// <param name="drag"></param>
    public void CliqueSimpleCarte(bool drag = false) {
        int nombreSortsLances = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().sortLance;
        if (clicked != 0) {
            canGoBig = false;
            // Si on a déjà cliqué sur la carte une première fois
            if (nombreSortsLances == 0) {
                // Aucun sort n'a encore été lancé par le joueur pendant la partie. Il peut donc en lancer un.
                ChangePosition();
                // Le sort a été joué. 
                Debug.Log("Le sort a été lancé");
                DisplayMessage("Le sort " + Name + "a été joué");
            }
            else {
                clicked = 0;
                canGoBig = true;
                // TODO: Display Message qui dit qu'un sort a déjà été lancé.
                Main.GetComponent<MainJoueur>().ReordonnerCarte(); 
                DisplayMessage("Un sort a déjà été lancé à ce tour."); 
            }
        }
        else {
            // Si on clique sur la carte pour la première fois.
            clicked = 1;
            canGoBig = false;
            positionBeforeDragging = new Vector3(transform.position.x,
                                                 transform.position.y,
                                                 transform.position.z); 
            //Dragging(); 
        }
    }

    public override void checkIfLocalPlayerOnMousEnter() {

        if (!isFromLocalPlayer) {
            sortState = State.ADVERSAIRE;
        }
    }

    public override void OnMouseExit() {
        /*
		 * Lorsque la souris qui "passait" sur la carte la quitte. 
		 * A ce moment là on détruit le zoom qui avait été créé sur la carte. 
		 * 
		 */
        base.OnMouseExit(); 
        //if (sortState == State.MAIN) {
        //    GetComponent<SpriteRenderer>().enabled = true;
        //    Destroy(BigCard);
        //    // On lance la petite animation de fin. 
        //    // StartCoroutine(AnimationFinBigCard());
        //    // On réactive les objets enfants lors de la destruction de la grosse carte. 
        //    for (int i = 0; i < transform.childCount; ++i) {
        //        transform.GetChild(i).gameObject.SetActive(true);
        //    }
        //}
    }

    /// <summary>
    /// Drag de la carte
    /// </summary>
    public override void OnMouseDrag() {

        if (!isFromLocalPlayer) {
            return;
        }

        dragging = true; 

        Vector3 MousePosition = Input.mousePosition;
        MousePosition.z = 15;
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);

#if (UNITY_ANDROID || UNITY_IOS)
        clicked = 1;
        if (!dragging && Vector2.Distance(mouseWorldPoint, positionBeforeDragging) > 0.5f) {
            clicked = 0;
            CliqueSimpleCarte(true);
            dragging = true;
        }
#endif
    }

    /// <summary>
    /// Lorsqu'on relache le clic sur une carte
    /// </summary>
    public void OnMouseUp() {

        if (!isFromLocalPlayer && sortState == State.MAIN) {
            return;
        }

        // Sinon on n'appelle pas la fonction
        dragging = false;

        Vector3 MousePosition = Input.mousePosition;
        MousePosition.z = 15;
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);

        // Dans le cas où l'on veut reposer le sort dans la main.
        if (Math.Abs(transform.position.y - positionBeforeDragging.y) < 1f) {
            DisplayMessage("Sort reposé dans la main"); 
            Main.GetComponent<MainJoueur>().ReordonnerCarte();
            return; 
        }


        Debug.Log(Vector3.Distance(positionBeforeDragging, transform.position));
        if (Vector2.Distance(positionBeforeDragging, mouseWorldPoint) > 0.5f) {
            CliqueSimpleCarte(true);
            positionBeforeDragging = transform.position;
        }

        else {
#if (UNITY_ANDROID || UNITY_IOS)
            InformationsSurLaCarte();
            clicked = 0;
            Main.SendMessage("ReordonnerCarte");
#endif
        }
    }
    /// <summary>
    /// Changer la position du sort, 
    /// Appelé lors d'un clic sur la carte. 
    /// Vérifie si la carte va être rejouée ou remise dans la main. 
    /// </summary>
    void ChangePosition() {
        /*
         * Lors du deuxième clique du joueur, après que la carte ait été bougée. 
         * Contrairement à une carte entité, 
         * il n'y a que deux pissibilité:
         * soit le joueur reclique sur sa main et repose la carte, 
         * soit le joueur clique autre part et la carte est jouée. 
         */
        Debug.Log("Changer La Position");
         if (Mathf.Abs(transform.position.y - Main.transform.position.y) < 1) {
            // Si le joueur reclique sur sa main, la carte est reposée. 
            Main.SendMessage("ReordonnerCarte"); 
        } else {
            JouerSort();
        }
    }

    /// <summary>
    /// Jouer le sort. 
    /// </summary>
    void JouerSort() {
        /*
         * Jouer la carte sort
         */
        // Si on a fait un clic sur la carte
        if (!dragging) {
            Debug.Log("On passe ici");
            if (coutCarte <= FindLocalPlayer().GetComponent<Player>().PlayerAKA && clicked == 1) {
                // Target du sort. 
                if (!ApplyEffectOnAll()) {
                    int CartesNecessaires = AllEffets[0].CartesNecessairesSort();
                    Debug.Log("Cartes nécessaires");
                    Debug.Log(CartesNecessaires);
                    if (CartesNecessaires < 1) {
                        throw new UnusualBehaviourException("un sort qui n'est pas global doit target plusieurs entités");
                    }
                    else if (CartesNecessaires == 1) {
                        // on ne doit destroy le box collider que si c'est pour une carte. 
                        // On change le sprite de la carte en une cible par exemple pour pouvoir target une autre carte,
                        Destroy(GetComponent<BoxCollider2D>());

                        GetComponent<SpriteRenderer>().sprite = Cible;
                        // On informe le gameManager qu'un sort est en cours, lors d'un clic prochain sur une carte. 
                        GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours = gameObject;
                        // Debug.Log("On a mis sort en cours à gameObject"); 
                        clicked = 2;
                    } else {
                        if (!GererConditions(AllEffets[0].AllConditionsEffet, debut: true, jouerDirect: true)) {
                            clicked = 0;
                            // on remet l'image de la carte. 
                            StartCoroutine(setImageCarte());
                            Main.SendMessage("ReordonnerCarte");
                            DisplayMessage("Les conditions ne sont pas réunies.");
                        }
                        else {
                            // Le sort a été joué. 
                            clicked = 0;
                            transform.position = new Vector2(-10, -10);
                            Destroy(GetComponent<SpriteRenderer>());
                            StartCoroutine(JouerSortPlusieursCartes(0));
                        }
                    }
                }
            }
            else if (clicked == 2) {
                // 3è clic sur la carte == choix de la carte sur laquelle le sort va être joué. 
                // ATTENTION: Il est possible qu'une carte ait effet sur tout le terrain, dans ce cas pas besoin d'un troisième clic.
                // Dans le cas où le sort n'a qu'une seule cible
                Debug.Log("Cliqué une deuxième fois");
                // CarteCibleeRayCast contient la référence de la carte qui est ciblee. 
                Debug.Log("Je suis dans le raycast");
            }
            else {
                // Display un message disant que le joueur n'a pas assez d'AKA pour jouer
                clicked = 0;
                Main.SendMessage("ReordonnerCarte");
                GameObject.Find("GameManager").GetComponent<GameManager>().DisplayMessage("Le sort n'a pas été lancé");
                return;
            }
        }
        // Si on a drag la carte
        // A priori on ne passe jamais ici.
        else {
            Debug.Log("On passe là"); 
            if (coutCarte <= getGameManager().GetComponent<GameManager>().getAKAGlobalTour() || true) {
                if (!ApplyEffectOnAll()) {
                    // Si l'effet s'applique sur tous il est joué dans la fonction
                    // ATTENTION : pour l'instant on part du principe que la carte n'a qu'un seul effet 
                    int CartesNecessaires = AllEffets[0].CartesNecessairesSort();
                    Debug.Log("Cartes nécessaires");
                    Debug.Log(CartesNecessaires);
                    if (CartesNecessaires < 1) {
                        throw new UnusualBehaviourException("un sort qui n'est pas global doit target plusieurs entités");
                    } else if (CartesNecessaires == 1) {
                        GameObject Proche = CarteProche(gameObject);
                        if (Proche != null) {
                            RecupererCarteJouerSort(Proche);
                        }
                        else {
                            clicked = 0;
                            dragging = false;
                            // on remet l'image de la carte. 
                            StartCoroutine(setImageCarte());
                            Main.SendMessage("ReordonnerCarte");
                        }
                    } else {
                        // grace à ça on saura quelles cartes il faut jouer. 
                        dragging = false;
                        Debug.Log("On gere les effets"); 
                        if (!GererConditions(AllEffets[0].AllConditionsEffet, debut: true, jouerDirect:true)) {
                            clicked = 0;
                            dragging = false;
                            // on remet l'image de la carte. 
                            StartCoroutine(setImageCarte());
                            Main.SendMessage("ReordonnerCarte");
                            DisplayMessage("Les conditions ne sont pas réunies.");
                        } else {
                            // Le sort a été joué. 
                            clicked = 0;
                            transform.position = new Vector2(-10, -10);
                            Destroy(GetComponent<SpriteRenderer>());
                            StartCoroutine(JouerSortPlusieursCartes(0)); 
                        }
                    }
                }
            } else {
                DisplayMessage("Vous n'avez pas assez d'AKA pour jouer ce sort");
                clicked = 0;
                dragging = false;
                StartCoroutine(setImageCarte());
                Main.SendMessage("ReordonnerCarte"); 
            }
        }
    }

    /// <summary>
    /// Après qu'on ait jouer le sort, 
    /// Si le sort a effet sur une ou plusieurs cartes en particulier, on devra cliquer sur ces cartes. 
    /// Et ces cartes appellent ensuite cette fonction.
    /// </summary>
    /// <param name="carteAffectee"></param>
    public void RecupererCarteJouerSort(GameObject carteAffectee) {
        /*
         * Cette fonction sera appelée par la carte sur laquelle le joueur aura cliqué après avoir cliqué sur le sort. 
         * 
         * Fonctionnement d'un sort qui ne target pas tout le board ou plusieurs cartes. 
         * D'abord on clique sur le sort. 
         * Puis on clique sur un endroit qui n'est pas la main, ce qui fait apparaitre une cible. 
         * Puis on clique sur la carte que l'on veut cibler. 
         * 
         * On pourrait aussi faire un système de drag and drop sur une carte. 
         * 
         * TODO : Faire appeler cette fonction par une carte.
         */

        // ApplyEffectOnCarte(CarteCibleeRayCast.transform.gameObject);
        Debug.Log("On a récupéré la carte"); 
        CarteCiblee = carteAffectee; 
        gameObject.tag = "SortJoue";
        // Le sort a été lancé
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().sortLance += 1;
        // Un choix de sort n'est plus en cours. 
        GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours = null;

        // On applique l'effet sur la carte. 
        // ApplyEffectOnCarte(carteAffectee);

        // Ici, il faut regarder si l'entité est une entité temporaire.
        if (CarteCiblee.GetComponent<EntiteTemporaire>() != null) {
            // Si la carte est une carte temporaire. 
            Debug.Log("La carte est une entité temporaire"); 
            CarteCiblee = CarteCiblee.GetComponent<EntiteTemporaire>().getVraieEntite(); 
        }

        // Si les conditions ne sont pas vérifiées, on remet la carte dans la main du joueur. 
        if (!GererEffets(AllEffets, Cible:CarteCiblee, debut: true)) {
            clicked = 0;
            dragging = false;
            // on remet l'image de la carte. 
            StartCoroutine(setImageCarte());
            Main.SendMessage("ReordonnerCarte");
            DisplayMessage("Les conditions ne sont pas réunies."); 
        } 
        // Sinon, le sort a été joué.

        // Le sort a été joué. 
        clicked = 0;
        transform.position = new Vector2(-10, -10);
        Destroy(GetComponent<SpriteRenderer>()); 
    }

    /// <summary>
    /// Jouer un sort sur plusieurs cartes. 
    /// </summary>
    /// <param name="numeroEffet">Numero de l"effet à jouer.</param>
    /// <returns></returns>
    private IEnumerator JouerSortPlusieursCartes(int numeroEffet) {
        yield return WaitForCardsChosen();
        GererActions(AllEffets[numeroEffet].AllActionsEffet, CibleDejaChoisie:true); 
    }

    /// <summary>
    /// Regarde si la carte a effet sur une ou toutes les cartes. 
    /// </summary>
    /// <returns></returns>
    bool ApplyEffectOnAll() {
        /*
         * Effectuer un sort sur tout le terrain. 
         * Avec verification, si le sort a une cible, plusieurs ou tout le terrain. 
         * 
         * return false dans le cas où l'effet ne target pas tout le monde 
         * true sinon
         */
        // Gestion de l'effet1
        Debug.Log(AllEffets[0].AllConditionsEffet[0].ConditionCondition);
         if (!HasEffetGlobal(0)) {
            // Pas d'effet global. 
            return false; 
        } else {
            // Effet global 
            GererEffets(AllEffets, debut: true);
            Debug.Log("Cette carte a un effet sur plusieurs cartes. "); 
            DetruireCarte();
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().sortLance += 1;
            return true; 
        }

    }

    bool HasEffetGlobal(int numeroEffet) {
        foreach (Condition cond in AllEffets[numeroEffet].AllConditionsEffet) {
            if (cond.ConditionCondition == Condition.ConditionEnum.EFFET_GLOBAL) {
                return true; 
            }
        }
        return false; 
    }

    /// <summary>
    /// Changer la puissance d'un element
    /// <obsolet>Cette méthode est obsolète. </obsolet>
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
                _carte.CmdAddStat(_intToAdd, 0);
            }
            else {
                FindLocalPlayer().GetComponent<Player>().CmdEnvoiMethodToServerCarteWithIntParameter(_carte.IDCardGame,
                    "CmdAddStat", _intToAdd, FindLocalPlayer().GetComponent<Player>().PlayerID);
            }
        }
    }

    /// <summary>
    /// Comme un sort peut durer plusieurs tours, 
    /// on updatera le sort à chaque tour afin de rendre par exemple un element d'origine à sa carte si celui-ci a été modifié. 
    /// A appeler à chaque début de tour.
    /// </summary>
    void UpdateSortNewTurn() {
        /* 
         */ 
         if (AllEffets.Count == 0) {
            // Si le sort n'effectue plus d'effets on le détruit. 
            CmdDetruireCarte(); 
         }

         for (int i = 0; i < AllEffets.Count; ++i) {
            if (AllEffets[i].AllActionsEffet[0].isEffetWithTimer()) {
                if (AllEffets[i].AllActionsEffet[0].intAction == 1) {
                    // Il faut donc remettre la carte dans un état normal. 
                    switch (AllEffets[i].AllActionsEffet[0].ActionAction) {
                        case Action.ActionEnum.NATURE_AIR:
                        case Action.ActionEnum.NATURE_EAU:
                        case Action.ActionEnum.NATURE_TERRE:
                        case Action.ActionEnum.NATURE_FEU:
                            CarteCiblee.SendMessage("resetCarteElement");
                            break; 
                    }
                    // Cet effet du sort est fini
                    AllEffets.RemoveAt(i);
                } else {
                    AllEffets[i].AllActionsEffet[0].intAction = AllEffets[i].AllActionsEffet[0].intAction - 1; 
                }
            } else {
                AllEffets.RemoveAt(i); 
            }
         }

    }



    // *------- METHODES ANNEXES

    public override void CreateBigCard(string messageToDisplay = "") {
        base.CreateBigCard(
            Name + "\n" +
            "Niveau" + Niveau.ToString());

        BigCard.GetComponent<Sort>().sortState = Sort.State.BIGCARD;
    }

    public override void DisplayInfoCarteGameManager(string shortCode = "", string messageToDisplay = "") {
        base.DisplayInfoCarteGameManager(this.shortCode,
            GetInfoCarte());
    }

    /// <summary>
    /// Appelé lors de l'instanciation de la carte sur le serveur. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetUpCard() {
        /*
		 * On attend de savoir si le state de la carte est Main ou pas. 
		 * Latence réseau. 
		 */

        // On attend un peu au début car la carte sera peut-être désactivée par le composant carteType; 
        // yield return new WaitForSeconds(0.1f); 

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        if (((Players[0].GetComponent<Player>().isLocalPlayer && Players[0].GetComponent<Player>().isServer) ||
            (Players[1].GetComponent<Player>().isLocalPlayer && Players[1].GetComponent<Player>().isServer)) && netId.Value != 0) {
            // Dans le cas d'une instantiation d'une carte sur le réseau.
            RpcsetoID1(IDCardGame, oID, Name, shortCode, Niveau, CoutAKA, AllEffetsString, AllEffetsStringToDisplay);
            // Inutile normalement.
            // RpcChangeParent (); 
        }

        yield return new WaitForSeconds(0.1f);

        // Debug.Log(netId.ToString() + hasAuthority.ToString());

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
        }
        else {
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

        if (IDCardGame == 0) {
            Destroy(gameObject);
            yield break; 
        }

        if (isFromLocalPlayer && sortState == State.MAIN) {
            // On informe que la carte a bien été piochée.
            FindLocalPlayer().GetComponent<Player>().CartePiocheOK(oID);
        }
    }

    /// <summary>
    /// Transmission d'informations sur les principales informations du sort.
    /// </summary>
    /// <param name="_ID">IDCardGame de la carte</param>
    /// <param name="_oID">oID de la carte (Meta Collection)</param>
    /// <param name="_Name">Nom de la carte</param>
    /// <param name="_shortCode">shortCode de la carte</param>
    /// <param name="_Niveau">Niveau du sort</param>
    /// <param name="_coutAKA">cout du Sort (inutile)</param>
    /// <param name="_Effet">Effets du sort (string à décortiquer)</param>
    /// <param name="_EffetToDisplay">Effets du sort (string compréhensible, à afficher)</param>
    [ClientRpc(channel=0)]
    void RpcsetoID1(int _ID, string _oID, string _Name, string _shortCode, int _Niveau,
                                    int _coutAKA, string _Effet, string _EffetToDisplay) {
        // On peut peut-être tout faire passer par les arguments. 
        IDCardGame = _ID;
        oID = _oID;
        Name = _Name;
        shortCode = _shortCode;
        sortState = State.MAIN;
        Niveau = _Niveau;
        CoutAKA = _coutAKA;
        AllEffetsString = _Effet;
        stringToEffetString(_EffetToDisplay);
        stringToEffetList(_Effet); 
        //OnStartAuthority (); 
    }


    public override void UpdateNewPhase(Player.Phases _currentPhase, int tour) {
        base.UpdateNewPhase(_currentPhase, tour);
        clicked = 0; 
    }

    public override void DetruireCarte() {
        Debug.Log("On detruit le sort"); 
        CmdDetruireCarte(); 
    }

    /// <summary>
    /// Destruction d'une carte sort. 
    /// </summary>
    /// <remarks>peut-être à inclure dans la classe parent "Carte"</remarks>
    [Command(channel=0)]
    void CmdDetruireCarte() {
        RpcDetruireCarte();
    }

    /// <summary>
    /// Detruire une carte.
    /// Execute sur les clients. 
    /// </summary>
    [ClientRpc(channel=0)]
    void RpcDetruireCarte() {
        clicked = 0; 

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;
        Cimetiere = transform.parent.parent.parent.Find("Cimetiere").Find("CartesCimetiere").gameObject;

        gameObject.tag = "Cimetiere";
        Cimetiere.SendMessage("CmdCarteDeposee", gameObject);
        Sanctuaire.SendMessage("ReordonnerCarte");
        ChampBataille.SendMessage("CmdReordonnerCarte");

        sortState = State.CIMETIERE; 
    }

    /// <summary>
    /// Recupere les informations essentielles du sort. 
    /// </summary>
    /// <returns></returns>
    public override string GetInfoCarte() {
        return "<color=red>" + Name + "</color>" + "\n" +
            "Niveau : " + Niveau.ToString() + "\n" +
            "Effets : " + AllEffetsStringToDisplay; 
    }

    /// <summary>
    /// Lors d'un clic droit sur la carte. 
    /// </summary>
    public override void RightClickOnCarte() {
        if (clicked != 0) {
            // Si un joueur fait un clic droit alors qu'il tient la carte en main, on remet la carte d'où elle vient. 
            clicked = 0;
            // on remet la carte dans la main
            Main.GetComponent<MainJoueur>().SendMessage("ReordonnerCarte"); 
            return;
        }
        base.RightClickOnCarte();
    }


    protected override bool CarteJouerReponseEffet(Player.Phases _currentPhase, int numeroListe = 0) {
        Debug.Log("On teste si cette carte peut utiliser un effet"); 
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

    public override bool isCarteInMain() {
        if (sortState == State.MAIN) {
            return true; 
        } else {
            return false; 
        }
    }


    /// <summary>
    /// Trouver la carte la plus proche de celle-ci
    /// </summary>
    /// <param name="distance">distance maximale que l'on veut entre deux cartes, sinon
    /// on considère qu'il n'y en a aucune de proche</param>
    /// <returns>La carte la plus proche, ou null si aucune n'est assez proche. </returns>
    public static GameObject CarteProche(GameObject MaCarte, float distance=1f) {
        Carte[] AllCartes = FindObjectsOfType(typeof(Carte)) as Carte[];
        List<GameObject> CartesProches = new List<GameObject>();

        Debug.Log(AllCartes.Length); 

        for (int i = 0; i < AllCartes.Length; i++) {
            GameObject Carte = AllCartes[i].gameObject;
            Debug.Log(Vector2.Distance(MaCarte.transform.position, Carte.transform.position)); 
            if (Vector2.Distance(MaCarte.transform.position, Carte.transform.position) < distance && MaCarte != Carte) {
                Debug.Log("Il y a une carte proche ici"); 
                CartesProches.Add(Carte);
            }
        }

        // S'il n'y a pas de cartes proches
        if (CartesProches.Count == 0) {
            return null; 
        }
        // S'il n'y a qu'une carte proche (le mieux!)
        else if (CartesProches.Count == 1) {
            return CartesProches[0]; 
        }
        // S'il y a plusieurs cartes proches
        else {
            GameObject CarteLaPlusProche = CartesProches[0];
            float minDistance = Vector3.Distance(MaCarte.transform.position, CartesProches[0].transform.position); 
            for (int i = 0; i < CartesProches.Count; i++) {
                if (Vector3.Distance(MaCarte.transform.position, CartesProches[i].transform.position) < minDistance) {
                    minDistance = Vector3.Distance(MaCarte.transform.position, CartesProches[i].transform.position);
                    CarteLaPlusProche = CartesProches[i]; 
                }
            }
            return CarteLaPlusProche;
        }
    }

    public State getState() {
        return sortState;
    }

}
