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
public class EventManager : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
    }

    /// <summary>
    /// Créer un nouvel ensemble d'events
    /// </summary>
    public void CreerNouvellePileEvent(List<EventEffet> allEvent) {
        // S'il n'y a qu'un seul évênement, on ne propose pas de choisir
        if (allEvent.Count == 1) {
            allEvent[0].Jouer();
            return; 
        }
        EventTotal = allEvent.Count;
        SetUpButtons(); 
    }


    /// <summary>
    /// Créer tous les boutons de choix des events
    /// </summary>
    public void SetUpButtons() {
        for (int i = 0; i < EventTotal; i++) {
            GameObject newButton = Instantiate(EventButtonPrefab);
            newButton.transform.SetParent(transform, false);
            int number = i;
            // Changement de deck. 
            newButton.GetComponent<Button>().onClick.AddListener(delegate { CliqueEffet(number); });

            listeButtons.Add(newButton);
        }
    }


}
