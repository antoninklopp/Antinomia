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
     
    protected string effetString; 

    public List<Condition> AllConditionsEffet = new List<Condition>();
    public List<Action> AllActionsEffet = new List<Action>();

    public string EffetString {
        get {
            return effetString;
        }

        set {
            effetString = value;
        }
    }

    public Effet () {

    }

    public Effet(List<Condition> AllCondiditonsEffet, List<Action> AllActionsEffet) {
        this.AllConditionsEffet = AllCondiditonsEffet;
        this.AllActionsEffet = AllActionsEffet; 
    }

    public override string ToString() {
        return effetString; 
    }

    /// <summary>
    /// Pour un sort il peut être nécessaire de choisir plusieurs cartes. 
    /// Il faut donc regarder si lors d'un sort, il n'est pas nécessaire de devoir jouer plusieurs cartes.
    /// </summary>
    public int CartesNecessairesSort() {
        foreach (Condition c in AllConditionsEffet) {
            Debug.Log(c.ConditionCondition);
            Debug.Log((int)c.ConditionCondition); 
            if (Array.IndexOf(new int[]{0, 1, 2, 3, 7, 8, 9}, (int)c.ConditionCondition) != -1) {
                return c.properIntCondition; 
            }
        }
        return 1; 
    }


}
