
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antinomia.Battle {

    public class CardInfoShowCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        /// <summary>
        /// String associé à la carte
        /// </summary>
        public string InfoCarte;

        /// <summary>
        /// ShortCode de la carte. 
        /// </summary>
        public string shortCode;

        /// <summary>
        /// Mettre à jour les infos à montrer. 
        /// </summary>
        /// <param name="shortCode"></param>
        /// <param name="InfoCarte"></param>
        public void SetInfo(string shortCode, string InfoCarte) {
            this.InfoCarte = InfoCarte;
            this.shortCode = shortCode;
        }

        /// <summary>
        /// Lorsque le pointeur entre sur le bouton. 
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ShowCarteInfo(shortCode, InfoCarte);
        }

        /// <summary>
        /// Lorsque le pointeur sort du bouton. 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().HideInfoCarte();
        }
    }

}