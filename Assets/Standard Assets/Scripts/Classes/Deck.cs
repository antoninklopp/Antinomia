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

    /// <summary>
    /// Cartes du deck
    /// </summary>
	public List<GameObject> Cartes = new List<GameObject>();

    /// <summary>
    /// Numero du deck. 
    /// </summary>
    private int number;

    public int Number {
        get {
            return number;
        }

        set {
            number = value;
        }
    }

    public Deck(List<GameObject> newCartes, int thisNumber) : this(){
		Cartes = newCartes;
		Number = thisNumber; 
	}

	public Deck(){
	
	}

    /// <summary>
    /// Ajouter une carte au deck 
    /// </summary>
    /// <param name="Carte"></param>
	public void AjouterCarte(GameObject Carte){
		Cartes.Add (Carte); 
	}

    /// <summary>
    /// Enlever une carte du deck
    /// </summary>
    /// <param name="Carte"></param>
	public void EnleverCarte(GameObject Carte){
		Cartes.Remove (Carte); 
	}

    /// <summary>
    /// Recuperer le nombre de cartes
    /// </summary>
    /// <returns></returns>
    public int getNombreCartes() {
        return Cartes.Count; 
    }

    /// <summary>
    /// Recupere le nombre d'entités dans un deck
    /// </summary>
    /// <returns></returns>
    public int getNombreEntites() {
        int nombreEntites = 0; 
        foreach (GameObject g in Cartes) {
            if (g.GetComponent<Carte>().GetType() == typeof(Entite)) {
                nombreEntites++; 
            }
        }
        return nombreEntites;  
    }

    /// <summary>
    /// Recuperer le nombre d'elementaires dans un deck. 
    /// </summary>
    /// <returns></returns>
    public int getNombreAssistances() {
        int nombreAssistances = 0; 
        foreach(GameObject g in Cartes) {
            if (g.GetComponent<Carte>().GetType() == typeof(Assistance)) {
                nombreAssistances++; 
            }
        }
        return nombreAssistances; 
    }

    public int getNombreSorts() {
        int nombreSorts = 0; 
        foreach (GameObject g in Cartes) {
            if (g.GetComponent<Carte>().GetType() == typeof(Sort)) {
                nombreSorts++; 
            }
        }
        return nombreSorts; 
    }

    /// <summary>
    /// Recuperer le nombre d'élémentaires. 
    /// </summary>
    /// <returns></returns>
    public Dictionary<Entite.Element, int> getElementaires() {
        Dictionary<Entite.Element, int> elementaires = new Dictionary<Entite.Element, int>();
        elementaires.Add(Entite.Element.AIR, 0);
        elementaires.Add(Entite.Element.EAU, 0);
        elementaires.Add(Entite.Element.TERRE, 0);
        elementaires.Add(Entite.Element.FEU, 0);
        foreach (GameObject g in Cartes) {
            if (g.GetComponent<Carte>().GetType() == typeof(Entite)) {
                if (g.GetComponent<Entite>().EntiteElement != Entite.Element.NONE) {
                    elementaires[g.GetComponent<Entite>().EntiteElement] ++;
                } 
            }
        }
        return elementaires; 
    } 

    /// <summary>
    /// Return true si le deck est complet selon le critères du nombre de cartes : 
    /// 
    /// Actuellement
    /// Nombre de cartes entre 40 et 70
    /// </summary>
    /// <returns></returns>
    public bool deckComplet() {
        if (Cartes.Count >= 40 && Cartes.Count <= 70) {
            return true;
        }
        return false; 
    }

    /// <summary>
    /// 
    /// Return null si le deck a les critères pour être "recommandé", 
    /// un certain nombre d'élémentaires : ce genre de choses. 
    /// 
    /// Sinon, retourne un string avec ce qui est recommandé
    /// </summary>
    /// <returns></returns>
    public string deckRecommande() {
        return null; 
    }
}
