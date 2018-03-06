using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AntinomiaException;
using System; 

/// <summary>
/// Gestion des evenements s'il y a plusieurs effets à faire en même temps. 
/// Script attaché à un objet qui représente cet event manager. 
/// </summary>
public class EventManager : MonoBehaviourAntinomia {

    private List<GameObject> listeButtons;

    public GameObject EventButtonPrefab; 

    List<EventEffet> listeEvents = new List<EventEffet>();

    /// <summary>
    /// Entier utilisé pour l'effet (choix de l'ordre). 
    /// </summary>
    private int EventCourant;

    /// <summary>
    /// Nombre total d'event. 
    /// </summary>
    public int EventTotal;

    private bool EffetFini = false;

    private GameObject ButtonFin;

    /// <summary>
    /// Passe à true quand TOUS les effets qui doivent être joués sont finis.
    /// </summary>
    private bool TousEffetsFini = true; 

    /// <summary>
    /// Bouton qui permet de remettre l'ordre des effets à 0
    /// Dans le cas où le joueur s'est trompé et veut changer l'ordre des effets. 
    /// </summary>
    private GameObject ResetOrdreButton; 

	// Use this for initialization
	public override void Start () {
        ButtonFin = GameObject.Find("FinEventManager");
        ButtonFin.SetActive(false);
        ResetOrdreButton = GameObject.Find("ResetOrdre");
        ResetOrdreButton.SetActive(false); 
	}

    public void AjouterEffet(EventEffet ef) {
        listeEvents.Add(ef);
    }

    public void EnleverEffet(EventEffet ef) {
        listeEvents.Remove(ef); 
    }

    /// <summary>
    /// Choix de l'ordre d'un effet. 
    /// Fonction delegate des boutons. 
    /// </summary>
    public void CliqueEffet(int nombre) {
        listeEvents[nombre].EventInt = EventCourant;
        EventCourant++;

        listeButtons[nombre].transform.Find("Number").gameObject.GetComponent<Text>().text = EventCourant.ToString(); 
    }

    /// <summary>
    /// Appui sur le bouton de fin de choix
    /// Il faut vérifier que tous les effets aient bien été choisis. 
    /// </summary>
    public void ChoixFini() {
        if(EventCourant != EventTotal) {
            GameObject.Find("GameManager").GetComponent<GameManager>().DisplayMessage("Vous n'avez pas tout choisi"); 
        }
        ButtonFin.SetActive(false);
        StartCoroutine(JouerEffets()); 
    }

    /// <summary>
    /// Créer un nouvel ensemble d'events
    /// </summary>
    public void CreerNouvellePileEvent() {
        // S'il n'y a qu'un seul évênement, on ne propose pas de choisir
        if (listeEvents.Count == 0) {
            Debug.Log("Il n'y a pas d'effets dans l'eventManager"); 
            return; 
        }

        if (EffetsDemandeInteraction() < 1) {
            Debug.Log("Il n'y a qu'un seul effet dans l'eventManager"); 
            JouerUnEffet(listeEvents[0]); 
            return; 
        }

        ButtonFin.SetActive(true);
        ResetOrdreButton.SetActive(true);
        EventTotal = listeEvents.Count;
        SetUpButtons();
    }


    /// <summary>
    /// Créer tous les boutons de choix des events
    /// </summary>
    public void SetUpButtons() {
        Debug.Log("On crée les bouttons"); 
        listeButtons = new List<GameObject>(); 
        for (int i = 0; i < EventTotal; i++) {
            GameObject newButton = Instantiate(EventButtonPrefab);
            newButton.transform.SetParent(transform, false);
            int number = i;
            // Changement de deck. 
            Debug.Log(1); 
            newButton.GetComponent<Button>().onClick.AddListener(delegate { CliqueEffet(number); });
            // On informe le joueur de l'effet qui peut être joué ainsi que de la carte qui peut le jouer. 
            newButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = listeEvents[i].effet.EffetString + "\n" + 
                "<color=purple>" + listeEvents[i].CarteAssociee.GetComponent<Carte>().Name + "</color>";
            Debug.Log(2); 
            listeButtons.Add(newButton);
        }
    }

    /// <summary>
    /// Reset tous les evenements. 
    /// </summary>
    public void Reset() {
        listeEvents = new List<EventEffet>();
        EventCourant = 0;
        EventTotal = 0; 
    }

    /// <summary>
    /// Jouer tous les effets dans l'eventManager. 
    /// A la fin on reset l'eventManager
    /// </summary>
    public IEnumerator JouerEffets() {
        // On détruit tous les boutons pour qu'ils ne soient plus visibles. 
        TousEffetsFini = false; 
        Debug.Log("On joue les effets"); 
        foreach (GameObject o in listeButtons) {
            Destroy(o); 
        }
        listeButtons = new List<GameObject>();

        ButtonFin.SetActive(false); 
        ResetOrdreButton.SetActive(false); 

        // On joue les effets dans l'ordre de tri
        for (int i = 0; i < EventTotal; i++) {
            // On met TousEffetsFini à true avant de jouer le dernier effet pour que le joueur adverse puisse répondre.
            if (i == EventTotal - 1) {
                // On a besoin d'avoir changer l'entier d'effet fini pour une vérification dans la pile. 
                yield return ChangerEffetFiniJoueur(); 
                TousEffetsFini = true; 
            }
            // On attend la fin de l'effet avant de passer au suivant.
            JouerUnEffet(FindEffet(i)); 
            while (!EffetFini) {
                yield return new WaitForSeconds(0.2f); 
            }
            Debug.Log("<color=purple> Un effet est fini</color>"); 
            EffetFini = false; 
        }
        Reset();
    }

    /// <summary>
    /// Une fois que tous les effets ont été vérifiés chez un joueur, on vérifie tous les effets chez l'autre
    /// </summary>
    /// <returns>None</returns>
    private IEnumerator ChangerEffetFiniJoueur() {
        int entierAttendu = 0; 
        // Et on passe l'entier au bon nombre (dans le Player). 
        if (FindLocalPlayer().GetComponent<Player>().EffetPlayer == 0) {
            // Dans le cas où c'était à nous de jouer en premier.
            entierAttendu = FindLocalPlayer().GetComponent<Player>().PlayerID;
        }
        FindLocalPlayer().GetComponent<Player>().CmdOnEffetPlayer(entierAttendu);

        // On vérifie que l'information est bien arrivée à l'autre joueur. 

        int decompte = 0; 
        while (FindNotLocalPlayer().GetComponent<Player>().EffetPlayer != entierAttendu && decompte < 10) {
            yield return new WaitForSeconds(0.1f);
            decompte++; 
        }

        // Dans ce cas l'information n'est pas arrivée. 
        if (decompte == 10) {
            // On rappelle la meme methode. 
            Debug.LogError("Probleme lors de l'envoi des informations");
            StartCoroutine(ChangerEffetFiniJoueur()); 
        }
    }

    /// <summary>
    /// Trouve l'effet dans la liste correspondant au nombre recherché. 
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private EventEffet FindEffet(int number) {
        foreach (EventEffet ef in listeEvents) {
            if (ef.EventInt == number) {
                return ef; 
            }
        }
        throw new UnusualBehaviourException("L'effet n'a pas été trouvé");
    }

    /// <summary>
    /// Passe EffetFini à true lorsqu'on trouve la fin d'un effet. 
    /// </summary>
    public void EffetFiniOK() {
        EffetFini = true; 
    }

    /// <summary>
    /// Remettre l'ordre des event à 0. 
    /// </summary>
    public void ResetOrdre() {
        EventCourant = 0;
        foreach (GameObject o in listeButtons) {
            o.transform.Find("Number").gameObject.GetComponent<Text>().text = ""; 
        }
    }

    /// <summary>
    /// Renvoie le nombre d'effets qui demandent un
    /// </summary>
    /// <returns></returns>
    private int EffetsDemandeInteraction() {
        int somme = 0; 
        foreach (EventEffet ef in listeEvents) {
            Debug.Log("Il y a un effet ici"); 
            if (ef.demandeInteraction) {
                somme++; 
            }
        }
        return somme; 
    }

    /// <summary>
    /// Renvoi true si tous les effets sont finis. 
    /// </summary>
    /// <returns></returns>
    public bool IsAllEffetsFini() {
        return TousEffetsFini; 
    }

    private void JouerUnEffet(EventEffet ef) {
        // il faut recréer une liste d'effets avec effet dans la bonne position, pour qu'il n'y ait pas de problèems
        // lors de la transmission des infos..
        // On mettra donc des elements à null pour compléter
        StartCoroutine(ef.CarteAssociee.GetComponent<Carte>().MettreEffetDansLaPileFromActions(
            ef.effet.numeroEffet, numeroListEffet: ef.effet.numeroListEffet
        ));
    }
}
