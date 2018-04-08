using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using LanguageModule;
using System; 

public class GameManagerSettings : MonoBehaviour {

    /// <summary>
    /// Canvas group où sont stockés les boutons de changement de langues.
    /// </summary>
    private GameObject LanguageDropDown;

    private GameObject CurrentLanguage;

    /// <summary>
    /// On garde une variable pour retenir si le dropDownLanguage a déjà été set
    /// Comme ça, inutile de le refaire. 
    /// </summary>
    private bool DropDownSet = false; 


	// Use this for initialization
	void Start () {
        LanguageDropDown = GameObject.Find("AllLanguages");
        LanguageDropDown.SetActive(false);
        CurrentLanguage = GameObject.Find("CurrentLanguage");
        CurrentLanguage.transform.Find("Text").gameObject.GetComponent<Text>().text = PlayerPrefs.GetString("Language"); 
	}

    /// <summary>
    /// Lorsqu'on clique sur le bouton de la langue courante, 
    /// on fait apparaitre le menu déroulant du choix des langues. 
    /// </summary>
    public void CurrentLanguageClick() {
        CurrentLanguage.SetActive(false);
        LanguageDropDown.SetActive(true);

        if (DropDownSet) {
            return; 
        }

        // On récupère le nombre de langues disponibles actuellement. 
        int availableLanguages = 0;
        try {
            availableLanguages = int.Parse(LanguageData.GetString("available", "availableLanguages")); 
        } catch (FormatException) {
            Debug.LogError("Erreur conversion"); 
        }

        for (int i = 1; i <= availableLanguages; i++) {
            GameObject newButton = Instantiate(Resources.Load("Prefabs/ButtonLanguageSettings") as GameObject);
            string Language = LanguageData.GetString("Langue" + i.ToString(), "availableLanguages");
            newButton.transform.SetParent(LanguageDropDown.transform, false); 
            newButton.transform.Find("Text").gameObject.GetComponent<Text>().text = Language; 
            newButton.GetComponent<Button>().onClick.AddListener(delegate {
                ChangeLanguage(Language);
            });
        }

        DropDownSet = true; 
    }

    /// <summary>
    /// Changer la langue du jeu. 
    /// Callback d'un bouton. 
    /// </summary>
    /// <param name="Language"></param>
    public void ChangeLanguage(string Language) {
        PlayerPrefs.SetString("Language", Language);
        CurrentLanguage.SetActive(true);
        LanguageDropDown.SetActive(false);
        CurrentLanguage.transform.Find("Text").gameObject.GetComponent<Text>().text = Language; 
    }
}
