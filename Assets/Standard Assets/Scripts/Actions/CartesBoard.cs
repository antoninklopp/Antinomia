﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class CartesBoard : NetworkBehaviour {

	/*
	 * Il peut y avoir au maximum 5 cartes sur le board. 
	 * 
	 * Abus de langage : Ici BOARD = CHAMP DE BATAILLE. 
	 */

	List<Creature> AllCreatures = new List<Creature>(); 
	List<GameObject> AllCreaturesChampBataille = new List<GameObject>();

	public GameObject CartePrefab; 

	private float OffsetCarteChampBataille = 0.2f;

	// Savoir quelle carte est en train d'être touchée. 
	public bool carteCurrentlyClicked; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PoserCarteChampBataille(){//Carte CarteAPoser){
		GameObject NouvelleCarte = Instantiate (CartePrefab); 
		NouvelleCarte.transform.SetParent (transform); 
		//AllCreaturesChampBataille.Add (NouvelleCarte); 
		CmdReordonnerCarte ();
	}

	void CmdReordonnerCarte(){
		/*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */ 
		print ("Reordonner ChampBataille");

		AllCreaturesChampBataille = new List<GameObject> (); 
		foreach (Transform child in transform) {
            AllCreaturesChampBataille.Add(child.gameObject);
		}
		if (AllCreaturesChampBataille.Count == 1) {
			// Si la carte ajoutée est la première carte, on la met au centre du board. 
			AllCreaturesChampBataille [0].transform.localPosition = new Vector3 (0, 0, 0);
		} else {
			for (int i = 0; i < AllCreaturesChampBataille.Count; i++) {
				// Sinon on les décale toutes vers la gauche et on insère à droite. 
				//AllCreaturesChampBataille [i].transform.localPosition = 
				//	new Vector3 (AllCreaturesChampBataille [i].transform.localPosition.x - Carte.GetComponent<BoxCollider2D>().size.x/2 - OffsetCarteChampBataille , 0, 0); 
				AllCreaturesChampBataille [i].transform.localPosition = 
					new Vector3 (2*(-(int)AllCreaturesChampBataille.Count/2 + i)*CartePrefab.GetComponent<BoxCollider2D>().size.x*CartePrefab.transform.localScale.x - OffsetCarteChampBataille , 0, 0);

			}
			// On insère la dernière carte à droite. 
			//AllCreaturesChampBataille[AllCreaturesChampBataille.Count - 1].transform.localPosition = 
			//	new Vector3(AllCreaturesChampBataille[AllCreaturesChampBataille.Count - 2].transform.localPosition.x + Carte.GetComponent<BoxCollider2D>().size.x + OffsetCarteChampBataille * 2, 0, 0);
		}
	}

	void OnMouseDown(){
		// Lorsque la souris touche le board. 


	}

	[Command]
	void CmdCarteDeposee(GameObject NewCard){
		/*
		 * Depot d'une carte sur le board, pour l'instant aucune vérification n'est faite. 
		 * TODO: Vérifier s'il est possible de poser la carte en question sur le board. 
		 * 
		 */ 

		print ("deposee ChampBataille");

		// On change le parent de la carte. 
		NewCard.transform.SetParent (transform);
		// Puis on réorganise l'affichage.
		AllCreaturesChampBataille.Add(NewCard); 
		CmdReordonnerCarte(); 

		print ("deposer!");

        // Et on change le statut de la carte de main à board. 
        if (NewCard.GetComponent<CarteType>().thisCarteType == CarteType.Type.ENTITE) {
            NewCard.SendMessage("setState", "BOARD");
            NewCard.SendMessage("setClicked", false);
        }
	}

	public int getNumberCardsChampBataille(){
		/*
		 * Récupérer le nombre de cartes actuellement présentes sur le board
		 */ 
		AllCreaturesChampBataille = new List<GameObject> (); 
		foreach (Transform child in transform) {
			AllCreaturesChampBataille.Add (child.gameObject); 
		}

		return AllCreaturesChampBataille.Count;  
	}

}