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

    private Effet effet;

    /// <summary>
    /// Passe à true quand l'event est fini. 
    /// </summary>
    public bool fini = false;

    /// <summary>
    /// True si l'effet demande une ingteraction .
    /// </summary>
    public bool demandeInteraction = false; 

    private GameObject CarteAssociee; 

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Jouer l'evenement. 
    /// </summary>
    public void Jouer() {
        CarteAssociee.GetComponent<Carte>().GererEffets(new List<Effet>() { effet }, jouerDirect:true); 
    }

    public EventEffet() {

    }

    public EventEffet(Effet ef, GameObject CarteAssociee) : this() {
        this.effet = ef;
        this.CarteAssociee = CarteAssociee; 
    }
}
