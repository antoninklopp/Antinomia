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


	// Use this for initialization
	void Start () {

    }

    public void InfoDebut(string _shortCode, string _StringToDisplay) {
        shortCode = _shortCode;
        StringToDisplay = _StringToDisplay;
        GetComponent<ImageCardToShow>().setImage(shortCode);
    }

    public void OnClickCarte() {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ShowCarteInfo(shortCode, StringToDisplay);
    }
}
