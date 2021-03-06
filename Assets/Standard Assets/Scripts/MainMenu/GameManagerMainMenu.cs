﻿
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using Antinomia.GameSparksScripts;
#if UNITY_EDITOR
using UnityEditor; 
using UnityEditor.Callbacks;
#endif

namespace Antinomia.MainMenu {

    /// <summary>
    /// GameManager du menu du jeu. 
    /// </summary>
    public class GameManagerMainMenu : MonoBehaviour {

        /// <summary>
        /// Offrir la possibilité d'un choix de Deck. 
        /// </summary>
        public bool choixDeck = false;
        GameObject BuildVersion;

        GameObject IA;

        GameObject Test;

        // Use this for initialization
        void Start() {
            if (GameObject.Find("ObjectIntro") != null) {
                Destroy(GameObject.Find("ObjectIntro"));
            }
            BuildVersion = GameObject.Find("BuildVersion");
            IA = GameObject.Find("IAvIA");
#if UNITY_EDITOR
            BuildVersion.GetComponent<Text>().text = "Debug Version : device" + "\nBuild version : " + Application.version;
#else
        BuildVersion.GetComponent<Text> ().text = "Connected as : " + 
            PlayerPrefs.GetString("user") + "\nBuild version : " + Application.version; 
        IA.SetActive(false);
        Test.SetActive(false); 
#endif
        }

        /// <summary>
        /// Aller au management des cartes. 
        /// </summary>
        public void GoToManageCards() {
            /*
             * Changement de scene pour le management des cartes
             */

            SceneManager.LoadScene("ManageCards");
        }

        /// <summary>
        /// Aller au lobby en passant par le matchmaking auto 
        /// </summary>
        public void GoToLobby() {
            /*
             * Changement de scène pour aller au lobby
             */
            Debug.Log("salut");
            if (choixDeck) {
                SceneManager.LoadScene("ChoixDeck");
            }
            else {
                PlayerPrefs.SetInt("ChoixDeck", 2);
                SceneManager.LoadScene("SimpleLobbyMatchmakingAuto");
            }
        }

        /// <summary>
        /// Aller au lobby sans passer par le matchmaking auto
        /// </summary>
        public void GoToLobbyCustom() {
            SceneManager.LoadScene("ChoixDeck");

        }

        /// <summary>
        /// Aller au Shop. 
        /// </summary>
        public void GoToShop() {
            /*
             * Changement de scène pour le magasin!
             */
            SceneManager.LoadScene("Shop");
        }

        /// <summary>
        /// Se déconnecter et revenir à l'écran de connexion. 
        /// </summary>
        public void Disconnect() {
            /*
             * Deconnexion du joueur. 
             */
            SaveLoginsOnDevice save = new SaveLoginsOnDevice();
            save.DisconnectPlayer();
            Application.Quit();
        }

        public void SubmitComment() {
            SceneManager.LoadScene("CommentBeta");
        }

        public void SceneIAvIA() {
            SceneManager.LoadScene("IAvIA");
        }

        public void SceneTest() {
            SceneManager.LoadScene("TestCartes");
        }

        public void SceneSettings() {
            SceneManager.LoadScene("Settings");
        }
    }

}
