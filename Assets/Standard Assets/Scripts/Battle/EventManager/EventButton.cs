
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antinomia.Battle {

    public class EventButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        /// <summary>
        /// ID de la carte associée au bouton
        /// </summary>
        public int IDCarte;

        /// <summary>
        /// Lorsque le pointeur entre sur le bouton. 
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData) {
            NetworkBehaviourAntinomia.FindCardWithID(IDCarte).GetComponent<Carte>().HighlightSelectionEvent();
        }

        /// <summary>
        /// Lorsque le pointeur sort du bouton. 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData) {
            NetworkBehaviourAntinomia.FindCardWithID(IDCarte).GetComponent<Carte>().StopHighlightSelectionEvent();
        }
    }

}
