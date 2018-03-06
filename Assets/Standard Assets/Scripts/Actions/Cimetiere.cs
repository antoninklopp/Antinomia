
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 


/// <summary>
/// Objet représentant le cimetiere d'un joueur. 
/// </summary>
public class Cimetiere : NetworkBehaviour {
	/*
	 * Toutes les cartes envoyées au cimetiere par un joueur. 
	 */ 

	List<GameObject> AllCreaturesCimetiere = new List<GameObject> (); 
	public GameObject CartePrefab; 

    /// <summary>
    /// Reordonner les cartes du cimetiere. 
    /// </summary>
	void CmdReordonnerCarte(){
		/*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */ 
		Debug.Log ("Reordonner Cimetiere");

		AllCreaturesCimetiere = new List<GameObject> (); 
		foreach (Transform child in transform) {
			AllCreaturesCimetiere.Add (child.gameObject); 
		}
		for (int i = 0; i < AllCreaturesCimetiere.Count; i++) {
			// On met toutes les cartes au même endroit. 
			AllCreaturesCimetiere [i].transform.localPosition = Vector3.zero; 

		}
	}

    /// <summary>
    /// Lors d'un clic sur le cimetiere. 
    /// </summary>
	void OnMouseDown(){
		// Lorsque la souris touche le board. 


	}

    /// <summary>
    /// Recuperer le nombre de cartes dans le cimetiere
    /// </summary>
    /// <returns>Nombre de cartes dans le cimetiere</returns>
    public int NombreDeCartesDansCimetiere() {
        return transform.childCount; 
    }

    /// <summary>
    /// Deposer une carte dans le cimetiere
    /// </summary>
    /// <param name="NewCard">Objet carte déposé</param>
	[Command(channel=0)]
	void CmdCarteDeposee(GameObject NewCard){
        /*
		 * Depot d'une carte sur le board, pour l'instant aucune vérification n'est faite. 
		 * TODO: Vérifier s'il est possible de poser la carte en question sur le board. 
		 * 
		 */

        Debug.Log("Carte deposee dans le cimetiere"); 

		// On change le parent de la carte. 
		NewCard.transform.SetParent (transform);
		// Puis on réorganise l'affichage.
		AllCreaturesCimetiere.Add(NewCard); 
		CmdReordonnerCarte();

        // Et on change le statut de la carte de main à cimetière. 
        if (NewCard.GetComponent<Entite>() != null) {
            Debug.Log("La carte est maintenant au cimetiere"); 
            NewCard.SendMessage("setState", "CIMETIERE");
            NewCard.SendMessage("setClicked", false);
        }
	}
}
