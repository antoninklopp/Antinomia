using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CarteZoom : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void updateInfo(string carteName, string shortCode, string carteInfo){
		/*
		 * Updater les informations de la carte zoom. 
		 * Lors d'un clic sur une carte spéciale. 
		 */ 
		transform.GetChild (0).gameObject.GetComponent<Text> ().text = carteInfo; 
		GetComponent<ImageCard> ().setImage (shortCode);
	}
}
