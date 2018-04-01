
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text; 

/// <summary>
/// Un effet avec plus d'informations notamment sur sa provenance
/// </summary>
public class EffetPlusInfo : Effet {

    public int numeroEffet = 0;

    public int numeroListEffet = 0;

    /// <summary>
    /// ID Card Game de la carte associee a l'effet
    /// </summary>
    public int IDCardGame = -1;

    /// <summary>
    /// Constructeur de la classe EffetPlusInfo
    /// </summary>
    /// <param name="effet"></param>
    /// <param name="_numeroEffet">Numero de l'effet dans la liste</param>
    /// <param name="_numeroListEffet">Numero de la liste
    /// 0 : si effets normaux
    /// 1 : si effets astraux
    /// 2 : si effets maléfiques</param>
    public EffetPlusInfo(Effet effet, int _numeroEffet, int _numeroListEffet) {
        AllActionsEffet = effet.AllActionsEffet;
        AllConditionsEffet = effet.AllConditionsEffet;
        numeroEffet = _numeroEffet;
        numeroListEffet = _numeroListEffet;
        EffetString = effet.EffetString;
        EstDeclarable = effet.EstDeclarable; 
        Debug.Log(effet); 
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

    public EffetPlusInfo(Effet effet, int _numeroEffet, int _numeroListEffet, int _IDCardGame) : this (effet, _numeroEffet,
        _numeroListEffet) {
        IDCardGame = _IDCardGame;
        EstDeclarable = effet.EstDeclarable;
    }

    public override string ToString() {
        return " "; 
    }

}
