﻿
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
// using UnityEngine.TestTools;

namespace Antinomia.Battle {

    /// <summary>
    /// Gestion des cartes sur le Champ de Bataille. 
    /// 
    /// Il peut y avoir au maximum 5 cartes sur le board
    /// </summary>
    public class CartesBoard : NetworkBehaviourAntinomia {

        /*
         * Il peut y avoir au maximum 5 cartes sur le board. 
         * 
         * Abus de langage : Ici BOARD = CHAMP DE BATAILLE. 
         */

        List<GameObject> AllCreaturesChampBataille = new List<GameObject>();

        public GameObject CartePrefab;

        private float OffsetCarteChampBataille = 0.2f;

        // Savoir quelle carte est en train d'être touchée. 
        public bool carteCurrentlyClicked;

        /// <summary>
        /// Poser une nouvelle carte sur le champ de bataille. 
        /// </summary>
        public void PoserCarteChampBataille() {//Carte CarteAPoser){
            GameObject NouvelleCarte = Instantiate(CartePrefab);
            NouvelleCarte.transform.SetParent(transform);
            //AllCreaturesChampBataille.Add (NouvelleCarte); 
            CmdReordonnerCarte();
        }

        /// <summary>
        /// Reordonner les cartes sur le champ de bataille. 
        /// </summary>
        public void CmdReordonnerCarte() {
            /*
             * Réordonner les cartes, pour l'instant sans animation
             * TODO: Rajouter une animation.
             */
            // Debug.Log ("Reordonner ChampBataille");

            AllCreaturesChampBataille = new List<GameObject>();
            foreach (Transform child in transform) {
                AllCreaturesChampBataille.Add(child.gameObject);
            }
            if (AllCreaturesChampBataille.Count == 1) {
                // Si la carte ajoutée est la première carte, on la met au centre du board. 
                AllCreaturesChampBataille[0].transform.localPosition = new Vector3(0, 0, 0);
            }
            else {
                for (int i = 0; i < AllCreaturesChampBataille.Count; i++) {
                    ChangePositionCarte(AllCreaturesChampBataille[i], new Vector3(2 * (-(int)AllCreaturesChampBataille.Count / 2 + i) *
                        CartePrefab.GetComponent<BoxCollider2D>().size.x * CartePrefab.transform.localScale.x - OffsetCarteChampBataille, 0, 0));
                }
            }
        }

        void OnMouseDown() {
            // Lorsque la souris touche le board. 


        }

        [Command(channel = 0)]
        void CmdCarteDeposee(GameObject NewCard) {
            /*
             * Depot d'une carte sur le board, pour l'instant aucune vérification n'est faite. 
             * TODO: Vérifier s'il est possible de poser la carte en question sur le board. 
             * 
             */

            // On change le parent de la carte. 
            NewCard.transform.SetParent(transform);
            // Puis on réorganise l'affichage.
            AllCreaturesChampBataille.Add(NewCard);
            CmdReordonnerCarte();

            // Et on change le statut de la carte de main à board. 
            if (NewCard.GetComponent<CarteType>().thisCarteType == CarteType.Type.ENTITE) {
                // NewCard.SendMessage("setState", "BOARD");
                NewCard.SendMessage("setClicked", false);
            }
        }

        /// <summary>
        /// Récupérer le nombre de cartes sur le champ de bataille.
        /// </summary>
        /// <returns>Nombre de cartes sur le champ de bataille.</returns>
        public int GetNumberCardsChampBataille() {
            /*
             * Récupérer le nombre de cartes actuellement présentes sur le board
             */
            AllCreaturesChampBataille = new List<GameObject>();
            foreach (Transform child in transform) {
                // On ne regarde que les cartes
                if (child.gameObject.GetComponent<Carte>() != null) {
                    AllCreaturesChampBataille.Add(child.gameObject);
                }
            }

            if (AllCreaturesChampBataille.Count > 5) {
                Debug.LogWarning("Attention il y a trop de cartes sur le champ de bataille");
            }

            return AllCreaturesChampBataille.Count;
        }

        /// <summary>
        /// Changer la position d'une carte
        /// </summary>
        public static void ChangePositionCarte(GameObject Carte, Vector3 newPosition) {
            // Debug.Log("CHANGEMENT DE POSITION DE LA CARTE");
            // TODO A rectifier. 
            if (Carte.GetComponent<Carte>() != null) {
                Carte.GetComponent<Carte>().RepositionnerCarte(1, newPosition);
            }
            else {
                Carte.transform.position = newPosition;
            }
        }

        /// <summary>
        /// Récupérer toutes les cartes sur le champ de bataille. 
        /// </summary>
        /// <returns></returns>
        public List<GameObject> getCartesChampBataille() {
            AllCreaturesChampBataille = new List<GameObject>();
            foreach (Transform child in transform) {
                AllCreaturesChampBataille.Add(child.gameObject);
            }

            return AllCreaturesChampBataille;
        }


        // [UnityTest]
        private bool checkIfCartesStateChampBataille() {
            List<GameObject> CartesChampBataille = getCartesChampBataille();
            for (int i = 0; i < CartesChampBataille.Count; i++) {
                switch (CartesChampBataille[i].GetComponent<CarteType>().thisCarteType) {
                    case CarteType.Type.ENTITE:
                    case CarteType.Type.EMANATION:
                        if (CartesChampBataille[i].GetComponent<Entite>().getState() != Entite.State.CHAMPBATAILLE) {
                            return false;
                        }
                        break;
                    case CarteType.Type.ASSISTANCE:
                        if (CartesChampBataille[i].GetComponent<Assistance>().AssistanceState != Assistance.State.JOUEE
                            && CartesChampBataille[i].GetComponent<Assistance>().AssistanceState != Assistance.State.ASSOCIE_A_CARTE) {
                            return false;
                        }
                        break;
                    default:
                        throw new System.Exception("Type inconnu");
                }
            }
            return true;
        }

    }

}
