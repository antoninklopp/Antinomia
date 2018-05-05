
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

    /// <summary>
    /// Classe de l'intelligence artificielle du jeu. 
    /// </summary>
    public class IA : Player {
        /*
         * L'intelligence artificielle sera très basique:
         * Ses actions : 
         * 
         * Choix des cartes au début : Verifier qu'il y a au mois une élémentaire dans les 6 premières cartes. 
         * Il faudra pré-build des cartes pour l'intelligence artificielle. 
         * (A stocker sur la machine pour pouvoir jouer hors connexion). 
         * 
         * Lors des phases principales : Lorsqu'il n'y a pas de cartes : essayer d'en poser une avec son aka
         * Si il n'y a pas d'aka, essayer d'en poser une avec un coût élémentaire. 
         * 
         * Lors de la phase principale : Si toutes les cartes sur le champ de bataille sont plus forte, en rapatrier une au sanctuaire
         * et inversement si les cartes adverses sont moins fortes
         * 
         * Lors de la phase de combat : Attaquer la carte qui fait que celle de l'IA ne se détruit pas. 
         * 
         * Cette IA va vraiment servir de test pour le jeu. 
         * 
         */

        /// <summary>
        /// Temps d'attente entre deux actions de l'IA
        /// </summary>
        private float refreshTime = 0.5f;

        /// <summary>
        /// On garde en mémoire l'objet GameManager
        /// </summary>
        private GameObject GameManagerObject;

        private Phases currentPhase;
        private int currentTour;

        private void Start() {
            GameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
            StartCoroutine(ActionsIA(refreshTime));
        }

        /// <summary>
        /// Equivalent d'une fonction update. 
        /// Toutes les 0.5 secondes, l'IA regarde si elle peut faire une action. 
        /// </summary>
        /// <returns></returns>
        private IEnumerator ActionsIA(float refreshTime) {
            while (true) {
                getTour();
                // Si ce n'est pas à l'IA de jouer et qu'il n'y a pas d'effets en cours, 
                // on refresh juste le système. 
                if (currentTour != PlayerID && GameObject.FindGameObjectsWithTag("Pile").Length == 0) {
                    yield return new WaitForSeconds(refreshTime);
                }
                else {
                    getPhase();
                    JouerAction();
                    yield return new WaitForSeconds(refreshTime);
                }
            }
        }

        private void getPhase() {
            currentPhase = GameManagerObject.GetComponent<GameManager>().getPhase();
        }


        private void getTour() {
            currentTour = GameManagerObject.GetComponent<GameManager>().getTour();
        }

        /// <summary>
        /// Permettre à l'IA de jouer une action
        /// </summary>
        private void JouerAction() {
            switch (currentPhase) {
                case Phases.INITIATION:
                    // On ne fait rien pendant la phase d'initiation
                    GoToNextPhase();
                    break;
                case Phases.PIOCHE:
                    // On ne fait rien pendant la phase de pioche
                    GoToNextPhase();
                    break;
                case Phases.PREPARATION:
                    JouerPhasePreparation();
                    break;
                case Phases.PRINCIPALE1:

                    break;
                case Phases.COMBAT:

                    break;
                case Phases.PRINCIPALE2:

                    break;
                case Phases.FINALE:
                    break;
            }
        }

        /// <summary>
        /// Actions de l'IA pendant la phase de préparation. 
        /// </summary>
        private void JouerPhasePreparation() {
            // S'il n'y a aucun effet en cours. 
            if (GameObject.FindGameObjectsWithTag("Pile").Length == 0) {
                // S'il n'y a aucune carte, on ne peut pas jouer. 
                if (getNombreDeCartes() == 0) {
                    return;
                }
                // L'IA met toutes les cartes du sanctuaire sur le board, sans réfléchir.
                else if (GetSanctuaireJoueur().GetComponent<Sanctuaire>().getNumberCardsSanctuaire() != 0) {
                    List<GameObject> CarteSanctuaire = GetSanctuaireJoueur().GetComponent<Sanctuaire>().getCartesSanctuaire();
                    if (CarteSanctuaire[0].GetComponent<Entite>() == null) {
                        Debug.LogWarning("Il est impossible qu'une carte dans le sanctuaire ne soit pas une entité " +
                            "ou une émanation");
                    }
                    else {
                        // On bouge la carte sur le champ de bataille. 
                        CarteSanctuaire[0].GetComponent<Entite>().MoveToChampBataille();
                        // On doit attendre de voir si le joueur réagit à cette action, avant de pouvoir
                        // potentiellement bouger la deuxième carte.
                        return;
                    }
                }
            }

        }

        /// <summary>
        /// Actions à faire lors de la phase principale 1. 
        /// </summary>
        private void JouerPhasePrincipale1() {
            // Si on n'a pas d'AKA ou de cartes, il faut en créer une grâce à une invocation élémentaire. 
            List<GameObject> CartesMain = GetMainJoueur().GetComponent<MainJoueur>().getCartesMain();
            if (PlayerAKA < 2 || getNombreDeCartes() == 0) {
                // On ne peut faire qu'une seule invocation élémentaire par tour, donc on cherche la 
                // première carte élémentaire. 
                for (int i = 0; i < CartesMain.Count; i++) {
                    if (CartesMain[i].GetComponent<CarteType>().thisCarteType == CarteType.Type.ENTITE) {
                        if (CartesMain[i].GetComponent<Entite>().EntiteNature == Entite.Nature.ELEMENTAIRE) {
                            // Si la carte est une carte élémentaire. 
                            CartesMain[i].GetComponent<Entite>().MoveToSanctuaire();
                            // On ne peut invoquer qu'une seule carte. 
                            // De cette façon. 
                            return;
                        }
                    }
                }
            }
            else {
                // Si on a plus de deux d'AKA et qu'il y a déjà des cartes sur le terrain.
                for (int i = 0; i < CartesMain.Count; i++) {
                    // On essaye d'instancier un maximum de cartes
                    if (possibleInvoquerCarteAKA(CartesMain[i])) {
                        CartesMain[i].GetComponent<Entite>().MoveToChampBataille();
                        return;
                    }
                }
            }

            // Si on ne peut faire aucune action, on va à la prochaine phase. 
            GoToNextPhase();
        }

        /// <summary>
        /// Actions à faire pendant la phase de combat. 
        /// </summary>
        private void JouerPhaseCombat() {
            List<GameObject> CartesChampBataille = GetChampBatailleJoueur().GetComponent<CartesBoard>().getCartesChampBataille();
            for (int i = 0; i < CartesChampBataille.Count; i++) {
                GameObject Carte = CartesChampBataille[i];
                if (Carte.GetComponent<CarteType>().thisCarteType == CarteType.Type.ASSISTANCE) {
                    // on ne fait pas attaquer une assistance. 
                    return;
                }
                else {
                    if (Carte.GetComponent<Entite>().hasAttacked != 1) {
                        continue;
                    }
                    // Si c'est une émanation ou une entité
                    List<GameObject> CartesChampBatailleAdversaire = FindNotLocalPlayer().GetComponent<Player>().GetChampBatailleJoueur().
                        GetComponent<CartesBoard>().getCartesChampBataille();
                    for (int j = 0; j < CartesChampBatailleAdversaire.Count; i++) {
                        if (Carte.GetComponent<Entite>().getPuissance() > CartesChampBatailleAdversaire[i].GetComponent<Entite>().getPuissance()) {
                            GameManagerObject.GetComponent<GameManager>().AttackMyPlayer(Carte);
                            GameManagerObject.GetComponent<GameManager>().AttackOtherPlayer(CartesChampBatailleAdversaire[i]);
                            GameManagerObject.GetComponent<GameManager>().Attack();
                            return;
                        }
                    }
                }
            }

            // Si on ne peut faire aucune action, on va à la prochaine phase. 
            GoToNextPhase();
        }

        private void JouerPhasePrincipale2() {
            // Pour l'instant on ne fait rien pendant la phase principale 2.
            GoToNextPhase();
        }

        private void JouerPhaseFinale() {
            // Rien à jouer ici, juste à passer à la phase suivante. 
            GoToNextPhase();
        }

        /// <summary>
        /// Permettre à l'IA de passer à la prochaine phase.
        /// </summary>
        private void GoToNextPhase() {
            GameManagerObject.GetComponent<GameManager>().GoToNextPhase();
        }

        /// <summary>
        /// Regarder si on peut invoquer une carte grâce à de l'AKA. 
        /// </summary>
        /// <param name="Carte">Carte qu'on veut tenter d'invoquer.</param>
        /// <returns>true si on peut invoquer la carte, false sinon</returns>
        private bool possibleInvoquerCarteAKA(GameObject Carte) {
            if (Carte.GetComponent<CarteType>().thisCarteType == CarteType.Type.ENTITE) {
                if (Carte.GetComponent<Entite>().CoutAKA <= PlayerAKA) {
                    return true;
                }
            }
            return false;
        }

    }

}
