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

    /// <summary>
    /// Nom du deck. 
    /// </summary>
    private string nom; 

    public int Number {
        get {
            return number;
        }

        set {
            number = value;
        }
    }

    public string Nom {
        get {
            return nom;
        }

        set {
            nom = value;
        }
    }

    public Deck(List<GameObject> newCartes, int thisNumber, string nom) : this(newCartes, thisNumber) {
        Nom = nom; 
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

    public void ResetDeck() {
        Cartes = new List<GameObject>(); 
    }

    /// <summary>
    /// Trier les cartes d'un deck selon certains critères. 
    /// </summary>
    public List<GameObject> TrierDeckCritere(string critere) {
        switch (critere) {
            case "AKA":
                return TrierParAKA();
            case "Puissance":
            case "puissance":
                return TrierParPuissance();
            case "nom":
            case "Name":
                return TrierParNom(); 

        }
        return null; 
    }

    /// <summary>
    /// On trie les cartes par AKA. 
    /// Comme seules les cartes ont cette attribut, on met les sorts et les assistances à la fin.
    /// </summary>
    /// <returns></returns>
    public List<GameObject> TrierParAKA() {
        for (int i = 0; i < Cartes.Count; i++) {
            for (int j = 0; j < i; j++) {
                if ((Cartes[j].GetType() == typeof(Sort) || Cartes[j].GetType() == typeof(Assistance)) 
                    && Cartes[i].GetType() == typeof(Entite)) {
                    GameObject Carte = Cartes[i];
                    Cartes[i] = Cartes[j];
                    Cartes[j] = Carte; 
                } else if ((Cartes[j].GetType() == typeof(Entite) && Cartes[i].GetType() == typeof(Entite)) && 
                    Cartes[j].GetComponent<Entite>().CoutAKA > Cartes[i].GetComponent<Entite>().CoutAKA) {
                    GameObject Carte = Cartes[i];
                    Cartes[i] = Cartes[j];
                    Cartes[j] = Carte;
                }
            }
        }
        return Cartes;
    }

    public List<GameObject> TrierParPuissance() {
        Debug.LogError("A implementer"); 
        return Cartes; 
    }

    public List<GameObject> TrierParNom() {
        Debug.LogError("A implementer"); 
        return Cartes; 
    }
 }
