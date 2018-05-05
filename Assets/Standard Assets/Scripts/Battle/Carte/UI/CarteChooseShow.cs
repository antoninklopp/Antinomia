
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

    /// <summary>
    /// Permet d'afficher les informations sur les cartes dans les grid View
    /// Choose/Show
    /// </summary>
    public class CarteChooseShow : MonoBehaviour {

        [HideInInspector]
        public string StringToDisplay;

        [HideInInspector]
        public string shortCode;

        public CarteChooseShow() {


        }

        public CarteChooseShow(string _shortCode, string _StringToDisplay) {
            shortCode = _shortCode;
            StringToDisplay = _StringToDisplay;

        }

        /// <summary>
        /// Après création de la classe, on met les infos à jour. 
        /// </summary>
        /// <param name="_shortCode">Le shortCode de la classe</param>
        /// <param name="_StringToDisplay">Contient les informations essentielles de la classe</param>
        public void InfoCarte(string _shortCode, string _StringToDisplay) {
            shortCode = _shortCode;
            StringToDisplay = _StringToDisplay;
            GetComponent<ImageCardToShow>().setImage(shortCode);
        }

        public void OnClickCarte() {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ShowCarteInfo(shortCode, StringToDisplay);
        }
    }

}
