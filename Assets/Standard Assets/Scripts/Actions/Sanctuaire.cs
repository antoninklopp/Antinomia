using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Représente le sanctuaire du joueur.
/// </summary>
public class Sanctuaire : MonoBehaviour {

	/*
	 * Carte dans un sanctuaire.
	 * Attention ! Il n'y a que deux sanctuaires!
	 * 
	 */ 

	List<Creature> AllCreatures = new List<Creature>(); 
	List<GameObject> AllCreatureSanctuaire = new List<GameObject>();

	public GameObject CartePrefab; 

	private float OffsetCarteChampBataille = 0.2f;

	// Savoir quelle carte est en train d'être touchée. 
	public bool carteCurrentlyClicked; 

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	public void PoserCarteSanctuaire(){//Carte CarteAPoser){
		GameObject NouvelleCarte = Instantiate (CartePrefab); 
		NouvelleCarte.transform.SetParent (transform); 
		AllCreatureSanctuaire.Add (NouvelleCarte); 
		ReordonnerCarte ();
	}

	void ReordonnerCarte(){
		/*
		 * Réordonner les cartes, pour l'instant sans animation
		 * TODO: Rajouter une animation.
		 */ 

		AllCreatureSanctuaire = new List<GameObject> (); 
		foreach (Transform child in transform) {
			AllCreatureSanctuaire.Add (child.gameObject); 
		}

		if (AllCreatureSanctuaire.Count == 1) {
			// Si la carte ajoutée est la première carte, on la met au centre du board. 
			AllCreatureSanctuaire [0].transform.localPosition = new Vector3 (0, 0, 0);
		} else {
			for (int i = 0; i < AllCreatureSanctuaire.Count; i++) {
				// Sinon on les décale toutes vers la gauche et on insère à droite. 
				//AllCreatureSanctuaire [i].transform.localPosition = 
				//	new Vector3 (AllCreatureSanctuaire [i].transform.localPosition.x - Carte.GetComponent<BoxCollider2D>().size.x/2 - OffsetCarteChampBataille , 0, 0);
				AllCreatureSanctuaire [i].transform.localPosition = 
					new Vector3 (2*(-(int)AllCreatureSanctuaire.Count/2 + i)*CartePrefab.GetComponent<BoxCollider2D>().size.x*CartePrefab.transform.localScale.x - OffsetCarteChampBataille , 0, 0);
			}
			// On insère la dernière carte à droite. 
			//AllCreatureSanctuaire[AllCreatureSanctuaire.Count - 1].transform.localPosition = 
			//	new Vector3(AllCreatureSanctuaire[AllCreatureSanctuaire.Count - 2].transform.localPosition.x + Carte.GetComponent<BoxCollider2D>().size.x + OffsetCarteChampBataille * 2, 0, 0);
		}
	}

	void OnMouseDown(){
		// Lorsque la souris touche le board. 


	}

    /// <summary>
    /// Une carte a été déposée sur le sanctuaire. 
    /// </summary>
    /// <param name="NewCard"></param>
	void CmdCarteDeposee(GameObject NewCard){
		/*
		 * Depot d'une carte sur le board, pour l'instant aucune vérification n'est faite. 
		 * TODO: Vérifier s'il est possible de poser la carte en question sur le board. 
		 * 
		 */ 

		// On change le parent de la carte. 
		NewCard.transform.SetParent (transform);
		// Puis on réorganise l'affichage.
		AllCreatureSanctuaire.Add(NewCard); 
		ReordonnerCarte(); 

		// Et on change le statut de la carte de main à board. 
		NewCard.SendMessage("setState", "SANCTUAIRE"); 
		NewCard.SendMessage ("setClicked", false); 
	}

    /// <summary>
    /// Récypérer le nombre de cartes du sanctuaire
    /// </summary>
    /// <returns>Un itncompris entre 0 et 2</returns>
	public int getNumberCardsSanctuaire(){
		AllCreatureSanctuaire = new List<GameObject> (); 
		foreach (Transform child in transform) {
			AllCreatureSanctuaire.Add (child.gameObject); 
		}

        if (AllCreatureSanctuaire.Count > 2) {
            Debug.LogWarning("Attention, il y a trop de cartes dans le sanctuaire"); 
        }

		return AllCreatureSanctuaire.Count; 
	}

    /// <summary>
    /// Récupérer les cartes du sanctuaire
    /// </summary>
    /// <returns>La liste des cartes du sanctuaire.</returns>
    public List<GameObject> getCartesSanctuaire() {
        List<GameObject> CartesSanctuaire = new List<GameObject>(); 
        for (int i = 0; i < transform.childCount; i++) {
            CartesSanctuaire.Add(transform.GetChild(i).gameObject); 
        }
        return CartesSanctuaire; 
    }
}
