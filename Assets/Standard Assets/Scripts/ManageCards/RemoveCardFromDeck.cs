
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER

using Antinomia.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Antinomia.ManageCards {

    /// <summary>
    /// Enlever une carte du deck.
    /// </summary>
    public class RemoveCardFromDeck : MonoBehaviour, IDropHandler {
        /*
         * Enlever une carte du deck. 
         * 
         */

        GameObject CardRemovedText;
        GameObject GameManagerObject;

        // Use this for initialization
        void Start() {
            CardRemovedText = GameObject.Find("Info");
            GameManagerObject = GameObject.Find("GameManager");
        }

        // Update is called once per frame
        void Update() {

        }

        #region IDropHandler implementation
        public void OnDrop(PointerEventData eventData) {

            //AllCards.Add (eventData.pointerDrag.gameObject);

            if (eventData.pointerDrag.GetComponent<DragImage>().inDeck) {
                eventData.pointerDrag.transform.SetParent(transform);
                EnleverCarte(eventData.pointerDrag);
            }

            //ReorganiseCardsInScrollView (transform.GetChild(0).GetChild(0).gameObject, AllCards, 0f); 
        }
        #endregion

        void EnleverCarte(GameObject objectAdded) {
            string cardName = GetName(objectAdded);
            int deckNumber = GameObject.Find("GameManager").GetComponent<GameManagerManageCards>().currentDeckNumber;
            if (CardRemovedText != null) {
                CardRemovedText.GetComponent<Text>().text = "Carte" + cardName + " enlevée du deck " + deckNumber.ToString();
            }

            GameManagerObject.GetComponent<GameManagerManageCards>().EnleverCarte(objectAdded, deckNumber);
            GameManagerObject.SendMessage("ReorganiseDeckCards", deckNumber);
        }

        string GetName(GameObject objectAdded) {
            string name = " ";
            if (objectAdded.GetComponent<Entite>() != null) {
                name = objectAdded.GetComponent<Entite>().Name;
            }
            else if (objectAdded.GetComponent<Sort>() != null) {
                name = objectAdded.GetComponent<Sort>().Name;
            }
            return name;
        }

    }

}
