using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Le multijoueur!
using UnityEngine.Networking; 

public class MainJoueur : NetworkBehaviour {
	/*
	 * Classe de la main du joueur. 
	 * Beaucoup d'informations vont être stockées ici et de calculs faits.
	 * 
	 * AKA rémanent, phase de pioche. 
	 * Tout sera fait depuis la main du joueur.
	 * 
	 */ 
	List<GameObject> CartesMains = new List<GameObject>(); 

	public GameObject CartePrefab; 

	// Variables de l'AKA rémanant et de l'AKA courant. 
	public int AKARemanent; 
	public int AKACourant; 

	private float OffsetCarteChampBataille = 0.2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnStartClient()
	{
		ClientScene.RegisterPrefab(CartePrefab);
	}
		
	void ReordonnerCarte(){
		/*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */ 
		CartesMains = new List<GameObject> (); 
		foreach (Transform child in transform) {
			CartesMains.Add (child.gameObject); 
		}
		if (CartesMains.Count == 1) {
			// Si la carte ajoutée est la première carte, on la met au centre du board. 
			CartesMains [0].transform.localPosition = new Vector3 (0, 0, 0);
		} else {
			for (int i = 0; i < CartesMains.Count; i++) {
				// Sinon on les décale toutes vers la gauche et on insère à droite. 
				//CartesMains [i].transform.localPosition = 
				//	new Vector3 (CartesMains [i].transform.localPosition.x - Carte.GetComponent<BoxCollider2D>().size.x/2 * Carte.transform.localScale.x, 0, 0); 
				CartesMains [i].transform.localPosition = 
					new Vector3 ((-(int)CartesMains.Count/2 + i)*CartePrefab.GetComponent<BoxCollider2D>().size.x*CartePrefab.transform.localScale.x - OffsetCarteChampBataille , 0, 0); 
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

	void ReordonnerAutreCarte(){
		/*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */ 
		CartesMains = new List<GameObject> (); 
		foreach (Transform child in transform) {
			CartesMains.Add (child.gameObject); 
		}
		if (CartesMains.Count == 1) {
			// Si la carte ajoutée est la première carte, on la met au centre du board. 
			CartesMains [0].transform.localPosition = new Vector3 (0, 0, 0);
		} else {
			for (int i = 0; i < CartesMains.Count; i++) {
				// Sinon on les décale toutes vers la gauche et on insère à droite. 
				//CartesMains [i].transform.localPosition = 
				//	new Vector3 (CartesMains [i].transform.localPosition.x - Carte.GetComponent<BoxCollider2D>().size.x/2 * Carte.transform.localScale.x, 0, 0); 
				CartesMains [i].transform.localPosition = 
					new Vector3 ((-(int)CartesMains.Count/2 + i)*CartePrefab.GetComponent<BoxCollider2D>().size.x*CartePrefab.transform.localScale.x - OffsetCarteChampBataille , 0, 0);
			}
			// On insère la dernière carte à droite. 
			//CartesMains[CartesMains.Count - 1].transform.localPosition = 
			//	new Vector3(CartesMains[CartesMains.Count - 2].transform.localPosition.x + Carte.GetComponent<BoxCollider2D>().size.x * Carte.transform.localScale.x, 0, 0);
		}
	}

	void DeleteCard(GameObject Object){
		/*
		 * Supprimer une carte de la liste des cartes de la main. 
		 */ 
		CartesMains.Remove (Object); 
		Debug.Log (CartesMains.Count); 
	}

//	void ReordonnerCarteOnline(){
//		/*
//		 * Après un mouvement de carte, on update sa position sur le serveur. 
//		 * Il faut que ce soit le joueur qui appelle la commande. 
//		 */ 
//		CmdReordonnerCarte (); 
//		transform.parent.parent.gameObject.SendMessage("ReordonnerCarteOnline")
//	}
//
//	[Command]
//	void CmdReordonnerCarte(){
//		ReordonnerCarte (); 
//		print("ReordonnerCarte!"); 
//	}
}
