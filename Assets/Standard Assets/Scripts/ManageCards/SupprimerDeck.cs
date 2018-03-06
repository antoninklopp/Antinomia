
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupprimerDeck : MonoBehaviour {

    /// <summary>
    /// Reponse du joueur à la demande de suppression de son deck.
    /// </summary>
    /// <param name="reponse">1 = oui, 2 = non</param>
    public void Reponse(int reponse) {
        GameObject.Find("GameManager").GetComponent<GameManagerManageCards>().ReponseRemoveDeck(reponse); 
    }

    public void Supprimer(int currentDeckNumber) {
        transform.Find("Text").gameObject.GetComponent<Text>().text = "Voulez-vous supprimer le deck " +
            currentDeckNumber.ToString() + "?"; 
    }
}
