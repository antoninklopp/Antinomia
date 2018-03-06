using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Event effet à gérer
/// </summary>
public class EventEffet {

    private int eventInt;

    public int EventInt {
        get {
            return eventInt;
        }

        set {
            eventInt = value;
        }
    }

    public EffetPlusInfo effet;

    /// <summary>
    /// Passe à true quand l'event est fini. 
    /// </summary>
    public bool fini = false;

    /// <summary>
    /// True si l'effet demande une interaction.
    /// </summary>
    public bool demandeInteraction = false; 

    public GameObject CarteAssociee;

    // Use this for initialization
    void Start () {
		
	}

    public EventEffet() {

    }

    public EventEffet(EffetPlusInfo ef, GameObject CarteAssociee) : this() {
        this.effet = ef;
        this.CarteAssociee = CarteAssociee;
        // Si carte nécessaire sort alors on a besoin d'une interaction.
        demandeInteraction = (ef.CartesNecessairesSort() != 0);
    }
}
