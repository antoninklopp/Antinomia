using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Xml;
using System.IO;

/// <summary>
///  Cette classe permet de récupérer les strings du jeu dans la langue choisir par le joueur. 
/// </summary>
public class LanguageData {

    /// <summary>
    /// Langue de l'utilisateur
    /// </summary>
    string LanguageName;

    public void SetLanguage(string _Language) {
        LanguageName = _Language;
    }

    /// <summary>
    /// Retourner un string dans une langue spéciale
    /// Les textes seront ceux à l'intérieur de l'application.
    /// </summary>
    /// <param name="language">Langue demandée</param>
    /// <param name="stringShortCode">shortCode du string, commun à toutes les langues.</param>
    /// <param name="nomFichier"> Nom du fichier à chercher. De base rien pour les textes à l'intérieur de l'appli
    /// Peut prendre d'autres valeurs pour rassembler des strings de quêtes par exemple.
    /// Le nom du fichier sera forcément fichier_langue </param>
    public string GetString(string stringShortCode, string language = "english", string nomFichier = "") {
        SetLanguage(language);
        XmlDocument xmlDoc = new XmlDocument();

        TextAsset textAsset;
        if (nomFichier == "") {
           textAsset = (TextAsset)Resources.Load(language, typeof(TextAsset));
        } else {
            textAsset = (TextAsset)Resources.Load(nomFichier + "_" + language, typeof(TextAsset));
        }
        xmlDoc.LoadXml(textAsset.text);
        XmlNodeList transformList = xmlDoc.GetElementsByTagName(stringShortCode)[0].ChildNodes;
        string toReturn = xmlDoc.SelectSingleNode("transforms/" + stringShortCode).InnerText; 
        if (toReturn != null) {
            return toReturn;
        } else {
            throw new Exception("Le string n'a pas été trouvé"); 
        }
    }


}
