
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Antinomia.ChoixGame {

    public class ChoixGameManager : MonoBehaviour {
        /*
         * Cette scène permet de choisir le type de game choisie par le joueur
         * 
         * Il peut être viewer : C'est à dire regarder une game
         * Il peut choisir de faire un matchmaking automatique
         * Il peut choisir de faire un matchmaking non automatique, c'est-à-dire créer son serveur
         * et "attendre" un autre joueur.
         * 
         * 
         */

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void OnClickViewer() {
            PlayerPrefs.SetString("isViewer", "true");
        }

        public void OnClickAutoMatchmaking() {


        }

        public void OnClickManualMatchmaking() {
            SceneManager.LoadScene("SimpleLobby");
        }
    }

}
