
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LanguageModule;
using AntinomiaException;

namespace Antinomia.Battle {

    /// <summary>
    /// Un deuxième Canvas qui permet d'informer le joueur de certaines informations comme le fait qu'on arrive à un nouveau Tour. 
    /// </summary>
    public class InformationManager : MonoBehaviour {

        public enum TourJoueur {
            JOUEUR_LOCAL,
            JOUEUR_ADVERSE
        };

        /// <summary>
        /// UIText sur lequel on met les informations importantes à donner au joueur. 
        /// </summary>
        private GameObject TextDisplay;

        /// <summary>
        /// ParticleSystem à instancier lors d'un nouveau tour. 
        /// </summary>
        public GameObject ParticleSystemNewTurn;

        /// <summary>
        /// ParticleSystem à instancier lors d'une nouvelle phase. 
        /// </summary>
        public GameObject ParticleSystemNewPhase;

        void Start() {
            // On vérifie que le particle system soit bien présent. 
            if (ParticleSystemNewPhase == null || ParticleSystemNewTurn == null) {
                throw new UnusualBehaviourException("Il manque un compsant de particle system ici. ");
            }
            TextDisplay = GameObject.Find("InformationTextInformation");
            TextDisplay.SetActive(false);
        }

        /// <summary>
        /// Afficher la phase en cours au joueur. 
        /// </summary>
        /// <param name="phase"></param>
        public void SetInformation(Player.Phases phase) {
            if (GetComponent<TutorialManager>() != null) {
                GetComponent<TutorialManager>().SetTutorialInformation(phase);
            }
            else {
                GameObject partInstantiated = Instantiate(ParticleSystemNewPhase);
                float duration = partInstantiated.GetComponent<ParticleSystem>().main.duration;
                SetInformation(LanguageData.GetString(phase.ToString(), "tutorial"), duration);
                Destroy(partInstantiated, duration);
            }
        }

        /// <summary>
        /// Attention : Ici le tour du joueur local est 0
        /// Le tour du joueur adverse est 1. 
        /// </summary>
        /// <param name="Turn"></param>
        public void SetInformation(TourJoueur t) {
            string message = " ";
            if (t == TourJoueur.JOUEUR_LOCAL) {
                message = LanguageData.GetString("MonTour", PlayerPrefs.GetString("Language"), "tutorial");
            }
            else {
                message = LanguageData.GetString("TourAdversaire", PlayerPrefs.GetString("Language"), "tutorial");
            }
            GameObject partInstantiated = Instantiate(ParticleSystemNewTurn);
            float duration = partInstantiated.GetComponent<ParticleSystem>().main.duration;
            SetInformation(message, 3f);
            Destroy(partInstantiated, 3f);
        }

        /// <summary>
        /// Montrer une information au joueur. 
        /// </summary>
        /// <param name="message">Message à afficher</param>
        /// <param name="time">Optionnel, temps au bout duquel l'objet information se désactive. 
        /// Valeur par défaut : 2s</param>
        public void SetInformation(string message, float time = 2f) {
            TextDisplay.SetActive(true);
            TextDisplay.GetComponent<Text>().text = message;
            StartCoroutine(DisableTextDisplay(time));
        }

        public IEnumerator DisableTextDisplay(float time) {
            yield return new WaitForSeconds(time);
            TextDisplay.SetActive(false);
        }

        public GameObject GetTextDisplay() {
            return TextDisplay;
        }
    }

}
