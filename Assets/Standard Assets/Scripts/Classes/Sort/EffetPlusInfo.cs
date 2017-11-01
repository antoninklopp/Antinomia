using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Un effet avec plus d'informations notamment sur sa provenance
/// </summary>
public class EffetPlusInfo : Effet {

    public int numeroEffet = 0;
    public int numeroListEffet = 0; 

    /// <summary>
    /// Constructeur de la classe EffetPlusInfo
    /// </summary>
    /// <param name="effet"></param>
    /// <param name="_numeroEffet"></param>
    /// <param name="_numeroListEffet"></param>
    public EffetPlusInfo(Effet effet, int _numeroEffet, int _numeroListEffet) {
        AllActionsEffet = effet.AllActionsEffet;
        AllConditionsEffet = effet.AllConditionsEffet;
        numeroEffet = _numeroEffet;
        numeroListEffet = _numeroListEffet; 

    }

    /// <summary>
    /// Constructeur de la classe EffetPlusInfo sans paramètre
    /// </summary>
    public EffetPlusInfo() {

    }

    /// <summary>
    /// Constructeur de la classe EffetPlusInfo
    /// </summary>
    /// <param name="_AllConditions"></param>
    /// <param name="_AllActions"></param>
    /// <param name="_numeroEffet"></param>
    /// <param name="_numeroListEffet"></param>
    public EffetPlusInfo(List<Condition> _AllConditions, List<Action> _AllActions, 
        int _numeroEffet, int _numeroListEffet)  : base (_AllConditions, _AllActions) {
        numeroEffet = _numeroEffet;
        numeroListEffet = _numeroListEffet; 
    }

}
