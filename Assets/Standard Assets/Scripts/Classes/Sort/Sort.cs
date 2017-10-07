using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System; 

/// <summary>
/// Classe pour modéliser un sort. 
/// Hérite de la classe carte. 
/// </summary>
public class Sort : Carte {

     /// <summary>
     /// Niveau du sort. 
     /// Pour pouvoir être joué, un sort doit avoir un niveau inférieur ou égal à l'AKA rémanent calculé au début du tour.
     /// </summary>
    public int Niveau;
    public string Condition;
    public int CoutAKA;


    public Sort() {
        /*
         * 
         */


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

        base.OnMouseDown(); 

        if (!isFromLocalPlayer) {
            return; 
        }

        if (sortState == State.CIMETIERE) {
            return;
        }

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;

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
            } else {
                clicked = 0;
                canGoBig = true; 
                // TODO: Display Message qui dit qu'un sort a déjà été lancé. 
            }
        } else {
            // Si on clique sur la carte pour la première fois. 
            clicked = 1;
            canGoBig = false; 
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
        if (coutCarte <= FindLocalPlayer().GetComponent<Player>().PlayerAKA && clicked == 1) {
            // On change le sprite de la carte en une cible par exemple pour pouvoir target une autre carte,
            Destroy(GetComponent<BoxCollider2D>()); 
            // Target du sort. 
            if (!ApplyEffectOnAll()) {
                GetComponent<SpriteRenderer>().sprite = Cible;
                // On informe le gameManager qu'un sort est en cours, lors d'un clic prochain sur une carte. 
                GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours = gameObject;
                clicked = 2;
            }
        }
        else if (clicked == 2) {
            // 3è clic sur la carte == choix de la carte sur laquelle le sort va être joué. 
            // ATTENTION: Il est possible qu'une carte ait effet sur tout le terrain, dans ce cas pas besoin d'un troisième clic.
            // Dans le cas où le sort n'a qu'une seule cible
            Debug.Log("Cliqué une deuxième fois"); 
            // CarteCibleeRayCast contient la référence de la carte qui est ciblee. 
            Debug.Log("Je suis dans le raycast");
            
        } else {
            // Display un message disant que le joueur n'a pas assez d'AKA pour jouer
            clicked = 0;
            Main.SendMessage("ReordonnerCarte");
            GameObject.Find("GameManager").GetComponent<GameManager>().DisplayMessage("Le sort n'a pas été lancé"); 
            return; 
        }
    }

    /// <summary>
    /// Après qu'on ait jouer le sort, 
    /// Si le sort a effet sur une ou plusieurs cartes en particulier, on devra cliquer sur ces cartes. 
    /// Et ces cartes appellent ensuite cette fonction.
    /// </summary>
    /// <param name="carteAffectee"></param>
    void RecupererCarteJouerSort(GameObject carteAffectee) {
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
        CarteCiblee = carteAffectee; 
        gameObject.tag = "SortJoue";
        // Le sort a été lancé
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().sortLance += 1;
        // Un choix de sort n'est plus en cours. 
        GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours = null;

        // On applique l'effet sur la carte. 
        // ApplyEffectOnCarte(carteAffectee);

        GererEffets(AllEffets, Cible:CarteCiblee); 

        // Le sort a été joué. 
        clicked = 0;
        transform.position = new Vector2(-10, -10);
        Destroy(GetComponent<SpriteRenderer>()); 
    }

    /// <summary>
    /// Appliquer un effet sur une carte.
    /// NE DOIT PAS ETRE UTILISE dans la version actuelle du jeu. 
    /// </summary>
    /// <param name="Cible">Carte cible du sort</param>
    void ApplyEffectOnCarte(GameObject Cible) {
        /*
         * Envoyer à la carte l'effet appliqué. 
         * 
         * NE DOIT PAS ETRE UTILISE
         */ 
         // Gestion de l'effet 1
        switch (AllEffets[0].AllActionsEffet[0].ActionAction) {
            case (Action.ActionEnum.CHANGER_POSITION):
                Cible.SendMessage("setClicked", true); 
                break;
            case (Action.ActionEnum.DETRUIRE):
                Cible.SendMessage("DetruireCarte"); 
                break;
            case (Action.ActionEnum.NATURE_AIR):
                Cible.GetComponent<Entite>().CmdChangeElement(Entite.Element.AIR, true);
                Debug.Log("Element de la carte changé en AIR"); 
                break;
            case (Action.ActionEnum.NATURE_EAU):
                Cible.GetComponent<Entite>().CmdChangeElement(Entite.Element.EAU, true);
                Debug.Log("Element de la carte changé en EAU");
                break;
            case (Action.ActionEnum.NATURE_FEU):
                Cible.GetComponent<Entite>().CmdChangeElement(Entite.Element.FEU, true);
                Debug.Log("Element de la carte changé en FEU");
                break;
            case (Action.ActionEnum.NATURE_TERRE):
                Cible.GetComponent<Entite>().CmdChangeElement(Entite.Element.TERRE, true);
                Debug.Log("Element de la carte changé en TERRE");
                break; 
        }
    }

    bool ApplyEffectOnAll() {
        /*
         * Effectuer un sort sur tout le terrain. 
         * Avec verification, si le sort a une cible, plusieurs ou tout le terrain. 
         * 
         * return false dans le cas où l'effet ne target pas tout le monde 
         * true sinon
         * 
         * NE DOIT PAS ETRE UTILISE
         */
        // Gestion de l'effet1
         if (!AllEffets[0].AllActionsEffet[0].isEffetTargetAll()) {
            return false; 
        } else {
            switch (AllEffets[0].AllActionsEffet[0].ActionAction) {
                case Action.ActionEnum.PIOCHER_CARTE:
                    Debug.Log("Effet PIOCHER CARTE");
                    // Attention possibilité de piocher plusieurs cartes. 
                    StartCoroutine(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().
                        PiocheMultiple(AllEffets[0].AllActionsEffet[0].intAction)); 
                    break;
                case Action.ActionEnum.PUISSANCE_AIR_AUGMENTE:
                    Debug.Log("Effet PUISSANCE AIR AUGMENTE"); 
                    ChangeEffetPuissance(Entite.Element.AIR, AllEffets[0].AllActionsEffet[0].intAction); 
                    break;
                case Action.ActionEnum.PUISSANCE_TERRE_AUGMENTE:
                    Debug.Log("Effet PUISSANCE TERRE AUGMENTE"); 
                    ChangeEffetPuissance(Entite.Element.TERRE, AllEffets[0].AllActionsEffet[0].intAction);
                    break;
                case Action.ActionEnum.PUISSANCE_EAU_AUGMENTE:
                    Debug.Log("Effet PUISSANCE EAU AUGMENTE"); 
                    ChangeEffetPuissance(Entite.Element.EAU, AllEffets[0].AllActionsEffet[0].intAction);
                    break;
                case Action.ActionEnum.PUISSANCE_FEU_AUGMENTE:
                    Debug.Log("Effet PUISSANCE FEU AUGMENTE"); 
                    ChangeEffetPuissance(Entite.Element.FEU, AllEffets[0].AllActionsEffet[0].intAction);
                    break;
                case Action.ActionEnum.TERRAIN_ASTRAL:
                    // TODO : A implémenter lors de l'implémentation des terrains
                    break;
                case Action.ActionEnum.TERRAIN_MALEFIQUE:
                    // TODO : A implémenter lors de l'implémentation des terrains. 
                    break; 
            }
            CmdDetruireCarte(); 
            return true; 
        }

    }

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
    /// Comme un sort peut durer plusieurs tours, 
    /// on updatera le sort à chaque tour afin de rendre par exemple un element d'origine à sa carte si celui-ci a été modifié. 
    /// A appeler à chaque début de tour.
    /// </summary>
    void updateSortNewTurn() {
        /*

         * 
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

        if (sortState != State.MAIN) {
            yield break;
        }

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
    [ClientRpc]
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
        AllEffetsStringToDisplay = _EffetToDisplay; 
        stringToEffetList(_Effet); 
        //OnStartAuthority (); 
    }


    public override void UpdateNewPhase(Player.Phases _currentPhase, int tour) {
        base.UpdateNewPhase(_currentPhase, tour);
        clicked = 0; 
    }

    /// <summary>
    /// Destruction d'une carte sort. 
    /// </summary>
    /// <remarks>peut-être à inclure dans la classe parent "Carte"</remarks>
    [Command]
    void CmdDetruireCarte() {

        RpcDetruireCarte();
    }

    /// <summary>
    /// Detruire une carte.
    /// Execute sur les clients. 
    /// </summary>
    [ClientRpc]
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

}
