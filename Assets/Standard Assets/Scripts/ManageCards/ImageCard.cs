using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


public class ImageCard : MonoBehaviour {
	/*
	 * C'est ici qu'on associe l'image au nom de l'entité. 
	 * TODO: Il faudra tenter de changer de système car celui-ci n'est pas le meilleur.
	 * 
	 */ 

	private Sprite[] AllImages; 

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
		AllImages = Resources.LoadAll<Sprite> ("Cartes");
		for (int i = 0; i < AllImages.Length; ++i) {
			if (name == AllImages [i].name) {
				// On cherche la bonne image dans la liste. 
				GetComponent<Image>().sprite = AllImages[i];
                return; 
			}
		}
        GetComponent<Image>().sprite = AllImages[0];
    }
}
