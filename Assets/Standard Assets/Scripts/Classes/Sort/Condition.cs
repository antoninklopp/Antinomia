using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

/// <summary>
/// Chaque effet peut-être joué s'il remplit une liste de conditions.
/// Chaque condition est représenté par cet objet. 
/// </summary>
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
    *      0 : Le tour n'a pas d'importance/pendant aucun tour. 
    *      1 : les tours des deux joueurs
    *      2 : le tour du joueur local
    *      3 : le tour de l'autre joueur
    *      
    * avec bb : Un autre effet de timing avec 
    *      1 : Lorsque la carte est détruite. 
    *      2 : Changement de position
    *      3 : 
    *      4 : 
    *      5 : 
    *      ...
    *
    * avec xx < 99 : l'entier symbolisant le changement tel que le nombre de cartes à choisir. 
    *   
    * LA DUREE DE L'EFFET EN NOMBRE DE TOURS EST GERE DANS ACTION. 
    *   
    * Si l'action est obligatoire, l'entier sera négatif. 
    * Seul l'entier de la première condition sera négatif.
    * 
    * S'il n'y a aucune information de timing, l'effet se fait lorsque la carte est déposée sur le board. 
     */

    // Une ou plusieurs conditions peuvent être nécessaires pour effectuer un sort
    public enum ConditionEnum {
        /// <summary>
        /// Choix d'une entité sur le terrain
        /// </summary>
        CHOIX_ENTITE_TERRAIN,
        /// <summary>
        /// Choix d'une entité sur le champ de bataille du joueur
        /// </summary>
        CHOIX_ENTITE_CHAMP_BATAILLE_JOUEUR,
        /// <summary>
        /// Choix d'une entite sur le champ de bataille de l'adversaire
        /// </summary>
        CHOIX_ENTITE_CHAMP_BATAILLE_ADVERSAIRE,
        /// <summary>
        /// Choix d'une entite sur un des deux champ de bataille. 
        /// </summary>
        CHOIX_ENTITE_CHAMP_BATAILLE,
        /// <summary>
        /// Defausser une ou plusieurs cartes de sa main.  
        /// </summary>
        DEFAUSSER,
        /// <summary>
        /// Avoir au moins x cartes dans le cimetière
        /// </summary>
        CARTES_CIMETIERE,
        /// <summary>
        ///  Avoir x cartes de moins que l'adversaire. 
        /// </summary>
        DELTA,
        /// <summary>
        /// Choix d'un element dans le cas où on veut changer l'élément d'une carte par exemple.
        /// </summary>
        CHOIX_ELEMENT, 
        /// <summary>
        /// Choisir une entité neutre de l'adversaire
        /// </summary>
        CHOIX_ENTITE_NEUTRE_ADVERSAIRE, 
        /// <summary>
        /// Choisir une entité neutre du joueur
        /// </summary>
        CHOIX_ENTITE_NEUTRE_JOUEUR,
        /// <summary>
        /// Sacrifier une carte
        /// </summary>
        SACRIFIER_CARTE, 
        /// <summary>
        /// Lors de la destruction de la carte
        /// </summary>
        MORT,
        /// <summary>
        /// Payer un cout en AKA
        /// </summary>
        PAYER_AKA, 
        /// <summary>
        /// Si la carte est sur le champ de bataille.
        /// </summary>
        CARTE_SUR_CHAMP_BATAILLE,
        /// <summary>
        /// Si la carte est dans le sanctuaire.
        /// </summary>
        CARTE_DANS_SANCTUAIRE, 
        /// <summary>
        /// Ici l'entier correspond au type de coût élémentaire payer
        /// int  = :
        /// 0 : n'importe quel coût élémentaire
        /// 1 : AIR
        /// 2 : EAU
        /// 3 : FEU
        /// 4 : TERRE
        /// </summary>
        PAYER_COUT_ELEMENTAIRE,
        /// <summary>
        /// Obligatoire est l'équivalent de NONE mais pour une action obligatoire, 
        /// c'est_à-dire qu'il n'y a besoin d'aucune condition mais qu'elle est obligatoire. 
        /// </summary>
        OBLIGATOIRE,
        /// <summary>
        /// Se défausser d'une carte d'un type spécial : 
        /// sera sous la forme yyxx. 
        /// Code associé : 
        /// 1 - AIR
        /// 2 - EAU
        /// 3 - FEU
        /// 4 - TERRE
        /// 5 - NEUTRE
        /// 6 - ASTRALE
        /// 7 - MALEFIQUE
        /// </summary>
        DEFAUSSER_TYPE,
        /// <summary>
        /// Le terrain doit être sous une certaine domination
        /// 1 - sans domination
        /// 2 - domination Astrale
        /// 3 - domination Malefique
        /// </summary>
        DOMINATION,
        /// <summary>
        /// Lorsque le terrain changement de domination
        /// 1 - Sans domination
        /// 2 - domination astrale
        /// 3 - Domination malefique
        /// </summary>
        CHANGEMENT_DOMINATION, 
        /// <summary>
        /// Lorsqu'une entité déclare une attaque. 
        /// </summary>
        DECLARE_ATTAQUE,
        /// <summary>
        /// Aucune condition nécessaire
        /// </summary>
        NONE
    };

    /// <summary>
    /// La condition (ou une des conditions) de l'effet. 
    /// </summary>
    public ConditionEnum ConditionCondition; 

    /// <summary>
    /// Entier transmis par la base de données. A décortiquer. 
    /// </summary>
    public int intCondition;

    public enum Reaction {
        /// <summary>
        /// Lorsqu'une carte est détruite. 
        /// </summary>
        CARTE_DETRUITE,
        /// <summary>
        /// Lors de l'arrivée d'une nouvelle carte sur le board.
        /// </summary>
        NOUVELLE_CARTE_BOARD, 
        /// <summary>
        /// Lorsque la carte est présente sur le board. L'effet est actif quand elle arrive, 
        /// quand elle est présente et disparaît une fois qu'elle en disparait. 
        /// </summary>
        PRESENCE_CARTE_BOARD,
        NONE
    }

    /// <summary>
    /// Tour sur lequel est la condition. 
    /// </summary>
    public enum Tour {
        /// <summary>
        /// Différent de Tour deux joueurs, ici le joueur ne peut jouer l'effet pendant aucun tour. 
        /// </summary>
        NONE,
        /// <summary>
        /// Peut-être executé sur le tour des deux joueurs. 
        /// </summary>
        TOUR_DEUX_JOUEURS,
        /// <summary>
        /// Lors du tour du joueur local
        /// </summary>
        TOUR_LOCAL,
        /// <summary>
        /// Lors du tour du joueur qui n'est pas local. 
        /// </summary>
        TOUR_NOT_LOCAL
    }

    /// <summary>
    /// True si la condition dépend de la phase. 
    /// </summary>
    public bool dependsOnPhase = false;
    public Player.Phases PhaseCondition;
    public Reaction ReactionCondition = Reaction.NONE;
    public Tour TourCondition = Tour.NONE;

    
    /// <summary>
    /// Entier propre à la condition corresponde à xx
    /// </summary>
    public int properIntCondition = 0;
    // Cet entier correspond à y
    public int nombreDeTours = 0;
    public bool ActionObligatoire = false;

    /// <summary>
    /// Lorsqu'on a le droit de faire une action une fois par tour, il faut marquer cette action comme ayant été faite.
    /// </summary> 
    public bool utilisePourCeTour = false; 

    /// <summary>
    /// Constructeur de la classe Condition
    /// </summary>
    /// <param name="_condition">La condition</param>
    /// <param name="_intCondition">L'entier qui correspond à la condition.</param>
    public Condition(ConditionEnum _condition, int _intCondition) {
        ConditionCondition = _condition;
        intCondition = _intCondition;
        understandInt();
    }

    /// <summary>
    /// Constructeur de la classe condition.
    /// Sans arguments. 
    /// </summary>
    public Condition() {


    }

    /// <summary>
    /// Methode permettant de décortiqueer l'int envoyé par la base de données gamesparks. 
    /// </summary>
    public void understandInt() {
        /*
         * Cette méthode a pour but de "disséquer" l'entier pour en tirer les informations nécessaires. 
         * Pas très élégant. Faire un switch case avec des puissances de 10? 
         */

        if (ConditionCondition == ConditionEnum.OBLIGATOIRE) {
            ActionObligatoire = true;
            ConditionCondition = ConditionEnum.NONE; 
        }

        if (intCondition < 0) {
            ActionObligatoire = true; 
        }
        int _intConditionAbs = Mathf.Abs(intCondition);

        if (_intConditionAbs < 100) {
            properIntCondition = _intConditionAbs;
        }
        else if (_intConditionAbs < Mathf.Pow(10, 3)) {
            properIntCondition = _intConditionAbs % 100;
            TourCondition = (Tour)(_intConditionAbs / Mathf.Pow(10, 2));
        }
        else if (_intConditionAbs < Mathf.Pow(10, 4)) {
            properIntCondition = _intConditionAbs % 100;
            if ((int)(_intConditionAbs / Mathf.Pow(10, 2)) % 10 != 0) {
                TourCondition = (Tour)((int)(_intConditionAbs / Mathf.Pow(10, 2)));
            }
            dependsOnPhase = true;
            PhaseCondition = (Player.Phases)((int)(_intConditionAbs / Mathf.Pow(10, 3)) - 1);
        }
        else {
            properIntCondition = _intConditionAbs % 100;
            if ((int)(_intConditionAbs / Mathf.Pow(10, 2)) % 10 != 0) {
                TourCondition = (Tour)((int)(_intConditionAbs / Mathf.Pow(10, 2)));
            }
            if ((int)(_intConditionAbs / Mathf.Pow(10, 3)) % 10 != 0) {
                dependsOnPhase = true;
                PhaseCondition = (Player.Phases)((int)(_intConditionAbs / Mathf.Pow(10, 3)) - 1);
            }
            ReactionCondition = (Reaction)((int)(_intConditionAbs / Mathf.Pow(10, 4)) - 1);
        }
    }

    /// <summary>
    /// Si le joueur a été utilisé à ce tour. 
    /// </summary>
    public void setSortUtilisePourCeTour() {
        if (TourCondition != Tour.NONE) {
            utilisePourCeTour = true;
        }
    }

    /// <summary>
    /// A chaque fin de tour, on appelle cette fonction pour remettre ou pas, 
    /// la variable utilisePourCeTour à false, dans le cas où le sort pourra
    /// être réutilisé plus tard. 
    /// </summary>
    /// <param name="isTourJoueurLocal">true, si c'est le prochain tour est celui du joueur local</param>
    public void updateUtilisePourCeTour(bool isTourJoueurLocal) {
        if (TourCondition == Tour.TOUR_DEUX_JOUEURS) {
            utilisePourCeTour = false; 
        } else if (((TourCondition == Tour.TOUR_LOCAL) && (isTourJoueurLocal)) ||
                        ((TourCondition == Tour.TOUR_NOT_LOCAL) && (!isTourJoueurLocal))){
            utilisePourCeTour = false; 
        } 
    }


}
