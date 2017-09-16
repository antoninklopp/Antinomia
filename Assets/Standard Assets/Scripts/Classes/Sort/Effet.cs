using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class Effet {
    /*
     * L'effet est une association d'une liste de conditions et d'une liste d'actions. 
     * 
     * Décrypter le string en arrivée de la base de données gamesparks. 
     * 
     * Effet1 : Effet2 : Effet3....
     * avec Effet1 => Condition1 ; Condition2 ; Condition3 ; ... ! Action1 ; Action2 ; Action3 ; ... . 
     * 
     */

    public List<Condition> AllConditionsEffet = new List<Condition>();
    public List<Action> AllActionsEffet = new List<Action>();

    public Effet () {

    }

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {

    }


}
