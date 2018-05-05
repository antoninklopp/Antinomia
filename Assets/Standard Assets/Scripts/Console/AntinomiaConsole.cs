
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Antinomia.Battle;
using Antinomia.GameSparksScripts;

namespace Console {

    /// <summary>
    /// La console de développement Antinomia. 
    /// Permet d'afficher des logs des actions. 
    /// Permet également quelques commandes élémentaires. 
    /// </summary>
    public class AntinomiaConsole : MonoBehaviourAntinomia {

        /// <summary>
        /// Le prefab d'un log.
        /// Devrait être un text. 
        /// </summary>
        public GameObject PrefabLog;

        /// <summary>
        /// Tous les strings en attente. 
        /// </summary>
        private List<string> AllStringToDebug = new List<string>();

        /// <summary>
        /// Etat de la console. Si true la console est active. 
        /// </summary>
        [HideInInspector]
        public bool state;

        /// <summary>
        /// Objet qui contient les logs.
        /// </summary>
        private GameObject Content;

        private ReportBug report;

        public override void Start() {
            base.Start();
            Content = transform.Find("ConsoleView").Find("Viewport").Find("Content").gameObject;
            if (PlayerPrefs.HasKey("user")) {
                report = new ReportBug("Joueur " + PlayerPrefs.GetString("user"));
            }
            else {
                report = new ReportBug();
            }

            DeactivateConsole();
        }

        public override void Update() {
            base.Update();
        }

        /// <summary>
        /// Ajouter un Log à la console. 
        /// </summary>
        /// <param name="log">Message du log</param>
        public void AddStringToConsole(string log) {
            if (state) {
                // Si la console est active on crée directement le log. 
                InstantiateOneStringConsole(log);
            }
            else {
                // Sinon on l'ajoute à la liste des string à instancier. 
                AllStringToDebug.Add(log);
            }
        }

        /// <summary>
        /// Desactiver la console.
        /// </summary>
        public void DeactivateConsole() {
            for (int i = 0; i < transform.childCount; ++i) {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            state = false;
        }

        /// <summary>
        /// Activer la console. 
        /// </summary>
        public void ActivateConsole() {
            for (int i = 0; i < transform.childCount; ++i) {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            InstantiateAllStringWaiting();
            state = true;
        }

        /// <summary>
        /// Lors de l'activation de la console, on instancie tous les strings qui
        /// étaient "en attente" dans la liste. 
        /// </summary>
        private void InstantiateAllStringWaiting() {
            for (int i = 0; i < AllStringToDebug.Count; ++i) {
                InstantiateOneStringConsole(AllStringToDebug[i]);
            }
            ClearWaitingList();
        }

        /// <summary>
        /// Créer un string dans la console.
        /// </summary>
        private void InstantiateOneStringConsole(string log) {
            GameObject NewLog = Instantiate(PrefabLog);
            NewLog.GetComponent<Text>().text = log;
            NewLog.transform.SetParent(Content.transform, false);
        }

        /// <summary>
        /// Remettre la liste des strings en attente à 0. 
        /// </summary>
        private void ClearWaitingList() {
            AllStringToDebug = new List<string>();
        }

        /// <summary>
        /// Executer une commande sur la console.
        /// </summary>
        public void ExecuteCommandConsole() {
            // On récupère d'abord le string
            string command = transform.Find("ConsoleView").Find("ConsoleWrite").gameObject.GetComponent<InputField>().text;
            string[] commandSplit = command.Split();

            switch (commandSplit[0]) {
                case "set_AKA":
                case "set_aka":
                    // Changer l'AKA du joueur
                    if (!checkParameters(2, commandSplit.Length)) {
                        return;
                    }
                    if (!checkIfParameterConsoleIsInt(commandSplit[1])) {
                        break;
                    }
                    FindLocalPlayer().GetComponent<Player>().PlayerAKA = int.Parse(commandSplit[1]);
                    AddStringToConsole("L'AKA de votre joueur a été mis à " + commandSplit[1]);
                    break;
                case "Pioche":
                case "pioche":
                    //Si le paramètre n'est pas un entier, on sort du switch.
                    if (!checkIfParameterConsoleIsInt(commandSplit[1])) {
                        break;
                    }
                    Debug.Log("Demande de pioche ICI");
                    StartCoroutine(GameObject.FindGameObjectWithTag("GameManager").
                        GetComponent<GameManager>().PiocheMultiple(int.Parse(commandSplit[1])));
                    break;
                case "format":
                case "Format":
                    if (!checkParameters(1, commandSplit.Length)) {
                        return;
                    }
                    AddStringToConsole("Format : Fonction parametre");
                    break;
                // Connaitre son ping. 
                case "ping":
                case "Ping":
                    if (!checkParameters(1, commandSplit.Length)) {
                        return;
                    }
                    // Connaitre la vitesse de sa connexion internet. 
                    StartCoroutine(ShowPing());
                    break;
                // Savoir si le jeu est en pause
                case "IsPause":
                case "is_pause":
                case "isPause":
                    if (!checkParameters(1, commandSplit.Length)) {
                        return;
                    }
                    bool pause = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().getGameIsPaused();
                    if (pause) {
                        AddStringToConsole("Le jeu est en pause");
                    }
                    else {
                        AddStringToConsole("Le jeu n'est pas en pause");
                    }
                    break;
                // Avoir la liste des fonctions que l'on peut faire dans la console. 
                case "liste_fonctions":
                case "liste_fct":
                case "ls_fct":
                    if (!checkParameters(1, commandSplit.Length)) {
                        return;
                    }
                    AddStringToConsole("set_aka, pioche, isPause, format, ping");
                    break;
                // Changer l'ascendance du terrain.
                case "set_terrain":
                case "terrain":
                    switch (commandSplit[1]) {
                        // Changement de terrain maléfique. 
                        case "malefique":
                        case "Malefique":
                            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().
                                SetAscendanceTerrain(GameManager.AscendanceTerrain.MALEFIQUE);
                            break;
                        case "astrale":
                        case "Astrale":
                        case "astral":
                            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().
                                SetAscendanceTerrain(GameManager.AscendanceTerrain.ASTRALE);
                            break;
                        case "none":
                        case "NONE":
                            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().
                                SetAscendanceTerrain(GameManager.AscendanceTerrain.NONE);
                            break;
                        default:
                            InstantiateOneStringConsole("Le terrain peut être changé grâce aux paramètres: astral, malefique, none");
                            break;
                    }
                    break;
                // Ajouter un bug au rapport de bug. 
                case "report":
                case "bug":
                case "report_bug":
                    string bug = "";
                    for (int i = 1; i < commandSplit.Length; i++) {
                        bug += commandSplit[i] + " ";
                    }
                    report.addBug(bug);
                    break;
                // Envoyer le rapport de bug. 
                case "send_report":
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ReportBugs();
                    break;
                case "activate_phase_button":
                case "phase_button":
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ReactivateButtonPhase();
                    break;
                // Changer les pvs deux deux joueurs.  
                case "set_PV":
                case "PV":
                case "set_pv":
                    FindLocalPlayer().GetComponent<Player>().CmdSetPlayerPV(int.Parse(commandSplit[1]));
                    break;
                default:
                    AddStringToConsole("Cette fonction n'existe pas");
                    break;
            }

            // On remet le texte à 0.
            transform.Find("ConsoleView").Find("ConsoleWrite").gameObject.GetComponent<InputField>().text = "";

        }

        /// <summary>
        /// Vérifie si le nombre de paramètres attendu dans la fonction console demandée est le bon. 
        /// </summary>
        /// <param name="attendu">Nombre de paramètres attendus</param>
        /// <param name="donne">Nombre de paramètres donnés</param>
        /// <returns>true, si attendu==donne, false sinon</returns>
        private bool checkParameters(int attendu, int donne) {
            if (attendu != donne) {
                AddStringToConsole("Format : Fonction parametre");
                return false;
            }
            else {
                return true;
            }
        }

        /// <summary>
        /// Verifie qu'un string peut bien être transformé en int. 
        /// </summary>
        /// <param name="parameter">un string de type "int"</param>
        private bool checkIfParameterConsoleIsInt(string parameter) {
            try {
                // On vérifie que l'argument est bien un entier.
                int.Parse(parameter);
            }
            catch (FormatException) {
                AddStringToConsole("Format : Fonction parametre");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Donne le ping du joueur aux serveurs de google.fr
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowPing() {

            Ping ping = new Ping("216.58.205.67");
            while (!ping.isDone) {
                yield return new WaitForSeconds(0.1f);
            }
            AddStringToConsole("Votre Ping aux serveurs google.fr : " + ping.time.ToString());
        }

        public void addAllBugsToGameSparksDataBase() {
            string allBugs = report.getAllReport();
            if (allBugs != null) {
                gameObject.AddComponent<GetGlobalInfoGameSparks>();
                StartCoroutine(GetComponent<GetGlobalInfoGameSparks>().WaitForComment(allBugs));
            }
        }

        /// <summary>
        /// Report un bug. 
        /// </summary>
        /// <param name="bug"></param>
        public void ReportABug(string bug) {
            report.addBug(bug);
        }

    }

}
