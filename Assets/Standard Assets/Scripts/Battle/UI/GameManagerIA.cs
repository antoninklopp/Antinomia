
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

    /// <summary>
    /// GameManager du "combat" entre IA
    /// </summary>
    public class GameManagerIA : GameManager {

        GameObject Player1Attack;
        GameObject Player2Attack;

        public override void Start() {
            NomJoueur1 = GameObject.Find("NomJoueur1");
            NomJoueur2 = GameObject.Find("NomJoueur2");
            //SetNames (); 
            NextPhase = GameObject.Find("ButtonPhase");
            CurrentPhase = GameObject.Find("Phase");
            CurrentTour = GameObject.Find("Tour");
            Capacite_Effet = GameObject.Find("Capacite_Effet");
            setNamePhaseUI(Phase);
            setTour(Tour);
            ChooseCardsObject = GameObject.Find("ChooseCards");
            CartesCimetiere = transform.Find("CartesCimetiere").gameObject;
            CarteBaseCimetiere = CartesCimetiere.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            CarteBaseCimetiere.SetActive(false);
            CartesCimetiere.SetActive(false);
            EffetParticuleTerrain = GameObject.Find("EffetParticuleTerrain");
            EffetParticuleTerrain.SetActive(false);
            InfoCarteBattle = GameObject.Find("InfoCarte");
            InfoCarteBattle.SetActive(false);
            InfoCarteBattlePhone = GameObject.Find("InfoCartePhone");
            InfoCarteBattlePhone.SetActive(false);
            DisplayInfo = GameObject.Find("DisplayInfo");
            PauseButton = GameObject.Find("PauseButton");
            DisplayInfo.SetActive(false);
            PauseButton.SetActive(false);
            ChoixCartesDebut = GameObject.Find("ChoixCartesDebut");
            CarteDebutPrefab = ChoixCartesDebut.transform.Find("ChoixCartesDebutFond").Find("CarteDebut").gameObject;
            ChoixCartesDebut.SetActive(false);
            ProposerEffet = GameObject.Find("ProposerEffet");
            ProposerEffet.SetActive(false);
            Console = GameObject.Find("Console");
            ProposerDefairePile = GameObject.Find("ProposerDefairePile");
            ProposerDefairePile.SetActive(false);
            // StartCoroutine(PiocheDebut (6)); 
            SliderPause = GameObject.Find("SliderPause");
            SliderPause.SetActive(false);

            StartCoroutine(PiocheDebut(6, 1));
            StartCoroutine(PiocheDebut(6, 2));
        }

        public IEnumerator PiocheDebut(int nombreCartes, int PlayerID) {
            /*
             * Pioche au début de la partie. 
             */
            yield return new WaitForSeconds(0.35f);
            yield return FindPlayerWithID(PlayerID).GetComponent<Player>().CreateDeck();
            for (int i = 0; i < nombreCartes; ++i) {
                // Debug.Log("JE PIOCHE AU DEBUT");
                // Debug.Log("LA CARTE " + i.ToString());
                yield return FindPlayerWithID(PlayerID).GetComponent<Player>().PiocherCarteRoutine();
                yield return new WaitForSeconds(0.5f);
            }
        }

    }

}
