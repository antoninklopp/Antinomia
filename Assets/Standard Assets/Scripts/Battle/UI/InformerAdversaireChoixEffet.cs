
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

    public class InformerAdversaireChoixEffet : MonoBehaviour {

        public GameObject InformerAdversaireChoisitEffet;

        private GameObject CurrentChoixAdversaire;

        // Use this for initialization
        void Start() {

        }

        /// <summary>
        /// Informer le joueur courant que son adversaire choisit des effets. 
        /// </summary>
        public void AdversaireChoisitEffet() {
            if (CurrentChoixAdversaire == null) {
                CurrentChoixAdversaire = Instantiate(InformerAdversaireChoisitEffet);
                // L'objet est un objet d'UI.
                CurrentChoixAdversaire.transform.SetParent(GameObject.Find("GameManager").transform, false);
                CurrentChoixAdversaire.SetActive(true);
            }
        }

        /// <summary>
        /// Detruire l'objet d'information à l'adversaire. 
        /// </summary>
        public void DetruireAdversaireChoisitEffet() {
            Destroy(CurrentChoixAdversaire);
            CurrentChoixAdversaire = null;
        }

    }

}
