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
            return; 
        }

        if (listeEvents.Count == 1) {
            listeEvents[0].Jouer();
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
        listeButtons = new List<GameObject>(); 
        for (int i = 0; i < EventTotal; i++) {
            GameObject newButton = Instantiate(EventButtonPrefab);
            newButton.transform.SetParent(transform, false);
            int number = i;
            // Changement de deck. 
            Debug.Log(1); 
            newButton.GetComponent<Button>().onClick.AddListener(delegate { CliqueEffet(number); });
            newButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = listeEvents[i].effet.EffetString;
            Debug.Log(2); 
            listeButtons.Add(newButton);
        }
    }

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
        Debug.Log("On joue les effets"); 
        foreach (GameObject o in listeButtons) {
            Destroy(o); 
        }
        listeButtons = new List<GameObject>();

        ButtonFin.SetActive(false); 
        ResetOrdreButton.SetActive(false); 

        // On joue les effets dans l'ordre de tri
        for (int i = 0; i < EventTotal; i++) {
            // On attend la fin de l'effet avant de passer au suivant.
            FindEffet(i).Jouer();
            while (!EffetFini) {
                yield return new WaitForSeconds(0.2f); 
            }
            EffetFini = false; 
        }
        Reset(); 
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


}
