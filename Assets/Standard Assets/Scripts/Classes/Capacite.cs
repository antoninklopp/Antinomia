using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.UI; 

/// <summary>
/// Classe obsolete, inutilisée. 
/// </summary>
public class Capacite {
	/*
	 * Classe spécifique pour la capacité qui peut prendre des formes très diverses. 
	 * 
     * !!!!!!!                   Classe obsolète.               !!!!!!
	 */ 


	public enum CapaciteName
	{
		// Toutes les capacités possibles d'une entité. 
		PIOCHE, // Faire piocher une carte (TODO: implémenter le fait de pouvoir en piocher plusieurs)
		DEFAUSSER, // faire défausser une ou des cartes d'une main
		POSITION, // Modifier la position d'une entité
		NOMBRE_ATTAQUE,  // Modifier le nombre d'attaque
		VEROUILLER, // verouiller la position d'une entité
		DETRUIRE, // Détruire des entités. 
		MODIFIER_PUISSANCE, // Modifier la puissance d'autres entités
		DEGATS, // Infliger des dégâts au joueur adverse
		REVELER, // Révéler des cartes de la main d'un ou plusieurs joueurs. 
		RENVOYER, // Renvoyer des entités à la main. 
		CHANGEMENT_FAIBLESSE, // Changement des faiblesses face aux autres natures. 
		ANNULER_OBLIGATION, // Annuler l'obligation de d'abord attaquer les entités avant le joueur. 
		OBLIGER_ATTAQUE, // Obliger les autres entités à l'attaquer. 
		DETRUIRE_ASSISTANCE, // Détruire une/des assistances/cartes
		CONTROLE, // Prendre le contrôle d'une/plusieurs entités. 
		MODIFIER_AKA, // modifier l'AKA rémanent et/ou courant. 
		MODIFIER_NOMBRE_INVOCATION, // Modifier le nombre d'invocations élémentaires limite par tour. 
		MODIFIER_LIMITE_SORTS, //Modifier le nombre limite de sorts par tour. 
		EMPECHER_CHOIX_UNITE, // Empecher une/des unités d'être choisies.
		ATTAQUER_DIRECTEMENT, 
		NONE // Aucune Capacité
	}

	public CapaciteName name; 
	public int capaciteInt; 
	public Entite.Element capaciteElement; 
	public int coutCapacite; 

	public Capacite(){

	}

	public Capacite(CapaciteName _name, int _capaciteInt, Entite.Element _capaciteElement){
		name = _name; 
		capaciteInt = _capaciteInt;
		capaciteElement = _capaciteElement; 
	}

	// -----------------------------------------------------------------------------------------------------------
	// 												METHODES ANNEXES
	// -----------------------------------------------------------------------------------------------------------


	public CapaciteName stringToCapacite(string stringCapacite){
		/*
		 * Retourne la Carte.Capacite associée à un string
		 * 
		 * PIOCHE, // Faire piocher une carte (TODO: implémenter le fait de pouvoir en piocher plusieurs)
		 * DEFAUSSER, // faire défausser une ou des cartes d'une main
		 * POSITION, // Modifier la position d'une entité
		 * NOMBRE_ATTAQUE,  // Modifier le nombre d'attaque
		 * VEROUILLER, // verouiller la position d'une entité
		 * DETRUIRE, // Détruire des entités. 
		 * MODIFIER_PUISSANCE, // Modifier la puissance d'autres entités
		 * DEGATS, // Infliger des dégâts au joueur adverse
		 * REVELER, // Révéler des cartes de la main d'un ou plusieurs joueurs. 
		 * RENVOYER, // Renvoyer des entités à la main. 
		 * CHANGEMENT_FAIBLESSE, // Changement des faiblesses face aux autres natures. CHANGEMENT_SANS INT
		 * ANNULER_OBLIGATION, // Annuler l'obligation de d'abord attaquer les entités avant le joueur. 
		 * OBLIGER_ATTAQUE, // Obliger les autres entités à l'attaquer. 
		 * DETRUIRE_ASSISTANCE, // Détruire une/des assistances/cartes
		 * CONTROLE, // Prendre le contrôle d'une/plusieurs entités. 
		 * MODIFIER_AKA, // modifier l'AKA rémanent et/ou courant. 
		 * MODIFIER_NOMBRE_INVOCATION, // Modifier le nombre d'invocations élémentaires limite par tour. 
		 * MODIFIER_LIMITE_SORTS, //Modifier le nombre limite de sorts par tour. 
		 * EMPECHER_CHOIX_UNITE, // Empecher une/des unités d'être choisies. 
		 * NONE // Aucune Capacité
		 */  
		CapaciteName _capacite = CapaciteName.CONTROLE; 
		switch (stringCapacite) {
		case "PIOCHE":
			_capacite = CapaciteName.PIOCHE;
			break; 
		case "DEFAUSSER":
			_capacite = CapaciteName.DEFAUSSER;
			break; 
		case "POSITION":
			_capacite = CapaciteName.POSITION;
			break; 
		case "NOMBRE_ATTAQUE":
			_capacite = CapaciteName.NOMBRE_ATTAQUE; 
			break; 
		case "VEROUILLER":
			_capacite = CapaciteName.VEROUILLER;
			break; 
		case "DETRUIRE":
			_capacite = CapaciteName.DETRUIRE; 
			break; 
		case "MODIFIER_PUISSANCE":
			_capacite = CapaciteName.MODIFIER_PUISSANCE; 
			break; 
		case "DEGATS":
			_capacite = CapaciteName.DEGATS;
			break; 
		case "REVELER":
			_capacite = CapaciteName.REVELER; 
			break; 
		case "RENVOYER":
			_capacite = CapaciteName.REVELER; 
			break;
		case "CHANGEMENT_FAIBLESSE":
			_capacite = CapaciteName.CHANGEMENT_FAIBLESSE; 
			break;
		case "ANNULER_OBLIGATION":
			_capacite = CapaciteName.ANNULER_OBLIGATION; 
			break; 
		case "OBLIGER_ATTAQUE":
			_capacite = Capacite.CapaciteName.OBLIGER_ATTAQUE; 
			break; 
		case "DETRUIRE_ASSISTANCE":
			_capacite = CapaciteName.DETRUIRE_ASSISTANCE; 
			break; 
		case "CONTROLE":
			_capacite = CapaciteName.CONTROLE; 
			break; 
		case "MODIFIER_AKA":
			_capacite = CapaciteName.MODIFIER_AKA; 
			break; 
		case "MODIFIER_NOMBRE_INVOCATIONS":
			_capacite = CapaciteName.MODIFIER_NOMBRE_INVOCATION; 
			break; 
		case "MODIFIER_LIMITE_SORTS":
			_capacite = CapaciteName.MODIFIER_LIMITE_SORTS; 
			break; 
		case "MODIFIER_CHOIX_UNITE":
			_capacite = CapaciteName.EMPECHER_CHOIX_UNITE; 
			break; 
		case "NONE":
			_capacite = CapaciteName.NONE; 
			break; 
		default:
			throw new Exception ("Nom de la capacité non valable"); 
		}
		return _capacite; 
	}
}
