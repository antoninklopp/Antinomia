using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

/// <summary>
/// Effets d'une carte
/// </summary>
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

    public Effet(List<Condition> AllCondiditonsEffet, List<Action> AllActionsEffet) {
        this.AllConditionsEffet = AllCondiditonsEffet;
        this.AllActionsEffet = AllActionsEffet; 
    }


}
