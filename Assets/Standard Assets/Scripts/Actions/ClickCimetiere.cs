using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickCimetiere : MonoBehaviour {
    /*
     * Lors d'un clic sur l'object qui représente le cimetiere, on ouvre le cimetiere. 
     * 
     */


    bool cardsCurrentlyShown = false;  
	// Use this for initialization
	void Start () {
		
	}

    private void OnMouseDown() {
        /*
         * Lors d'un clic sur l'objet cimetiere (objet rose). 
         */

        
        if (!cardsCurrentlyShown) {
            // On récupère les cartes dans le cimetière
            List<GameObject> allCartesCimetieres = new List<GameObject>();
            Transform CartesCimetieresTransform = transform.parent.Find("CartesCimetiere"); 
            for (int i = 0; i < CartesCimetieresTransform.childCount; ++i) {
                allCartesCimetieres.Add(CartesCimetieresTransform.GetChild(i).gameObject); 
            }
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("ShowCemetery", allCartesCimetieres);
        } else {
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("HideCemetery"); 
        }
        cardsCurrentlyShown = !cardsCurrentlyShown; 
    }
}
