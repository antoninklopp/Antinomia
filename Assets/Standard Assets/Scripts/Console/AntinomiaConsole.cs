using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 

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

    public override void Start() {
        base.Start();
        Content = transform.Find("ConsoleView").Find("Viewport").Find("Content").gameObject;


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
        } else {
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

        //On vérifie que le nombre de paramètre est bien égal à 2, la fonction puis le paramètre. 
        // Debug.Assert(commandSplit.Length == 2);

        if (commandSplit.Length != 2) {
            AddStringToConsole("Format : Fonction parametre");
            
        }
        else {

            switch (commandSplit[0]) {
                case "set_AKA":
                case "set_aka":
                    // Changer l'AKA du joueur

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
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PiocheMultiple(int.Parse(commandSplit[1]));
                    break;
                case "format":
                case "Format":
                    AddStringToConsole("Format : Fonction parametre");
                    break;
                case "ping":
                case "Ping":
                    // Connaitre la vitesse de sa connexion internet. 
                    StartCoroutine(ShowPing()); 
                    break; 
                default:
                    AddStringToConsole("Cette fonction n'existe pas");
                    break;
            }
        }

        // On remet le texte à 0.
        transform.Find("ConsoleView").Find("ConsoleWrite").gameObject.GetComponent<InputField>().text = ""; 

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
        catch (FormatException e) {
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

}
