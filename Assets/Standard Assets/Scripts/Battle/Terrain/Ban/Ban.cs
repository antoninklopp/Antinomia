
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

/// <summary>
/// Zone de Ban
/// </summary>
public class Ban : NetworkBehaviourAntinomia {

    /// <summary>
    /// Liste de toutes les cartes Ban
    /// </summary>
    List<GameObject> AllCartesBan = new List<GameObject>();

    /// <summary>
    /// Prefab de la carte. 
    /// </summary>
    public GameObject CartePrefab;

    /// <summary>
    /// Nombre de cartes bannies face cachée. 
    /// </summary>
    public int NombreCartesBanniesFaceCachee;

    public override void Start() {
        base.Start(); 
        // On rotate, pour que le texte soit dans le bon sens. 
        if (transform.parent.parent.GetComponent<Player>().isLocalPlayer) {
            transform.parent.Find("BanText").transform.Rotate(new Vector3(0, 180, 180)); 
        }
    }

    /// <summary>
    /// Reordonner les cartes du cimetiere. 
    /// </summary>
    void CmdReordonnerCarte() {
        /*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */

        AllCartesBan = new List<GameObject>();
        foreach (Transform child in transform) {
            AllCartesBan.Add(child.gameObject);
        }
        for (int i = 0; i < AllCartesBan.Count; i++) {
            // On met toutes les cartes au même endroit. 
            AllCartesBan[i].transform.localPosition = Vector3.zero;
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
    public void CmdCarteDeposee(GameObject NewCard) {
        /*
		 * Depot d'une carte sur le board, pour l'instant aucune vérification n'est faite. 
		 * TODO: Vérifier s'il est possible de poser la carte en question sur le board. 
		 * 
		 */

        Debug.Log("Carte deposee dans le ban");

        // On change le parent de la carte. 
        NewCard.transform.SetParent(transform);
        // Puis on réorganise l'affichage.
        AllCartesBan.Add(NewCard);
        CmdReordonnerCarte();

        // Et on change le statut de la carte de main à cimetière. 
        if (NewCard.GetComponent<Entite>() != null) {
            NewCard.SendMessage("setState", "BAN");
            NewCard.SendMessage("setClicked", false);
        }
    }

    public void AjouterUneCarteBannieFaceCachee() {
        NombreCartesBanniesFaceCachee++; 
    }
}
