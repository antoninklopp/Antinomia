
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ImageCarteChoixDeck : MonoBehaviour {

    private Sprite[] AllImages;

    /// <summary>
    /// Lorsque la carte est spawn, on récupère toutes les images de carte. 
    /// </summary>
    void Start() {
        AllImages = Resources.LoadAll<Sprite>("Cartes");
    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// Mettre à jour l'image de la carte. 
    /// </summary>
    /// <param name="name">shortCode de la carte</param>
    public void setImage(string name) {
        /*
		 * Changer l'image
		 */
        AllImages = Resources.LoadAll<Sprite>("Cartes");
        for (int i = 0; i < AllImages.Length; ++i) {
            if (name == AllImages[i].name) {
                // On cherche la bonne image dans la liste. 
                GetComponent<Image>().sprite = AllImages[i];
                return;
            }
        }

        GetComponent<Image>().sprite = AllImages[0];

        Debug.Log(name);
        Debug.LogWarning("La face de la carte n'a pas été trouvée");
        //throw new Exception ("La face de la carte n'a pas été trouvée" + name); 
    }
}
