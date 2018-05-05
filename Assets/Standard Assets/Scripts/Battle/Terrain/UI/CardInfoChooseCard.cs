
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antinomia.Battle {

    public class CardInfoChooseCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        /// <summary>
        /// ID de la carte associée au bouton
        /// </summary>
        public int IDCarte;

        /// <summary>
        /// Lorsque le pointeur entre sur le bouton. 
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData) {
            GameObject Carte = NetworkBehaviourAntinomia.FindCardWithID(IDCarte);
            Carte.GetComponent<Carte>().HighlightSelectionEvent();
            Carte.GetComponent<Carte>().DisplayInfoCarteGameManager();
        }

        /// <summary>
        /// Lorsque le pointeur sort du bouton. 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData) {
            NetworkBehaviourAntinomia.FindCardWithID(IDCarte).GetComponent<Carte>().StopHighlightSelectionEvent();
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().HideInfoCarte();
        }
    }

}
