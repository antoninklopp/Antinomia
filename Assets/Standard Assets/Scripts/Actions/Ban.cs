﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

/// <summary>
/// Zone de Ban
/// </summary>
public class Ban : NetworkBehaviourAntinomia {

    List<GameObject> AllCreaturesBan = new List<GameObject>();
    public GameObject CartePrefab;

    /// <summary>
    /// Reordonner les cartes du cimetiere. 
    /// </summary>
    void CmdReordonnerCarte() {
        /*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */
        print("Reordonner Ban");

        AllCreaturesBan = new List<GameObject>();
        foreach (Transform child in transform) {
            AllCreaturesBan.Add(child.gameObject);
        }
        for (int i = 0; i < AllCreaturesBan.Count; i++) {
            // On met toutes les cartes au même endroit. 
            AllCreaturesBan[i].transform.localPosition = Vector3.zero;

        }
    }

    /// <summary>
    /// Lors d'un clic sur le cimetiere. 
    /// </summary>
	void OnMouseDown() {
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
	[Command]
    void CmdCarteDeposee(GameObject NewCard) {
        /*
		 * Depot d'une carte sur le board, pour l'instant aucune vérification n'est faite. 
		 * TODO: Vérifier s'il est possible de poser la carte en question sur le board. 
		 * 
		 */

        Debug.Log("Carte deposee dans le ban");

        // On change le parent de la carte. 
        NewCard.transform.SetParent(transform);
        // Puis on réorganise l'affichage.
        AllCreaturesBan.Add(NewCard);
        CmdReordonnerCarte();

        // Et on change le statut de la carte de main à cimetière. 
        if (NewCard.GetComponent<Entite>() != null) {
            NewCard.SendMessage("setState", "CIMETIERE");
            NewCard.SendMessage("setClicked", false);
        }
    }
}