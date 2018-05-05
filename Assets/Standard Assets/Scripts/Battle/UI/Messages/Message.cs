
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Antinomia.Battle {

    /// <summary>
    /// 6 possibilités de messages
    /// </summary>
    public class Message : MonoBehaviour, IPointerClickHandler {

        /// <summary>
        /// Chaque message a un code, moins lourd à envoyer par le réseau. 
        /// </summary>
        [HideInInspector]
        public int code;

        //public void OnMouseDown() {

        //}

        public void OnPointerClick(PointerEventData eventData) {
            GameObject.Find("GameManager").GetComponent<DisplayMessagePossibilities>().SendMessageToPlayer(code);
        }

        /// <summary>
        /// Aggicher le message sur le Text UI. 
        /// </summary>
        /// <param name="m"></param>
        public void SetMessage(string m) {
            GetComponent<RectTransform>().Find("Text").gameObject.GetComponent<Text>().text = m;
        }
    }

}
