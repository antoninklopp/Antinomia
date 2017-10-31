using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Les Émanations sont des entités créées par des effets de cartes. 
/// Sauf effet contraire, elles ne procurent pas d’Aka rémanent aux joueurs. 
/// Si une émanation doit être déplacée du terrain vers la main, 
/// le deck ou la zone de bannissement, elle est détruite à la place.
/// </summary>
public class Emanation : Entite {

    public Emanation() {

    }

    /// <summary>
    /// Definition de la classe entité
    /// Ce CONSTRUCTEUR N'EST JAMAIS UTILISE
    /// </summary>
    /// <param name="_Name">Nom de l'entité</param>
    /// <param name="_ID">ID de l'entité</param>
    /// <param name="_STAT">STAT de la carte</param>
    /// <param name="_CoutAKA">Cout en AKA de la carte</param>
    /// <param name="_carteElement">élément de la carte</param>
    /// <param name="_carteAscendance">ascendance de la carte</param>
    public Emanation(string _Name, int _ID, int _STAT, int _CoutAKA, Element _carteElement,
        Ascendance _carteAscendance) {
        /*
		 * Définition d'une nouvelle Carte.
		*/
        STAT = _STAT;
        IDCardGame = _ID;
        Name = _Name;
        CoutAKA = _CoutAKA;
        carteElement = _carteElement;
        carteAscendance = _carteAscendance;
        // Pour l'instant chaque carte n'a qu'une seule capacité. 
        canGoBig = true;
    }

    /// <summary>
    /// Changement du state de la carte
    /// </summary>
    /// <param name="newState">Nouvel état de la carte.</param>
    public override void setState(string newState) {
        switch (newState) {
            case "MAIN":
                carteState = State.MAIN;
                // Dans le cas d'un retour à la main, on détruit la carte. 
                DetruireCarte();
                break;
            case "DECK":
                carteState = State.DECK;
                // Dans le cas d'un retour sur le deck, on détruit la carte. 
                DetruireCarte();
                break;
            case "BAN":
                carteState = State.BAN;
                // Dans le cas d'un ban, on détruit la carte. 
                DetruireCarte();
                break;
            case "BOARD":
                carteState = State.CHAMPBATAILLE;
                // On remet la bonne image pour la carte, dans le cas où la carte était de dos.
                GetComponent<ImageCardBattle>().setImage(shortCode);
                break;
            case "SANCTUAIRE":
                carteState = State.SANCTUAIRE;
                GetComponent<ImageCardBattle>().setImage(shortCode);
                break;
            case "CIMETIERE":
                carteState = State.CIMETIERE;
                break;
        }
    }

}
