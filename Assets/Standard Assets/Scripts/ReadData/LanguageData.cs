
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Xml;
using System.IO;

namespace LanguageModule {

    /// <summary>
    /// Cette classe permet de récupérer les strings du jeu dans la langue choisir par le joueur. 
    /// </summary>
    public class LanguageData {

        /// <summary>
        /// Get a string from the resources folder
        /// </summary>
        /// <param name="language">Language you want thr string in</param>
        /// <param name="stringShortCode">shortCode from the string (XML banner), must be the same
        /// for every string that represent same thing in different languages</param>
        /// <param name="fileName">Name of the file where is the stringShortCode banner</param>
        public static string GetString(string stringShortCode, string language, string fileName = "global") {
            XmlDocument xmlDoc = new XmlDocument();

            try {
                TextAsset textAsset;
                textAsset = (TextAsset)Resources.Load("XmlData/" + language + "/" + fileName, typeof(TextAsset));
                if (textAsset == null) {
                    // If file does not exist, we search it in English, in case it was not written for other languages. 
                    textAsset = (TextAsset)Resources.Load("XmlData/" + "English" + "/" + fileName, typeof(TextAsset));
                }
                xmlDoc.LoadXml(textAsset.text);
                XmlNodeList transformList = xmlDoc.GetElementsByTagName(stringShortCode)[0].ChildNodes;
                string toReturn = xmlDoc.SelectSingleNode("transforms/" + stringShortCode).InnerText;
                if (toReturn != null) {
                    return toReturn;
                }
                else {
                    throw new Exception("String not found");
                }
            } catch (NullReferenceException e) {
                throw new Exception("FileNotFoundException : Your File " + fileName + ".xml doesn't exist"); 
            }
        }

        /// <summary>
        /// Same method as <see cref="GetString(string, string, string)"</see> but the language used is the one
        /// from PlayerPrefs. /> 
        /// </summary>
        /// <param name="stringShortCode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetString(string stringShortCode, string fileName) {
            string language = PlayerPrefs.GetString("Language");
            return GetString(stringShortCode, language, fileName); 
        }

        /// <summary>
        /// Change Language from the Player
        /// </summary>
        /// <param name="Language"></param>
        public static void SetLanguage(string Language) {
            PlayerPrefs.SetString("Language", Language); 
        }

        /// <summary>
        /// Get Language from the Player
        /// </summary>
        /// <returns></returns>
        public static string GetLanguage() {
            return PlayerPrefs.GetString("Language"); 
        }
    }

}
