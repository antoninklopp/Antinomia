
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

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
                List<GameObject> AllCartesBan = new List<GameObject>();
                Transform CartesCimetieresTransform = transform.parent.Find("CartesBan");
                for (int i = 0; i < CartesCimetieresTransform.childCount; ++i) {
                    AllCartesBan.Add(CartesCimetieresTransform.GetChild(i).gameObject);
                }
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ShowBan(AllCartesBan,
                    transform.parent.Find("CartesBan").GetComponent<Ban>().NombreCartesBanniesFaceCachee);
            }
            else {
                GameObject.FindGameObjectWithTag("GameManager").SendMessage("HideBan");
            }
            cardsCurrentlyShown = !cardsCurrentlyShown;
        }
    }

}
