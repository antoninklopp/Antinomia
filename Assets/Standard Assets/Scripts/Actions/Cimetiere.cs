using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class Cimetiere : NetworkBehaviour {
	/*
	 * 
	 * Toutes les cartes envoyées au cimetiere par un joueur. 
	 * 
	 */ 

	List<GameObject> AllCreaturesCimetiere = new List<GameObject> (); 
	public GameObject CartePrefab; 

	void CmdReordonnerCarte(){
		/*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */ 
		print ("Reordonner Cimetiere");

		AllCreaturesCimetiere = new List<GameObject> (); 
		foreach (Transform child in transform) {
			AllCreaturesCimetiere.Add (child.gameObject); 
		}
		for (int i = 0; i < AllCreaturesCimetiere.Count; i++) {
			// On met toutes les cartes au même endroit. 
			AllCreaturesCimetiere [i].transform.localPosition = Vector3.zero; 

		}
	}

	void OnMouseDown(){
		// Lorsque la souris touche le board. 


	}

    public int NombreDeCartesDansCimetiere() {
        return transform.childCount; 
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
		AllCreaturesCimetiere.Add(NewCard); 
		CmdReordonnerCarte(); 

		print ("deposer!"); 

		// Et on change le statut de la carte de main à board. 
		NewCard.SendMessage("setState", "CIMETIERE"); 
		NewCard.SendMessage ("setClicked", false); 
	}
}
