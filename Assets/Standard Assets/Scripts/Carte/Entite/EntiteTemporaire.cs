
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
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
public class EntiteTemporaire : Carte {

    public Entite.State carteState; 

	// Use this for initialization
	public override void Start () {
        Def = new EntiteDefinition(); 
	}

    public override void OnMouseDown() {}
    public override void OnMouseDrag() {}

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

    /// <summary>
    /// Detruire l'entité temporaire.
    /// </summary>
    public void DetruireTemporaire() {
        Destroy(gameObject); 
    }

    public GameObject getVraieEntite() {
        return NetworkBehaviourAntinomia.FindCardWithID(IDCardGame); 
    }

    public override bool EstTemporaire() {
        return true; 
    }

    public override bool IsEntite() {
        return true; 
    }
}
