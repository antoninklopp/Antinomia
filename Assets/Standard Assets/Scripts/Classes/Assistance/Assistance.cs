﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class Assistance : Carte {
    /*
     * 
     * Ce type de carte peut être invoqué sur votre champ de bataille en sacrifiant une entité de votre champ de bataille. 
     * Elle n’est pas considérée comme une entité et ne procure pas d’Aka rémanent. 
     * Tant que vous contrôlez sur votre champ de bataille un nombre d’entités inférieur ou égal au nombre d'assistances que vous contrôlez, 
     * envoyez au cimetière une assistance de votre choix. Une assistance ne peut pas attaquer mais peut l’être. 
     * Durant l’une de vos phases principales, vous pouvez lier ou délier à une assistance une entité 
     * de votre champ de bataille qui n’a pas attaqué ce tour. Une entité liée ne peut pas déclarer d'attaque. 
     * Une assistance ne peut pas être ciblée par une attaque tant que l’entité qui lui est liée est sur le champ de bataille. 
     * Une assistance ne peut être liée qu’à une entité à la fois.
     * 
     */


    public enum State {
        MAIN,
        ASSOCIE_A_CARTE,
        JOUEE, 
        DECK,
        CIMETIERE, 
        BIGCARD
        };

    public State assistanceState = State.DECK; 

    public int STAT;
    public string EffetString;

    private int clicked = 0;

    public GameObject CarteCiblee; 

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
	void Update () {
		if (clicked != 0) {
            Dragging(); 
        }
	}

    private void OnMouseDown() {
        /*
         * Lors d'un click sur la carte. 
         */


        Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;

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
            } else {
                GameObject.Find("GameManager").SendMessage("AttackOtherPlayer", gameObject);
            }
        }

        if (clicked != 0) {
            clicked = 1;
            canGoBig = false;
            ChangePosition();
        } else {
            clicked = 1;
            canGoBig = false; 
        }
    }

    public override void CreateBigCard(string messageToDisplay = "") {
        base.CreateBigCard("Assistance " + "\n" + 
            Name + "\n");
        BigCard.GetComponent<Assistance>().assistanceState = State.BIGCARD;
    }

    public override void DisplayInfoCarteGameManager(string shortCode = "", string messageToDisplay = "") {
        base.DisplayInfoCarteGameManager(this.shortCode,
            "Assistance " + "\n" +
            Name + "\n");
    }

    public override void OnMouseExit() {
        /*
		 * Lorsque la souris qui "passait" sur la carte la quitte. 
		 * A ce moment là on détruit le zoom qui avait été créé sur la carte. 
		 * 
		 */
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
        }
        else {
            JouerAssistance();
        }
    }

    void JouerAssistance() {
        /*
         * Jouer la carte sort
         */

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

        carteAffectee.SendMessage("DetruireCarte");

        CmdPoserAssistance(); 

        // On applique l'effet sur la carte. 
        // LierAssistance(carteAffectee);

        // Le sort a été joué. 
        clicked = 0;
    }



    void LierAssistance(GameObject carteAffectee) {
        /*
         * Lier l'assistance à une entité. 
         */ 
         // On regarde d'abord si la carteAffectee a attaqué à ce tour. 
         if (carteAffectee.GetComponent<Entite>().hasAttacked) {
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("DisplayMessage", "Impossible de lier une " +
                "assistance à une carte ayant déjà attaqué"); 
            return; 
        } else {
            // on ajoute le script EntiteAssocieeAssistance à la carte affectée.
            CmdLierAssistance(carteAffectee); 
        }
    }

    [Command]
    void CmdPoserAssistance() {
        /*
         * Pose d'une assistance sur le terrain. 
         */ 
        RpcPoserAssistance(); 

    }

    [ClientRpc]
    void RpcPoserAssistance() {
        assistanceState = State.ASSOCIE_A_CARTE;
        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        ChampBataille.SendMessage("CmdCarteDeposee", gameObject);
        Main.SendMessage("ReordonnerCarte");

        GetComponent<ImageCardBattle>().setImage(shortCode);
    }

    [Command]
    void CmdLierAssistance(GameObject carteAffectee) {
        /*
         * Changement effectués sur le réseau. 
         */
        RpcLierAssistance(carteAffectee); 
    }

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
    }

    void DelierAssistance() {

    }
    
    void EntiteDetruite() {
        /*
         * On joue l'assistance lorsque l'entité est détruite. 
         */
        CmdEntiteDetruite(); 
    }

    [Command]
    void CmdEntiteDetruite() {
        RpcEntiteDetruite();
    }

    [ClientRpc]
    void RpcEntiteDetruite() {
        /*
         * Lorsqu'une entité est détruite, son assistance associée prend sa place. 
         */ 
        assistanceState = State.JOUEE;
        GetComponent<BoxCollider2D>().enabled = true;
        transform.localScale = new Vector3(localScaleCard, localScaleCard, localScaleCard);

        ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
        ChampBataille.SendMessage("CmdCarteDeposee", gameObject);


    }


    public IEnumerator SetUpCard() {

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        if (((Players[0].GetComponent<Player>().isLocalPlayer && Players[0].GetComponent<Player>().isServer) ||
            (Players[1].GetComponent<Player>().isLocalPlayer && Players[1].GetComponent<Player>().isServer)) && netId.Value != 0) {
            // Dans le cas d'une instantiation d'une carte sur le réseau.
            RpcsetoID(IDCardGame, oID, Name, shortCode, STAT, EffetString);
            // Inutile normalement.
            // RpcChangeParent (); 
        }

        // Ici il faut attendre les infos, et pas attendre un temps fini. 
        yield return new WaitForSeconds(0.1f);

        if (assistanceState != State.MAIN) {
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
    }

    [ClientRpc]
    public void RpcsetoID(int _IDCardGame, string _oID, string _Name, string _shortCode,
        int _STAT, string _EffetString) {
        IDCardGame = _IDCardGame;
        oID = _oID;
        Name = _Name;
        shortCode = _shortCode;
        STAT = _STAT;
        EffetString = _EffetString;
        assistanceState = State.MAIN; 

        stringToEffetList(_EffetString);

    }

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

    [Command]
    void CmdDetruireCarte() {

        RpcDetruireCarte(); 
    }

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
    }


}