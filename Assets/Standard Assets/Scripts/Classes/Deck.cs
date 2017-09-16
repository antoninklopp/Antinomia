using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class Deck {
	/*
	 * Regroupe un ensemble de cartes. 
	 * En tant que gameObjects. 
	 * 
	 */ 

	public List<GameObject> Cartes = new List<GameObject>(); 
	public int number; 

	public Deck(List<GameObject> newCartes, int thisNumber){
		Cartes = newCartes;
		number = thisNumber; 
	}

	public Deck(){
	
	}

	public void AjouterCarte(GameObject Carte){
		/*
		 * Ajouter une carte au deck
		 * 
		 */ 
		Cartes.Add (Carte); 
	}

	public void EnleverCarte(GameObject Carte){
		/*
		 * Enlever une carte d'un deck. 
		 */ 
		Cartes.Remove (Carte); 
	}
}
