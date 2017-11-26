using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 

/// <summary>
/// Version objet de l'image de la carte.
/// </summary>
public class ImageCardBattle : MonoBehaviour {
	/*
	 * Ceci est la version Objet et non UI. 
	 * 
	 */ 

	private Sprite[] AllImages; 
	public Sprite DosCarte; 

	/// <summary>
    /// Lorsque la carte est spawn, on récupère toutes les images de carte. 
    /// </summary>
	void Start () {
		AllImages = Resources.LoadAll<Sprite> ("Cartes");
	}

	// Update is called once per frame
	void Update () {

	}

    /// <summary>
    /// Mettre à jour l'image de la carte. 
    /// </summary>
    /// <param name="name">shortCode de la carte</param>
	public void setImage(string name){
		/*
		 * Changer l'image
		 */ 
         AllImages = Resources.LoadAll<Sprite>("Cartes"); 
        for (int i = 0; i < AllImages.Length; ++i) {
			if (name == AllImages [i].name) {
				// On cherche la bonne image dans la liste. 
				GetComponent<SpriteRenderer>().sprite = AllImages[i]; 
				return; 
			}
		}

        GetComponent<SpriteRenderer>().sprite = AllImages[0];

        Debug.Log(name); 
        Debug.LogWarning("La face de la carte n'a pas été trouvée"); 
		//throw new Exception ("La face de la carte n'a pas été trouvée" + name); 
	}

    /// <summary>
    /// Si la carte est une carte adverse, l'image de la carte est le dos de carte. 
    /// </summary>
	public void setDosCarte(){
		/*
		 * Dos de la carte
		 */ 
		GetComponent<SpriteRenderer>().sprite = DosCarte;
	}
}
