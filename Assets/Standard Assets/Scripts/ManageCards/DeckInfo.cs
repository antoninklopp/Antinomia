
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class DeckInfo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Recuperer le nombre 
    /// </summary>
    /// <param name="deck"></param>
    public void setDeckInfo(Deck deck) {
        Dictionary<Entite.Element, int> elementaires = deck.getElementaires();

        string deckEstComplet = ""; 
        if (deck.deckComplet()) {
            deckEstComplet = "Le deck est complet";
        } else {
            deckEstComplet = "Le deck est incomplet"; 
        }

        // Recuperation de l'objet Texte
        transform.GetChild(0).gameObject.GetComponent<Text>().text =
            "INFORMATIONS SUR LE DECK \n\n" +
            "Nombre de cartes " + deck.getNombreCartes().ToString() + "\n" +
            deckEstComplet + "\n\n" +
            "élémentaires FEU " + elementaires[Entite.Element.FEU].ToString() + "\n" +
            "élémentaires AIR " + elementaires[Entite.Element.AIR].ToString() + "\n" +
            "élémentaires EAU " + elementaires[Entite.Element.EAU].ToString() + "\n" +
            "élémentaires TERRE " + elementaires[Entite.Element.TERRE].ToString() + "\n" +
            "Nombre d'entités " + deck.getNombreEntites().ToString() + "\n" +
            "Nombre de sorts " + deck.getNombreSorts().ToString() + "\n" +
            "Nombre d'assistances " + deck.getNombreAssistances().ToString() + "\n\n" +
            "Cout moyen en AKA " + deck.AKAMoyen() + "\n"; 
        
        StartCoroutine(DisableInfo());
    }

    /// <summary>
    /// Desactiver les infos sur le deck. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableInfo() {
        yield return new WaitForSeconds(4f);
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
    }
}
