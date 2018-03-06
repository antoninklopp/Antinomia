
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoice : MonoBehaviour {
    /*
     * Lors de l'arrivée d'un joueur sur le serveur, 
     * il peut avoir choisi d'être joueur ou de regarder une game. 
     * 
     * S'il choisit de regarder une game, 
     * (très utile pour comprendre les techniques de jeu de tous les joueurs), 
     * on instancie juste un joueur "vide", 
     * qui récupèrera toutes les données sur le serveur sans en envoyer. 
     * 
     */ 

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.HasKey("isViewer")) {
            if (PlayerPrefs.GetString("isViewer") == "true") {
                // Si le joueur a choisi d'être un viewer. 
                GetComponent<Player>().enabled = false; 
                for (int i = 0; i < transform.childCount; ++i) {
                    // on désactive tous les child objects. 
                    transform.GetChild(i).gameObject.SetActive(false); 
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
