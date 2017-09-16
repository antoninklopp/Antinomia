using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 

public class ImageCardBattle : MonoBehaviour {
	/*
	 * Ceci est la version Objet et non UI. 
	 * 
	 */ 

	private Sprite[] AllImages; 
	public Sprite DosCarte; 

	// Use this for initialization
	void Start () {
		AllImages = Resources.LoadAll<Sprite> ("Cartes");
	}

	// Update is called once per frame
	void Update () {

	}

	public void setImage(string name){
		/*
		 * Changer l'image
		 */ 
		for (int i = 0; i < AllImages.Length; ++i) {
			if (name == AllImages [i].name) {
				// On cherche la bonne image dans la liste. 
				GetComponent<SpriteRenderer>().sprite = AllImages[i]; 
				return; 
			}
		}
        GetComponent<SpriteRenderer>().sprite = AllImages[0]; 

		throw new Exception ("La face de la carte n'a pas été trouvée" + name); 
	}

	public void setDosCarte(){
		/*
		 * Dos de la carte
		 */ 
		GetComponent<SpriteRenderer>().sprite = DosCarte;
	}
}
