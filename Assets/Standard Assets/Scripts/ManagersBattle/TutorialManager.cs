
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using LanguageModule;
using AntinomiaException; 

/// <summary>
/// Composant attaché au GameManager du tutorial. 
/// Utile pour expliquer au début ce qu'il faut jouer. 
/// </summary>
public class TutorialManager : MonoBehaviour {

    public GameObject ParticleSystemTutorial; 

    private GameObject TextDisplay = null; 

	void Start () {
        // On vérifie que le particle system soit bien présent. 
        if (ParticleSystemTutorial == null) {
            throw new UnusualBehaviourException("Il manque un compsant de particle system ici. ");
        }
        if (!PlayerPrefs.HasKey("TutorialActivated")) {
            PlayerPrefs.SetInt("TutorialActivated", 0); 
        }
        // Dans le cas où le joueur ne souhaite voir aucun tutoriel on détruit le composant
        else if (PlayerPrefs.GetInt("TutorialActivated") == 0) {
            Destroy(this); 
        }
	}
	
	public void SetTutorialInformation(string message) {
        if (TextDisplay == null) {
            TextDisplay = GetComponent<InformationManager>().GetTextDisplay(); 
        }
        TextDisplay.SetActive(true); 
        TextDisplay.GetComponent<Text>().text = message;
        // Normalement, rien à faire il devrait tout faire seul. 
        // On le détruit après sa durée
        GameObject PartInstantiated = Instantiate(ParticleSystemTutorial);
        float particleDuration = 5f; 
        Destroy(PartInstantiated, particleDuration);
        StartCoroutine(DisableTextDisplay(particleDuration)); 
    }

    public IEnumerator DisableTextDisplay(float time) {
        yield return new WaitForSeconds(time);
        TextDisplay.SetActive(false); 
    }

    public void SetTutorialInformation(Player.Phases phase) {
        string message = LanguageData.GetString(phase.ToString() + "TUTORIAL", PlayerPrefs.GetString("Language"), "tutorial");
        SetTutorialInformation(message);
    }
}
