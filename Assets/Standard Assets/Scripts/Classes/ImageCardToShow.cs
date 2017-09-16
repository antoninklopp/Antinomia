using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 

public class ImageCardToShow : MonoBehaviour {
	/*
	 * Composant attaché à l'image à montrer lorsque le joueur doit choisir des cartes à montrer à l'autre joueur par exemple. 
	 */

	private Sprite[] AllImages; 
	public GameObject Croix; 
	private int intCarte = 0; 
	private bool isTargeted = false; 
	private GameObject CroixCreated; 

	// Use this for initialization
	void Start () {
		AllImages = Resources.LoadAll<Sprite> ("Cartes"); 
	}

	public void setImage(string name){
		/*
		 * Changer l'image
		 */ 
		AllImages = Resources.LoadAll<Sprite> ("Cartes"); 
		for (int i = 0; i < AllImages.Length; ++i) {
			if (name == AllImages [i].name) {
				// On cherche la bonne image dans la liste. 
				GetComponent<Image>().sprite = AllImages[i]; 
				return; 
			}
		}
		throw new Exception ("La face de la carte n'a pas été trouvée" + name); 
	}

	public void setOnIntListener(int _intCarte){

		intCarte = _intCarte; 
	}

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

	public void OnMouseDown(){
		// Clique sur la carte. 
		Debug.Log("MERDEEEEE"); 
		CreateTarget();
		SendCarteToManager (); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
