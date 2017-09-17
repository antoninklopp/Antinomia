using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class Condition {
    /*
     * Classe répertoriant les conditions possibles lors de l'instanciation d'un sort. 
     * 
     * Elle comprend la condition
     * et l'entier associé à la condition. 
    * 
    * Entiers compris dans l'entier lié à l'action. 
    * l'entier est sous la forme:
    * 
    * effetint = bbzaxx
    * 
    * avec z < 10 qui sera le timing de la phase. 
    * avec 1 : Phase d'Inititation
    *      2 : Phase Pioche
    *      3 : Phase de Préparation
    *      4 : Phase Principale 1
    *      5 : Phase de combat
    *      6 : Phase principale 2 
    *      7 : Phase Finale
    *      
    * avec a: le tour lors duquel ce timing s'effectue. 
    *      0 : les tours des deux joueurs
    *      1 : le tour du joueur local
    *      2 : le tour de l'autre joueur
    *      
    * avec bb : Un autre effet de timing avec 
    *      1 : Lorsque la carte est détruite. 
    *      2 : Changement de position
    *      3 : 
    *      4 : 
    *      5 : 
    *      ...
    *
    * avec xx < 99 : l'entier symbolisant le changement tel qu'un changement de puissance. 
    *   
    * LA DUREE DE L'EFFET EN NOMBRE DE TOURS EST GERE DANS ACTION. 
    *   
    * Si l'action est obligatoire, l'entier sera négatif. 
    * 
    * S'il n'y a aucune information de timing, l'effet se fait lorsque la carte est déposée sur le board. 
     */

    // Une ou plusieurs conditions peuvent être nécessaires pour effectuer un sort
    public enum ConditionEnum {
        CHOIX_ENTITE_TERRAIN, // Choix d'une entité sur le terrain
        CHOIX_ENTITE_CHAMP_BATAILLE_JOUEUR, // Choix d'une entité sur le champ de bataille du joueur
        CHOIX_ENTITE_CHAMP_BATAILLE_ADVERSAIRE, // Choix d'une entite sur le champ de bataille de l'adversaire
        CHOIX_ENTITE_CHAMP_BATAILLE, // Choix d'une entite sur un des deux champ de bataille. 
        DEFAUSSER, // Defausser une ou plusieurs cartes de sa main.  
        CARTES_CIMETIERE, // Avoir au moins x cartes dans le cimetière
        DELTA, // Avoir x cartes de moins que l'adversaire. 
        CHOIX_ELEMENT, // Choix d'un element dans le cas où on veut changer l'élément d'une carte par exemple.
        CHOIX_ENTITE_NEUTRE_ADVERSAIRE, 
        CHOIX_ENTITE_NEUTRE_JOUEUR,
        SACRIFIER_CARTE, 
        MORT, // Lors de la destruction de la carte. 
        PAYER_AKA, // Payer un cout en AKA
        NONE
    };

    public ConditionEnum ConditionCondition; 
    public int intCondition;

    public enum Reaction {
        CARTE_DETRUITE,
        NONE



    }

    public enum Tour {
        TOUR_DEUX_JOUEURS,
        TOUR_LOCAL,
        TOUR_NOT_LOCAL
    }

    public bool dependsOnPhase = false;
    public Player.Phases PhaseCondition;
    public Reaction ReactionCondition = Reaction.NONE;
    public Tour TourCondition = Tour.TOUR_DEUX_JOUEURS;
    // Cet entier correspond à xx
    public int properIntCondition = 0;
    // Cet entier correspond à y
    public int nombreDeTours = 0;
    public bool ActionObligatoire = false;

    // Lorsqu'on a le droit de faire une action une fois par tour, il faut marquer cette action comme ayant été faite. 
    public bool utilisePourCeTour = false; 

    public Condition(ConditionEnum _condition, int _intCondition) {
        ConditionCondition = _condition;
        intCondition = _intCondition;
        understandInt();
    }

    public Condition() {


    }

    public void understandInt() {
        /*
         * Cette méthode a pour but de "disséquer" l'entier pour en tirer les informations nécessaires. 
         * Pas très élégant. Faire un switch case avec des puissances de 10? 
         */

        if (intCondition < 0) {
            ActionObligatoire = true; 
        }
        int _intConditionAbs = Mathf.Abs(intCondition);

        if (_intConditionAbs < 100) {
            properIntCondition = _intConditionAbs;
        }
        else if (_intConditionAbs < Mathf.Pow(10, 4)) {
            properIntCondition = _intConditionAbs % 100;
        }
        else if (_intConditionAbs < Mathf.Pow(10, 3)) {
            properIntCondition = _intConditionAbs % 100;
            TourCondition = (Tour)(_intConditionAbs / Mathf.Pow(10, 2) - 1);
        }
        else if (_intConditionAbs < Mathf.Pow(10, 4)) {
            properIntCondition = _intConditionAbs % 100;
            if ((int)(_intConditionAbs / Mathf.Pow(10, 2)) % 10 != 0) {
                TourCondition = (Tour)((int)(_intConditionAbs / Mathf.Pow(10, 2)) - 1);
            }
            dependsOnPhase = true;
            PhaseCondition = (Player.Phases)((int)(_intConditionAbs / Mathf.Pow(10, 3)) - 1);
        }
        else {
            properIntCondition = _intConditionAbs % 100;
            if ((int)(_intConditionAbs / Mathf.Pow(10, 2)) % 10 != 0) {
                TourCondition = (Tour)((int)(_intConditionAbs / Mathf.Pow(10, 2)) - 1);
            }
            if ((int)(_intConditionAbs / Mathf.Pow(10, 3)) % 10 != 0) {
                dependsOnPhase = true;
                PhaseCondition = (Player.Phases)((int)(_intConditionAbs / Mathf.Pow(10, 3)) - 1);
            }
            ReactionCondition = (Reaction)((int)(_intConditionAbs / Mathf.Pow(10, 4)) - 1);
        }
    }



}
