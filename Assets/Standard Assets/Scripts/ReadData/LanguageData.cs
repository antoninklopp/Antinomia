using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Xml;
using System.IO;

namespace LanguageModule {

    /// <summary>
    ///  Cette classe permet de récupérer les strings du jeu dans la langue choisir par le joueur. 
    /// </summary>
    public class LanguageData {

        /// <summary>
        /// Retourner un string dans une langue spéciale
        /// Les textes seront ceux à l'intérieur de l'application.
        /// </summary>
        /// <param name="language">Langue demandée</param>
        /// <param name="stringShortCode">shortCode du string, commun à toutes les langues.</param>
        /// <param name="nomFichier"> Nom du fichier à chercher.</param>
        public static string GetString(string stringShortCode, string language = "English", string nomFichier = "global") {
            XmlDocument xmlDoc = new XmlDocument();

            TextAsset textAsset;
            textAsset = (TextAsset)Resources.Load("XmlData/" + language + "/" + nomFichier, typeof(TextAsset));
            if (textAsset == null) {
                // Si le fichier n'existe pas. 
                textAsset = (TextAsset)Resources.Load("XmlData/" + "English" + "/" + nomFichier, typeof(TextAsset));
            }
            Debug.Log(textAsset); 
            xmlDoc.LoadXml(textAsset.text);
            XmlNodeList transformList = xmlDoc.GetElementsByTagName(stringShortCode)[0].ChildNodes;
            string toReturn = xmlDoc.SelectSingleNode("transforms/" + stringShortCode).InnerText;
            if (toReturn != null) {
                return toReturn;
            }
            else {
                throw new Exception("Le string n'a pas été trouvé");
            }
        }


    }

}
