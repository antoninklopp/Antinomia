
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TirerCarte : MonoBehaviour {

	/*
	 * Script sur le deck. 
	 * Action de tirer une carte. 
	 */ 
	public int NumeroJoueur; 
	bool isAllowed = true; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown(){
		/*
		 * Lors du clic sur le deck. 
		 */ 
		//NewCard (); 
		GameObject.Find("Joueur(Clone)").SendMessage("SpawnCard"); 
	}

	void NewCard(){
		if (isAllowed) {
			// Si il est possible de tirer une carte. 
			if (NumeroJoueur == 1) {
				// Deck du joueur 1. 
				GameObject.Find ("CartesMainJoueur1").SendMessage ("TirerCarte");
				//isAllowed = false; 
			}
		}
	}

	public void allowNewCard(){
		isAllowed = true; 
	}
}
