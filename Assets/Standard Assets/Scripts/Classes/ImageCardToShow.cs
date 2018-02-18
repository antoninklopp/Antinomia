using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 

/// <summary>
/// Image de la carte sur un GameObject UI. 
/// </summary>
public class ImageCardToShow : MonoBehaviour {
	/*
	 * Composant attaché à l'image à montrer lorsque le joueur doit choisir des cartes à montrer à l'autre joueur par exemple. 
	 */

    /// <summary>
    /// Tous les sprites des Images. Recherche des sprites dans le dossier Resources.
    /// </summary>
	private Sprite[] AllImages; 

    /// <summary>
    /// Prefab de la croix à créer lors d'un choix.
    /// </summary>
	public GameObject Croix;

    private int intCarte = 0; 

    /// <summary>
    /// True si la carte est choisie lors d'un choix quelconque. 
    /// </summary>
	private bool isTargeted = false;

    /// <summary>
    /// Cible créée lors d'un choix. 
    /// </summary>
    private GameObject CroixCreated; 

	// Use this for initialization
	void Start () {
		AllImages = Resources.LoadAll<Sprite> ("Cartes"); 
	}

    /// <summary>
    /// Afficher l'image de la carte
    /// </summary>
    /// <param name="name">shortCode de la carte</param>
	public void setImage(string name){
		AllImages = Resources.LoadAll<Sprite> ("Cartes"); 
		for (int i = 0; i < AllImages.Length; ++i) {
			if (name == AllImages [i].name) {
				// On cherche la bonne image dans la liste. 
				GetComponent<Image>().sprite = AllImages[i]; 
				return; 
			}
		}
        GetComponent<Image>().sprite = AllImages[0];
        Debug.LogWarning ("La face de la carte n'a pas été trouvée" + name); 
	}

	public void setOnIntListener(int _intCarte){
		intCarte = _intCarte; 
	}

    /// <summary>
    /// Créer une croix pour pouvoir choisir la carte, pour un choix quelconque. 
    /// </summary>
	public void CreateTarget(){
		/*
		 * Pour faire voir au joueur qu'il a choisi cette carte, 
		 * on crée une croix. 
		 * Si le joueur réappuie sur la carte, on détruit la croix. 
		 * 
		 */ 
		if (!isTargeted) {
			// Si la carte n'était pas choisie
			CroixCreated = Instantiate(Croix); 
			CroixCreated.transform.SetParent (transform); 
			CroixCreated.GetComponent<RectTransform> ().localPosition = new Vector2 (0f, 0f); 
			CroixCreated.GetComponent<Image> ().raycastTarget = false; 
		} else {
			// Si la carte était choisie
			Destroy(CroixCreated); 
		}
	}

    /// <summary>
    /// Envoyer la carte au gameManager afin de pouvoir récupérer la liste des
    /// cartes choisies pour un effet. 
    /// </summary>
	public void SendCarteToManager(){
		/*
		 * Dire que la carte est choisie ou qu'elle n'est finalement plus choisie. 
		 */ 
		if (!isTargeted) {
			transform.parent.gameObject.SendMessage ("AddNewCardToReturn", intCarte); 
		} else {
			transform.parent.gameObject.SendMessage ("RemoveCardToReturn", intCarte);
		}
	}

    /// <summary>
    /// Clic sur la carte
    /// </summary>
	public void OnMouseDown(){
        // Clic sur la carte. 
		CreateTarget();
		SendCarteToManager ();
        // On selectionne ou on deselectionne la carte. 
        isTargeted = !isTargeted;
    }
}
