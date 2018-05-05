
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

    /// <summary>
    /// On ajoute ce script à une entité, lorsque une assistance est liée à cette entité. 
    /// On le détruit lorsque l'assistance est déliée. 
    /// </summary>
    public class EntiteAssocieeAssistance : MonoBehaviour {

        public GameObject AssistanceAssociee;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void OnMouseEnter() {
            //AssistanceAssociee.GetComponent<Assistance>().CreateBigCard();
            //AssistanceAssociee.GetComponent<Assistance>().setBigCardPosition(
            //    new Vector2(transform.position.x + 3, transform.position.y)); 
        }

        private void OnMouseExit() {
            AssistanceAssociee.GetComponent<Assistance>().DestroyBigCard();
        }

        public void EntiteDetruite() {
            /*
             * Lorsque l'entité associée à une assistance est détruite, on l'indique à l'entité. 
             */
            AssistanceAssociee.SendMessage("EntiteDetruite");

        }
    }

}
