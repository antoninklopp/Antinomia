using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LanguageModule {
    /// <summary>
    /// Juste un string UI qui change de texte en fonction de la langue? 
    /// </summary>
    public class UILanguageString : MonoBehaviour {

        public string stringToDisplay;
        public string stringFileName = "global";

        // Use this for initialization
        void Start() {
            if (stringToDisplay == null || stringToDisplay == "") {
                Debug.LogWarning("L'objet " + name + " n'a pas de string associé"); 
                return; 
            }

            if (PlayerPrefs.HasKey("Language")) {
                gameObject.GetComponent<Text>().text = LanguageData.GetString(stringToDisplay, PlayerPrefs.GetString("Language"), 
                    stringFileName);
            } else {
                gameObject.GetComponent<Text>().text = LanguageData.GetString(stringToDisplay, 
                    Application.systemLanguage.ToString(), stringFileName);
            }
        }

    }
}