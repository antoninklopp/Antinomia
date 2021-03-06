﻿
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Antinomia.Battle {

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
        private State assistanceState = State.DECK;

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
        public int Puissance;

        private int clicked = 0;

        private bool dragging = false;
        private Vector3 positionBeforeDragging;

        /// <summary>
        /// Le gameObject qui représente la liaison entre une entité et une assistance. 
        /// </summary>
        private GameObject LiaisonEntiteAssistance;

        /// <summary>
        /// Carte Ciblee par un effet de l'assistance.
        /// </summary>
        public GameObject CarteCiblee;

        public State AssistanceState {
            get {
                return assistanceState;
            }

            set {
                assistanceState = value;

                // On change le sprite de la carte selon son état
                switch (assistanceState) {
                    case State.ASSOCIE_A_CARTE:
                    case State.JOUEE:
                        if (!GetComponent<SpriteRenderer>().enabled) {
                            // On met à false le sprite renderer de la carte. 
                            GetComponent<SpriteRenderer>().enabled = true;
                            GetComponent<BoxCollider2D>().enabled = true;
                            GetComponent<ImageCardBattle>().setImage(shortCode);
                            GetComponent<VisuelCarte>().DisableVisuel();
                        }
                        break;
                    case State.MAIN:
                        if (!GetComponent<SpriteRenderer>().enabled && isFromLocalPlayer) {
                            // On met à false le sprite renderer de la carte. 
                            GetComponent<SpriteRenderer>().enabled = true;
                            GetComponent<BoxCollider2D>().enabled = true;
                            GetComponent<ImageCardBattle>().setImage(shortCode);
                            GetComponent<VisuelCarte>().SetUpVisuel();
                        }
                        else if (!isFromLocalPlayer) {
                            // Si la carte appartient à l'adversaire et est dans sa main, 
                            // il ne faut pas pouvoir la voir. 
                            GetComponent<ImageCardBattle>().setDosCarte();
                            GetComponent<VisuelCarte>().DisableVisuel();
                        }
                        break;
                    case State.CIMETIERE:
                        if (GetComponent<SpriteRenderer>().enabled) {
                            // On met à false le sprite renderer de la carte. 
                            GetComponent<SpriteRenderer>().enabled = false;
                            GetComponent<BoxCollider2D>().enabled = false;
                        }
                        break;

                }
            }
        }

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

            Def = new AssistanceDefinition();

            if (GetComponent<RectTransform>() == null) {
                // Si l'element est UI pas besoin de setUp la carte
                StartCoroutine(SetUpCard());
            }
        }

        // Update is called once per frame
        public override void Update() {
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

            if (!isFromLocalPlayer && AssistanceState == State.MAIN) {
                return;
            }

            Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;

#if (UNITY_ANDROID || UNITY_IOS)
        // InformationsSurLaCarte(); 
        if(!isFromLocalPlayer && (getState() == State.ASSOCIE_A_CARTE || getState() == State.JOUEE)) {
            InformationsSurLaCarte(); 
        }
#else
            CliqueSimpleCarte();
#endif
        }

        /// <summary>
        /// Lorsque la souris ou le doigt est levé(e)
        /// </summary>
        public void OnMouseUp() {

            if (!isFromLocalPlayer && AssistanceState == State.MAIN) {
                return;
            }

            Debug.Log(Vector3.Distance(positionBeforeDragging, transform.position));

            Vector3 MousePosition = Input.mousePosition;
            MousePosition.z = 15;
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);

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
            // Sinon on n'appelle pas la fonction
            dragging = false;
        }

        /// <summary>
        /// Lorsqu'on bouge la carte. 
        /// </summary>
        public override void OnMouseDrag() {

            if (!isFromLocalPlayer) {
                return;
            }

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
        /// Lors d'un clic sur la carte/ ou d'un drag pour android. 
        /// </summary>
        /// <param name="drag">la carte est-elle drag? </param>
        public void CliqueSimpleCarte(bool drag = false) {
            if (AssistanceState == State.CIMETIERE) {
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
                        (AssistanceState == State.JOUEE))
                        || ((GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PRINCIPALE1 ||
                        GameObject.Find("GameManager").GetComponent<GameManager>().Phase == Player.Phases.PRINCIPALE2) &&
                        (AssistanceState == State.MAIN || AssistanceState == State.JOUEE || AssistanceState == State.ASSOCIE_A_CARTE)))) {
                clicked = 1;
                positionBeforeDragging = new Vector3(transform.position.x,
                                                     transform.position.y,
                                                     transform.position.z);
                //canGoBig = false;
                ChangePosition();
            }
            else {
                positionBeforeDragging = new Vector3(transform.position.x,
                                                     transform.position.y,
                                                     transform.position.z);
                clicked = 1;
            }
        }

        /// <summary>
        /// Creation de la carte zoomée. 
        /// </summary>
        /// <param name="messageToDisplay"></param>
        public override void CreateBigCard(string messageToDisplay = "") {
            base.CreateBigCard("Assistance " + "\n" +
                Name + "\n");
            BigCard.GetComponent<Assistance>().AssistanceState = State.BIGCARD;
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
            }
            else {
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
                    if (!CheckIfDeposeAssistanceOK()) {
                        AssistanceRetourMain();
                        return;
                    }
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
            else {
                GameObject Proche = Sort.CarteProche(gameObject);
                if (Proche != null) {
                    RecupererCarteJouerAssistance(Proche);
                }
                else {
                    DisplayMessage("Impossible de jouer l'assistance.");
                    clicked = 0;
                    dragging = false;
                    Main.SendMessage("ReordonnerCarte");
                }
            }
        }

        public void RecupererCarteJouerAssistance(GameObject carteAffectee) {
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

            // On vérfie tout d'abord que la carte soit une entité sur le champ de bataille. 
            // Seule une carte sur le champ de bataille peut etre sacrifiée pour invoquer une assistance

            Debug.Log("On invoque une assistance ici");

            if (!carteAffectee.GetComponent<Carte>().IsEntite()) {
                AssistanceRetourMain();
                DisplayMessage("Sacrifice => Entité");
                return;
            }

            if (carteAffectee.GetComponent<Entite>().EntiteState != Entite.State.CHAMPBATAILLE) {
                AssistanceRetourMain();
                DisplayMessage("Sacrifice => Champ de Bataille.");
                return;
            }


            CarteCiblee = carteAffectee;
            gameObject.tag = "Assistance";
            // Un choix de sort n'est plus en cours. 
            GameObject.Find("GameManager").GetComponent<GameManager>().SortEnCours = null;

            switch (AssistanceState) {
                case State.MAIN:
                    MettreEffetDansLaPile(new List<GameObject>(), -2);

                    // Dans le cas où la carte est encore dans la main. 
                    // CmdPoserAssistance();

                    Debug.Log("Detruire " + carteAffectee.GetComponent<Carte>().Name);
                    carteAffectee.GetComponent<Entite>().DetruireCarte();
                    break;
                case State.JOUEE:
                    if (carteLieeID == 0) {
                        LierAssistance(carteAffectee);
                    }
                    else {
                        DisplayMessage("Impossible cette carte est déjà liée à une entité. ");
                    }
                    break;
                case State.ASSOCIE_A_CARTE:
                    Debug.Log("Delier1");
                    if (carteLieeID == carteAffectee.GetComponent<Entite>().IDCardGame) {
                        DelierAssistance(carteAffectee);
                    }
                    else {
                        DisplayMessage("Cette assistance est déjà liée à une entité");
                    }
                    break;
                default:
                    Debug.LogError("Comportement inattendu lors de la dépose d'une assistance");
                    break;
            }


            // Le sort a été joué. 
            GetComponent<BoxCollider2D>().enabled = true;

            clicked = 0;
        }

        /// <summary>
        ///  Si on a pas pu poser l'assistance, on la fait retourner dans la main. 
        ///  Appeler lors d'un mouvement impossible du joueur
        /// </summary>
        private void AssistanceRetourMain() {
            clicked = 0;
            Main.SendMessage("ReordonnerCarte");
            AssistanceState = State.MAIN;
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
            }
            else {
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
        [Command(channel = 0)]
        public void CmdPoserAssistance() {

            RpcPoserAssistance();

        }

        /// <summary>
        /// Poser l'assistance. 
        /// Fonction appelée sur tous les clients. 
        /// </summary>
        [ClientRpc(channel = 0)]
        void RpcPoserAssistance() {
            // On pose l'assistance. 
            Debug.Log("On pose l'assistance");
            AssistanceState = State.JOUEE;
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
        [Command(channel = 0)]
        void CmdDelierAssistance(GameObject carteAffectee) {
            RpcDelierAssistance(carteAffectee);
        }

        /// <summary>
        /// Délier une assistance à une entité. Fonction appelée sur le chaque client. 
        /// </summary>
        /// <param name="carteAffectee">L'entité à laquelle cette assistance est déliée</param>
        [ClientRpc(channel = 0)]
        void RpcDelierAssistance(GameObject carteAffectee) {
            AssistanceState = State.JOUEE;
            ChampBataille.SendMessage("CmdCarteDeposee", gameObject);

            Destroy(carteAffectee.GetComponent<EntiteAssocieeAssistance>());

            GetComponent<ImageCardBattle>().setImage(shortCode);

            GetComponent<SpriteRenderer>().color = Color.white;

            ProposerMettreJeuEnPause();

            Destroy(LiaisonEntiteAssistance);
        }

        /// <summary>
        /// Lier une assistance à une entité. Fonction appelée sur le serveur. 
        /// </summary>
        /// <param name="carteAffectee">L'entité à laquelle cette assistance est liée</param>
        [Command(channel = 0)]
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
        [ClientRpc(channel = 0)]
        void RpcLierAssistance(GameObject carteAffectee) {

            AssistanceState = State.ASSOCIE_A_CARTE;
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

            // Dessiner une ligne entre l'assistance et la carte. 
            GameObject LineRendererPrefab = Resources.Load("Prefabs/LiaisonEntiteAssistance", typeof(GameObject)) as GameObject;
            LiaisonEntiteAssistance = Instantiate(LineRendererPrefab);
            LiaisonEntiteAssistance.GetComponent<LineRendererAssistanceEntite>().setLine(carteAffectee, gameObject);
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
                RpcsetoID(IDCardGame, oID, Name, shortCode, Puissance, AllEffetsString, AllEffetsStringToDisplay);
                // Inutile normalement.
                // RpcChangeParent (); 
            }

            // Ici il faut attendre les infos, et pas attendre un temps fini. 
            yield return new WaitForSeconds(0.1f);

            if (AssistanceState != State.MAIN) {
                Debug.Log("<color=purple> Probleme ici, la carte aurait avoir le State Main</color>");
                // yield break; 
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

            if (IDCardGame == 0) {
                Destroy(gameObject);
                yield break;
            }

            if (isFromLocalPlayer && AssistanceState == State.MAIN) {
                // On informe que la carte a bien été piochée.
                if (!FindLocalPlayer().GetComponent<Player>().CartePiocheOK(oID)) {
                    CmdDestroyDirect();
                }
            }

            assistanceState = State.MAIN;
        }

        /// <summary>
        /// Fonction appelée sur tous les clients. 
        /// Transmettre les infos importantes de la carte. 
        /// </summary>
        /// <param name="_IDCardGame">Attribut de la carte. Numero d'identification de la carte dans la partie</param>
        /// <param name="_oID">Attribut de la carte. Numero d'identification de la carte dans la META COLLECTION.</param>
        /// <param name="_Name">Attribut de la carte.</param>
        /// <param name="_shortCode">Attribut de la carte.</param>
        /// <param name="_Puissance">Attribut de la carte.</param>
        /// <param name="_EffetString">Attribut de la carte. On parle ici des effets sous la forme à "décortiquer" et pas la forme 
        /// "compréhensible"</param>
        /// <param name="_EffetsToDisplay">Attribut de la carte. Forme compréhensible. </param>
        [ClientRpc(channel = 0)]
        public void RpcsetoID(int _IDCardGame, string _oID, string _Name, string _shortCode,
            int _Puissance, string _EffetString, string _EffetsToDisplay) {
            IDCardGame = _IDCardGame;
            oID = _oID;
            Name = _Name;
            shortCode = _shortCode;
            Puissance = _Puissance;
            AllEffetsString = _EffetString;
            AssistanceState = State.MAIN;
            stringToEffetString(_EffetsToDisplay);

            stringToEffetList(_EffetString);

        }

        /// <summary>
        /// Detruire la carte.
        /// </summary>
        public override void DetruireCarte() {

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
        [Command(channel = 0)]
        void CmdDetruireCarte() {

            RpcDetruireCarte();
        }

        /// <summary>
        /// Detruire la carte. 
        /// Appelé sur tous les clients. 
        /// </summary>
        [ClientRpc(channel = 0)]
        void RpcDetruireCarte() {

            ChampBataille = transform.parent.parent.parent.Find("ChampBatailleJoueur").Find("CartesChampBatailleJoueur").gameObject;
            Main = transform.parent.parent.parent.Find("MainJoueur").Find("CartesMainJoueur").gameObject;
            Sanctuaire = transform.parent.parent.parent.Find("Sanctuaire").Find("CartesSanctuaireJoueur").gameObject;
            Cimetiere = transform.parent.parent.parent.Find("Cimetiere").Find("CartesCimetiere").gameObject;

            gameObject.tag = "Cimetiere";
            Cimetiere.SendMessage("CmdCarteDeposee", gameObject);
            Sanctuaire.SendMessage("ReordonnerCarte");
            ChampBataille.SendMessage("CmdReordonnerCarte");

            AssistanceState = State.CIMETIERE;
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
            if (AssistanceState == State.MAIN) {
                return true;
            }
            else {
                return false;
            }
        }

        public Assistance.State getState() {
            return AssistanceState;
        }

        protected override void InformationsSurLaCarte() {

            isFromLocalPlayer = transform.parent.parent.parent.gameObject.GetComponent<Player>().isLocalPlayer;
            if (!isFromLocalPlayer && AssistanceState == State.MAIN) {
                return;
            }
            base.InformationsSurLaCarte();
        }

        public override bool IsAssistance() {
            return true;
        }

        /// <summary>
        /// On vérifie s'il est possible de déposer une assistance sur le board
        /// </summary>
        /// <returns></returns>
        private bool CheckIfDeposeAssistanceOK() {
            // Si il n'y a pas de cartes on ne peut pas déposer d'assistance. 
            int nombreEntites = 0;
            foreach (GameObject g in
                FindLocalPlayer().GetComponent<Player>().GetChampBatailleJoueur().GetComponent<CartesBoard>().getCartesChampBataille()) {
                // On a trouvé une entité, il y en a donc plus que zéro. 
                // L'assistance peut donc être invoquée
                // On sort de la boucle. 
                if (g.GetComponent<Carte>().IsEntite()) {
                    nombreEntites += 1;
                    break;
                }
            }

            return (nombreEntites != 0);
        }

    }

}
