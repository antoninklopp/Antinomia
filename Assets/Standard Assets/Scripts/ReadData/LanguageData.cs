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
    /// </summary>
    /// <param name="language">Langue demandée</param>
    /// <param name="stringShortCode">shortCode du string, commun à toutes les langues.</param>
    public string GetString(string stringShortCode, string language = "english") {
        SetLanguage(language);
        XmlDocument xmlDoc = new XmlDocument();

        TextAsset textAsset = (TextAsset)Resources.Load(language, typeof(TextAsset));
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
