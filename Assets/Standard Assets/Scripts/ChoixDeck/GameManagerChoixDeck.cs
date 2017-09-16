using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManagerChoixDeck : MonoBehaviour {
	
	/*
	 * Avant d'entrer dans le lobby, le joueur choisit avec quel deck il veut jouer. 
	 */ 

	int choixDeck = 0; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClickDeck1(){
		/*
		 * Choix du deck 1. 
		 */ 
		choixDeck = 1; 
	}

	public void OnClickDeck2(){
		/*
		 * Choix du deck 2
		 */ 
		choixDeck = 2; 
	}

	public void OnClickDeck3(){
		/*
		 * Choix du deck 3
		 */ 
		choixDeck = 3; 
	}

	public void OnClickAllCards(){
		/*
		 * Choix: toutes les cartes
		 */ 

		choixDeck = 0; 
	}

	public void Go(){
		/*
		 * Aller dans le lobby.
		 */ 
		PlayerPrefs.SetInt ("ChoixDeck", choixDeck); 
		SceneManager.LoadScene ("SimpleLobby"); 

	}
}
