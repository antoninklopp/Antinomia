
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Le multijoueur!
using UnityEngine.Networking;
// using UnityEngine.TestTools; 

/// <summary>
/// Classe de la main du joueur. 
/// </summary>
public class MainJoueur : NetworkBehaviourAntinomia {

    /// <summary>
    /// Liste des cartes présentes dans la main. 
    /// </summary>
	List<GameObject> CartesMain = new List<GameObject>(); 

    /// <summary>
    /// Le prefab de la carte de la main.
    /// </summary>
	public GameObject CartePrefab; 

	// Variables de l'AKA rémanant et de l'AKA courant. 
	public int AKARemanent; 
	public int AKACourant; 

	private float OffsetCarteChampBataille = 0.2f;

	public override void OnStartClient()
	{
		ClientScene.RegisterPrefab(CartePrefab);
	}
		
    /// <summary>
    /// Reordonner les cartes dans la main. 
    /// </summary>
	public void ReordonnerCarte(){
		/*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */ 
		CartesMain = new List<GameObject> (); 
		foreach (Transform child in transform) {
			CartesMain.Add (child.gameObject); 
		}
		if (CartesMain.Count == 1) {
			// Si la carte ajoutée est la première carte, on la met au centre du board. 
			CartesMain [0].transform.localPosition = new Vector3 (0, 0, 0);
		} else {
			for (int i = 0; i < CartesMain.Count; i++) {
				// Sinon on les décale toutes vers la gauche et on insère à droite. 
				//CartesMains [i].transform.localPosition = 
				//	new Vector3 (CartesMains [i].transform.localPosition.x - Carte.GetComponent<BoxCollider2D>().size.x/2 * Carte.transform.localScale.x, 0, 0); 
				//CartesMains [i].transform.localPosition = 
				//	new Vector3 ((-(int)CartesMains.Count/2 + i)*CartePrefab.GetComponent<BoxCollider2D>().size.x*
                //    CartePrefab.transform.localScale.x - OffsetCarteChampBataille , 0, 0);
                CartesBoard.ChangePositionCarte(CartesMain[i], new Vector3((-(int)CartesMain.Count / 2 + i) * 
                    CartePrefab.GetComponent<BoxCollider2D>().size.x * CartePrefab.transform.localScale.x - OffsetCarteChampBataille, 0, 0)); 
            }
			// On insère la dernière carte à droite. 
			//CartesMains[CartesMains.Count - 1].transform.localPosition = 
			//	new Vector3(CartesMains[CartesMains.Count - 2].transform.localPosition.x + Carte.GetComponent<BoxCollider2D>().size.x * Carte.transform.localScale.x, 0, 0);
		}

		// On demande à l'autre main de se réordonner. 
		GameObject[] Players = GameObject.FindGameObjectsWithTag("Player"); 
		for (int i = 0; i < Players.Length; i++) {
			if (Players [i] != transform.parent.gameObject) {
				Players [i].transform.Find ("MainJoueur").Find ("CartesMainJoueur").SendMessage ("ReordonnerAutreCarte"); 
				//break; 
			}
		}
	}

	public void ReordonnerAutreCarte(){
		/*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */ 
		CartesMain = new List<GameObject> (); 
		foreach (Transform child in transform) {
			CartesMain.Add (child.gameObject); 
		}
		if (CartesMain.Count == 1) {
			// Si la carte ajoutée est la première carte, on la met au centre du board. 
			CartesMain [0].transform.localPosition = new Vector3 (0, 0, 0);
		} else {
			for (int i = 0; i < CartesMain.Count; i++) {
				// Sinon on les décale toutes vers la gauche et on insère à droite. 
				//CartesMains [i].transform.localPosition = 
				//	new Vector3 (CartesMains [i].transform.localPosition.x - Carte.GetComponent<BoxCollider2D>().size.x/2 * Carte.transform.localScale.x, 0, 0); 
				CartesMain [i].transform.localPosition = 
					new Vector3 ((-(int)CartesMain.Count/2 + i)*CartePrefab.GetComponent<BoxCollider2D>().size.x*CartePrefab.transform.localScale.x - OffsetCarteChampBataille , 0, 0);
			}
			// On insère la dernière carte à droite. 
			//CartesMains[CartesMains.Count - 1].transform.localPosition = 
			//	new Vector3(CartesMains[CartesMains.Count - 2].transform.localPosition.x + Carte.GetComponent<BoxCollider2D>().size.x * Carte.transform.localScale.x, 0, 0);
		}
	}

    /// <summary>
    /// Enlever une carte de la liste des cartes de la main. 
    /// </summary>
    /// <param name="Object"></param>
	void DeleteCard(GameObject Object){
		/*
		 * Supprimer une carte de la liste des cartes de la main. 
		 */ 
		CartesMain.Remove (Object); 
		Debug.Log (CartesMain.Count); 
	}

    /// <summary>
    /// Récupérer les cartes de la main. 
    /// </summary>
    /// <returns></returns>
    public List<GameObject> getCartesMain() {
        CartesMain = new List<GameObject>();
        foreach (Transform child in transform) {
            CartesMain.Add(child.gameObject);
        }

        return CartesMain;
    }

    public int getNombreCartesMain() {
        return transform.childCount; 
    }

    // [UnityTest]
    private bool checkIfCartesStateMain() {
        List<GameObject> _CartesMain = getCartesMain();
        for (int i = 0; i < _CartesMain.Count; i++) {
            if (!_CartesMain[i].GetComponent<Carte>().isCarteInMain()) {
                return false; 
            }
        }
        return true;
    }

    public void AjouterCarteMain(GameObject Carte) {
        Carte.transform.SetParent(transform, false);
        ReordonnerCarte(); 
    }

}
