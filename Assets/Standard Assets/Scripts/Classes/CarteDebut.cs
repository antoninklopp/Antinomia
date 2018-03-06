
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Carte montrée au début d'une game, pour permettre au joueur de choisir s'il veut garder ou non
/// sa main initiale. 
/// </summary>
public class CarteDebut : MonoBehaviour {

    public string StringToDisplay;
    public string shortCode; 

    public CarteDebut() {


    }

    public CarteDebut(string _shortCode, string _StringToDisplay) {
        shortCode = _shortCode;
        StringToDisplay = _StringToDisplay;

    }

    /// <summary>
    /// Après création de la classe, on met les infos à jour. 
    /// </summary>
    /// <param name="_shortCode">Le shortCode de la classe</param>
    /// <param name="_StringToDisplay">Contient les informations essentielles de la classe</param>
    public void InfoDebut(string _shortCode, string _StringToDisplay) {
        shortCode = _shortCode;
        StringToDisplay = _StringToDisplay;
        GetComponent<ImageCardToShow>().setImage(shortCode);
    }

    public void OnClickCarte() {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ShowCarteInfo(shortCode, StringToDisplay);
    }
}
