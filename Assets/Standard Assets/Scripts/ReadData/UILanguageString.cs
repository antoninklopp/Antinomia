using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LanguageModule {
    /// <summary>
    /// A simple string to put on GameObject. It changes automatically by language. 
    /// </summary>
    public class UILanguageString : MonoBehaviour {

        /// <summary>
        /// ShortCode of the string <see cref="LanguageData"/>
        /// </summary>
        public string stringToDisplay;

        /// <summary>
        /// FileName of the file where the string is.
        /// </summary>
        public string stringFileName = "global";

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