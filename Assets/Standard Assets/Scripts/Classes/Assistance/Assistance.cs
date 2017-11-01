using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Ce type de carte peut être invoqué sur votre champ de bataille en sacrifiant une entité de votre champ de bataille. 
/// Elle n’est pas considérée comme une entité et ne procure pas d’Aka rémanent.
/// Tant que vous contrôlez sur votre champ de bataille un nombre d’entités inférieur ou égal au nombre d'assistances que vous contrôlez, 
/// envoyez au cimetière une assistance de votre choix. Une assistance ne peut pas attaquer mais peut l’être.
/// Durant l’une de vos phases principales, vous pouvez lier ou délier à une assistance une entité 
/// de votre champ de bataille qui n’a pas attaqué ce tour.Une entité liée ne peut pas déclarer d'attaque. 
/// Une assistance ne peut pas être ciblée par une attaque tant que l’entité qui lui est liée est sur le champ de bataille.
/// Une assistance ne peut être liée qu’à une entité à la fois.
/// </summary>
public class Assistance : Carte, ICarte {

    /// <summary>
    /// Etats possibles de l'assistance. 
    /// </summary>
    public enum State {
        MAIN,
        ASSOCIE_A_CARTE,
        JOUEE, 
        DECK,
        CIMETIERE, 
        BIGCARD
        };

    /// <summary>
    /// Etat courant de l'assistance. 
    /// </summary>
    public State assistanceState = State.DECK;

    /// <summary>
    /// Carte à laquelle l'assistance est liée. (string)
    /// </summary>
    public string carteLiee = "";

    /// <summary>
    /// Carte à laquelle l'assistance est liée (int IDCardGame)
    /// </summary>
    public int carteLieeID = 0; 

    /// <summary>
    /// STAT de l'assistance
    /// </summary>
    public int STAT;


    private int clicked = 0;

    private bool dragging = false;

    /// <summary>
    /// Carte Ciblee par un effet de l'assistance.
    /// </summary>
    public GameObject CarteCiblee; 

    /// <summary>
    /// Initialisation de la classe Assistance. 
    /// </summary>
    public Assistance() {

    }

    // Use this for initialization
    public override void Start() {
        base.Start();
        gameObject.tag = "Assistance";

        localScaleCard = Mathf.Abs(transform.localScale.x);
        canGoBig = true;

        if (GetComponent<RectTransform>() == null) {
            // Si l'element est UI pas besoin de setUp la carte
            StartCoroutine(SetUpCard());
        }
    }
	
	// Update is called once per frame
	public override void Update () {
        base.Update(); 
		if (clicked != 0) {
            Dragging(); 
        }
	}

    /// <summary>
    /// Lors d'un clic sur la carte
    /// </summary>
    public override void OnMouseDown() {
        /*
         * Lors d'un click sur la carte. 
         */
        base.OnMouseDown(); 

        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;

#if (UNITY_ANDROID || UNITY_IOS)
        InformationsSurLaCarte(); 
#else
        CliqueSimpleCarte();
#endif
    }

    public void OnMouseUp() {
        if (dragging) {
            CliqueSimpleCarte(true); 
        }
    }

    public override void OnMouseDrag() {
        base.OnMouseDrag();
        dragging = false; 
        CliqueSimpleCarte(true);
    }

    public void CliqueSimpleCarte(bool drag = false) {
        if (assistanceState == State.CIMETIERE) {
            return;
        }

        if (GameObject.Find("GameManager").GetComponent<GameManager>().Tour != FindLocalPlayer().GetComponent<Player>().PlayerID) {
            // On ne peut pas interagir avec ses cartes si ce n'est pas son tour!
            return;
        }

        // On ne peut interagir avec une assistance si on n'est pas dans une phase principale. 
        if (!(GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PRINCIPALE1) ||
            (GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PRINCIPALE2)) {
            return;
        }

        if (GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.COMBAT) {
            if (isFromLocalPlayer) {
                // Une assistance ne peut pas attaquer. 
                return;
            }
            else {
                GameObject.Find("GameManager").SendMessage("AttackOtherPlayer", gameObject);
            }
        }

        if (clicked != 0 && ((GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PREPARATION &&
                    (assistanceState == State.JOUEE))
                    || ((GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PRINCIPALE1 ||
                    GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PRINCIPALE2) &&
                    (assistanceState == State.MAIN || assistanceState == State.JOUEE || assistanceState == State.ASSOCIE_A_CARTE)))) {
            clicked = 1;
            //canGoBig = false;
            ChangePosition();
        }
        else {
            if (drag) {
                dragging = true;
            } else {
                clicked = 1;
            }
        }
    }

    /// <summary>
    /// Creation de la carte zoomée. 
    /// </summary>
    /// <param name="messageToDisplay"></param>
    public override void CreateBigCard(string messageToDisplay = "") {
        base.CreateBigCard("Assistance " + "\n" + 
            Name + "\n");
        BigCard.GetComponent<Assistance>().assistanceState = State.BIGCARD;
    }

    /// <summary>
    /// Montrer les infos dans le CardManager. 
    /// </summary>
    /// <param name="shortCode"></param>
    /// <param name="messageToDisplay"></param>
    public override void DisplayInfoCarteGameManager(string shortCode = "", string messageToDisplay = "") {
        if (carteLiee == "") {
            base.DisplayInfoCarteGameManager(this.shortCode,
                GetInfoCarte());
        } else {
            base.DisplayInfoCarteGameManager(this.shortCode,
                "<color=red>" + Name + "</color>" + "\n" +
                "Assistance " + "\n" +
                "Effets : " + AllEffetsStringToDisplay + "\n\n" + 
                "Liee à " + carteLiee);
        }
    }

    public override void OnMouseExit() {
        /*
		 * Lorsque la souris qui "passait" sur la carte la quitte. 
		 * A ce moment là on détruit le zoom qui avait été créé sur la carte. 
		 * 
		 */
        base.OnMouseExit();
        //if (assistanceState == State.MAIN) {
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
    /// Lors du deuxième clique du joueur, après que la carte ait été bougée. 
    /// Contrairement à une carte entité, 
    /// il n'y a que deux pissibilité:
    /// soit le joueur reclique sur sa main et repose la carte, 
    /// soit le joueur clique autre part et la carte est jouée.
    /// </summary>
    void ChangePosition() {
        Debug.Log("Changer La Position");
        if (Mathf.Abs(transform.position.y - Main.transform.position.y) < 1) {
            // Si le joueur reclique sur sa main, la carte est reposée. 
            Main.SendMessage("ReordonnerCarte");
        }
        else {
            JouerAssistance();
        }
    }

    /// <summary>
    /// Jouer l'assistance. 
    /// En deux temps, d'abord le joueur clique autre part que dans sa main pour indiquer
    /// qu'il veut jouer l'assistance. 
    /// Il choisit ensuite la carte à détruire pour pouvoir la poser (conformément aux règles). 
    /// </summary>
    void JouerAssistance() {
        if (!dragging) {
            if (clicked == 1) {
                // On change le sprite de la carte en une cible par exemple pour pouvoir target une autre carte,
                GetComponent<BoxCollider2D>().enabled = false;
                // Target du sort. 
                GetComponent<SpriteRenderer>().sprite = Cible;
                Debug.Log("Sprite changé en cible");
                // On informe le gameManager qu'un sort est en cours, lors d'un clic prochain sur une carte. 
                GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours = gameObject;
                clicked = 2;
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
                GameObject.Find("GameManager").GetComponent<GameManager>().DisplayMessage("L'assistance n'a pas été jouée");
                return;
            }
        } else {
            GameObject Proche = Sort.CarteProche(gameObject);
            if (Proche != null) {
                RecupererCarteJouerSort(Proche);
            }
            else {
                DisplayMessage("Vous n'avez pas assez d'AKA pour jouer ce sort");
                clicked = 0;
                dragging = false;
                Main.SendMessage("ReordonnerCarte");
            }
        }
    }

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
        gameObject.tag = "Assistance";
        // Un choix de sort n'est plus en cours. 
        GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours = null;


        if (assistanceState == State.MAIN) {
            // Dans le cas où la carte est encore dans la main. 
            CmdPoserAssistance();

            JouerEffetDeposeCarte();

            carteAffectee.SendMessage("DetruireCarte");
        }
        else if (assistanceState == State.JOUEE) {
            if (carteLieeID == 0) {
                LierAssistance(carteAffectee);
            } else {
                DisplayMessage("Impossible cette carte est déjà liée à une entité. "); 
            }
        }
        else if (assistanceState == State.ASSOCIE_A_CARTE){
            Debug.Log("Delier1"); 
            if(carteLieeID == carteAffectee.GetComponent<Entite>().IDCardGame) {
                DelierAssistance(carteAffectee);
            }
            else {
                DisplayMessage("Cette assistance est déjà liée à une entité");
            }
        }
        

        // Le sort a été joué. 
        GetComponent<BoxCollider2D>().enabled = true; 

        clicked = 0;
    }


    /// <summary>
    /// Lier l'assistance à une entité. 
    /// </summary>
    /// <param name="carteAffectee"></param>
    void LierAssistance(GameObject carteAffectee) {

         // On regarde d'abord si la carteAffectee a attaqué à ce tour. 
         if (carteAffectee.GetComponent<Entite>().hasAttacked == 1) {
            DisplayMessage("Impossible de lier une assistance à une carte ayant déjà attaqué"); 
            return; 
         } else {
            // on ajoute le script EntiteAssocieeAssistance à la carte affectée.
            CmdLierAssistance(carteAffectee);

            carteLiee = carteAffectee.GetComponent<Entite>().Name;
            carteLieeID = carteAffectee.GetComponent<Entite>().IDCardGame;
         }

    }

    /// <summary>
    /// Delier l'assistance d'une entité. 
    /// </summary>
    /// <param name="carteAffectee"></param>
    void DelierAssistance(GameObject carteAffectee) {
        if (carteAffectee.GetComponent<Entite>().hasAttacked == 1) {
            DisplayMessage("Impossible de lier une assistance à une carte ayant déjà attaqué");
            return;
        }
        else {
            // on ajoute le script EntiteAssocieeAssistance à la carte affectée.
            CmdDelierAssistance(carteAffectee);

            carteLiee = "";
            carteLieeID = 0;
        }
    }

    /// <summary>
    /// Poser d'une assistance sur le terrain. 
    /// Fonction faite sur le serveur. 
    /// </summary>
    [Command]
    void CmdPoserAssistance() {

        RpcPoserAssistance(); 

    }

    /// <summary>
    /// Poser l'assistance. 
    /// Fonction appelée sur tous les clients. 
    /// </summary>
    [ClientRpc]
    void RpcPoserAssistance() {

        assistanceState = State.JOUEE;
        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        ChampBataille.SendMessage("CmdCarteDeposee", gameObject);
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Main.SendMessage("ReordonnerCarte");

        GetComponent<ImageCardBattle>().setImage(shortCode);

        GetComponent<BoxCollider2D>().enabled = true;

        ProposerMettreJeuEnPause(); 
    }

    /// <summary>
    /// Délier une assistance à une entité. Fonction appelée sur le serveur. 
    /// </summary>
    /// <param name="carteAffectee">L'entité à laquelle cette assistance est déliée</param>
    [Command]
    void CmdDelierAssistance(GameObject carteAffectee) {
        RpcDelierAssistance(carteAffectee); 
    }

    /// <summary>
    /// Délier une assistance à une entité. Fonction appelée sur le chaque client. 
    /// </summary>
    /// <param name="carteAffectee">L'entité à laquelle cette assistance est déliée</param>
    [ClientRpc]
    void RpcDelierAssistance(GameObject carteAffectee) {
        assistanceState = State.JOUEE;
        ChampBataille.SendMessage("CmdCarteDeposee", gameObject);

        Destroy(carteAffectee.GetComponent<EntiteAssocieeAssistance>());

        GetComponent<ImageCardBattle>().setImage(shortCode);

        GetComponent<SpriteRenderer>().color = Color.white;

        ProposerMettreJeuEnPause(); 
    }

    /// <summary>
    /// Lier une assistance à une entité. Fonction appelée sur le serveur. 
    /// </summary>
    /// <param name="carteAffectee">L'entité à laquelle cette assistance est liée</param>
    [Command]
    void CmdLierAssistance(GameObject carteAffectee) {
        /*
         * Changement effectués sur le réseau. 
         */
        RpcLierAssistance(carteAffectee);
    }

    /// <summary>
    /// Lier une assistance à une entité. Fonction appelée sur le chaque client. 
    /// </summary>
    /// <param name="carteAffectee">L'entité à laquelle cette assistance est liée</param>
    [ClientRpc]
    void RpcLierAssistance(GameObject carteAffectee) {
        
        assistanceState = State.ASSOCIE_A_CARTE;
        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        ChampBataille.SendMessage("CmdCarteDeposee", gameObject);
        Main.SendMessage("ReordonnerCarte");

        carteAffectee.AddComponent<EntiteAssocieeAssistance>();
        carteAffectee.GetComponent<EntiteAssocieeAssistance>().AssistanceAssociee = gameObject;

        //GetComponent<BoxCollider2D>().enabled = false;
        //transform.position = new Vector2(carteAffectee.transform.position.x + 0.5f,
        //    carteAffectee.transform.position.y + 0.5f);
        //StartCoroutine(setImageCarte());
        //transform.localScale /= 3;

        GetComponent<ImageCardBattle>().setImage(shortCode);

        GetComponent<SpriteRenderer>().color = Color.red; 
    }
    
    /// <summary>
    /// Detruire l'assistance. 
    /// </summary>
    void EntiteDetruite() {
        /*
         * On joue l'assistance lorsque l'entité est détruite. 
         */
        DetruireCarte(); 
    }

    /// <summary>
    /// Appelée lorsque la carte est instanciée sur le serveur. 
    /// Permet de transmettre toutes les infos depuis l'objet carte sur le serveur, jusqu'aux cartes sur les clients. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetUpCard() {

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        if (((Players[0].GetComponent<Player>().isLocalPlayer && Players[0].GetComponent<Player>().isServer) ||
            (Players[1].GetComponent<Player>().isLocalPlayer && Players[1].GetComponent<Player>().isServer)) && netId.Value != 0) {
            // Dans le cas d'une instantiation d'une carte sur le réseau.
            RpcsetoID(IDCardGame, oID, Name, shortCode, STAT, AllEffetsString, AllEffetsStringToDisplay);
            // Inutile normalement.
            // RpcChangeParent (); 
        } else {
            yield break; 
        }

        // Ici il faut attendre les infos, et pas attendre un temps fini. 
        yield return new WaitForSeconds(0.1f);

        if (assistanceState != State.MAIN) {
            Debug.Log("<color=purple> Probleme ici, la carte aurait avoir le State Main</color>"); 
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
    /// Fonction appelée sur tous les clients. 
    /// Transmettre les infos importantes de la carte. 
    /// </summary>
    /// <param name="_IDCardGame">Attribut de la carte. Numero d'identification de la carte dans la partie</param>
    /// <param name="_oID">Attribut de la carte. Numero d'identification de la carte dans la META COLLECTION.</param>
    /// <param name="_Name">Attribut de la carte.</param>
    /// <param name="_shortCode">Attribut de la carte.</param>
    /// <param name="_STAT">Attribut de la carte.</param>
    /// <param name="_EffetString">Attribut de la carte. On parle ici des effets sous la forme à "décortiquer" et pas la forme 
    /// "compréhensible"</param>
    /// <param name="_EffetsToDisplay">Attribut de la carte. Forme compréhensible. </param>
    [ClientRpc]
    public void RpcsetoID(int _IDCardGame, string _oID, string _Name, string _shortCode,
        int _STAT, string _EffetString, string _EffetsToDisplay) {
        IDCardGame = _IDCardGame;
        oID = _oID;
        Name = _Name;
        shortCode = _shortCode;
        STAT = _STAT;
        AllEffetsString = _EffetString;
        assistanceState = State.MAIN;
        AllEffetsStringToDisplay = _EffetsToDisplay; 

        stringToEffetList(_EffetString);

    }

    /// <summary>
    /// Detruire la carte.
    /// </summary>
    void DetruireCarte() {
        
        if (transform.parent.parent.parent.gameObject.GetComponent<Player>().isLocalPlayer) {
            /*
			 * Si on est pas dans le cas d'un player local, on ne peut pas envoyer de command. 
			 * 
			 */
            CmdDetruireCarte();
        }
        else {
            GameObject LocalPlayer = FindLocalPlayer();
            LocalPlayer.SendMessage("DetruireCarte", gameObject.GetComponent<Entite>().IDCardGame);
        }
    }

    /// <summary>
    /// Detruire la carte. 
    /// Serveur. 
    /// </summary>
    [Command]
    void CmdDetruireCarte() {

        RpcDetruireCarte(); 
    }

    /// <summary>
    /// Detruire la carte. 
    /// Appelé sur tous les clients. 
    /// </summary>
    [ClientRpc]
    void RpcDetruireCarte() {

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
        Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;
        Cimetiere = transform.parent.parent.parent.Find("Cimetiere").Find("CartesCimetiere").gameObject;

        gameObject.tag = "Cimetiere";
        Cimetiere.SendMessage("CmdCarteDeposee", gameObject);
        Sanctuaire.SendMessage("ReordonnerCarte");
        ChampBataille.SendMessage("CmdReordonnerCarte");

        assistanceState = State.CIMETIERE; 
    }

    /// <summary>
    /// Update de la carte lors d'un changement de phase. 
    /// </summary>
    /// <param name="_currentPhase">La nouvelle phase</param>
    /// <param name="tour">Le nouveau tour</param>
    public override void UpdateNewPhase(Player.Phases _currentPhase, int tour) {
        base.UpdateNewPhase(_currentPhase, tour);
        clicked = 0;

    }

    /// <summary>
    /// Recupérer les infos de l'assistance. 
    /// </summary>
    /// <returns>Un string contenant les infos essentielles à la carte. </returns>
    public override string GetInfoCarte() {
        return "<color=red>" + Name + "</color>" + "\n" +
                "Assistance " + "\n" +
                "Effets : " + AllEffetsStringToDisplay; 
    }

    /// <summary>
    /// La carte peut-elle être jouée en réponse à un effet.
    /// </summary>
    /// <returns>toujours false, une assistance ne peut pas répondre à un effet. </returns>
    protected override bool CarteJouerReponseEffet(Player.Phases _currentPhase, int numeroListe = 0) {
        return false; 
    }

    public override bool isCarteInMain() {
        if (assistanceState == State.MAIN) {
            return true; 
        } else {
            return false;
        }
    }

}
