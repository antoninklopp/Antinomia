using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Click sur le ban.
/// </summary>
public class ClickBan : MonoBehaviour {

    /// <summary>
    /// true si le cimetière est ouvert
    /// false si le cimetière est fermé
    /// </summary>
    bool cardsCurrentlyShown = false;

    private void OnMouseDown() {
        /*
            * Lors d'un clic sur l'objet cimetiere (objet rose). 
            */
        if (!cardsCurrentlyShown) {
            // On récupère les cartes dans le cimetière
            List<GameObject> allCartesCimetieres = new List<GameObject>();
            Transform CartesCimetieresTransform = transform.parent.Find("CartesBan");
            for (int i = 0; i < CartesCimetieresTransform.childCount; ++i) {
                allCartesCimetieres.Add(CartesCimetieresTransform.GetChild(i).gameObject);
            }
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("ShowBan", allCartesCimetieres);
        }
        else {
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("HideBan");
        }
        cardsCurrentlyShown = !cardsCurrentlyShown;
    }
}
