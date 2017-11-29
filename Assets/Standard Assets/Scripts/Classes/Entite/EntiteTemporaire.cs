﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Toutes les caractéristiques d'une entité sauf qu'elle est temporaire sur le plateau et ne peut 
/// être instanciée que pour une montrer une position de carte dans un flux. 
/// 
/// Cette entité ne peut pas être la cible de l'attaque d'une autre entité. 
/// 
/// Pour l'instant aucun effet n'est possible sur cette carte. 
/// </summary>
public class EntiteTemporaire : MonoBehaviour {

    public string Name;
    /// <summary>
    /// IDCardGame de l'entité associée à cet carte. 
    /// </summary>
    public int IDCardGame;
    public string shortCode;

    public Entite.State carteState; 

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    /// <summary>
    /// Définition d'une entité temporaire à partir d'une autre entité. 
    /// </summary>
    /// <param name="_entite"></param>
    public EntiteTemporaire(Entite _entite) {
        setInfoEntiteTemporaire(_entite); 
    }

    public void setInfoEntiteTemporaire(Entite _entite) {
        Name = _entite.Name;
        IDCardGame = _entite.IDCardGame;
        shortCode = _entite.shortCode;

        // Comme l'entité est temporaire, on l'indique aussi avec un alpha de 1/2
        Color ancienneCouleur = GetComponent<SpriteRenderer>().color; 
        GetComponent<SpriteRenderer>().color = new Color(ancienneCouleur.r, ancienneCouleur.g, ancienneCouleur.b, 0.5f);

        gameObject.tag = "CarteTemporaire";

        GetComponent<ImageCardBattle>().setImage(shortCode);
        GetComponent<CarteType>().thisCarteType = CarteType.Type.ENTITE_TEMPORAIRE;
        GetComponent<CarteType>().instanciee = false; 
    }

    public void DetruireTemporaire() {
        Destroy(gameObject); 
    }
    
}