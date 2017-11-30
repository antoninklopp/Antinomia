using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Un changement de puissance d'une entité. 
/// </summary>
public class ChangementPuissance {

    /// <summary>
    /// Type de changement de puissance. 
    /// </summary>
	public enum Type {
        MULTIPLICATION, 
        ADDITION
    };

    private Type ChangementType;
    private int ChangementInt;

    /// <summary>
    /// IDCardGame de la carte qui a créé l'effet. 
    /// </summary>
    private int IDCard; 

    public ChangementPuissance(Type _ChangementType, int _ChangementInt) {
        ChangementType = _ChangementType;
        ChangementInt = _ChangementInt; 
    }

    public ChangementPuissance(Type _ChangementType, int _ChangementInt, int _IDCard) : this(_ChangementType, _ChangementInt) {
        IDCard = _IDCard;
    }

    public ChangementPuissance() {
    }

    public Type getType() {
        return ChangementType;
    }

    public int getValeur() {
        return ChangementInt; 
    }
    
    public int getIDCard() {
        return IDCard; 
    }

    public override bool Equals(object obj) {
        if (obj.GetType() != GetType()) {
            return false; 
        } else {
            ChangementPuissance _changement = obj as ChangementPuissance; 
            if (_changement.getType() == getType() && _changement.getValeur() == getValeur() 
                && _changement.getIDCard() == getIDCard()) {
                return true; 
            } else {
                return false; 
            }
        }
    }

    public override int GetHashCode() {
        var hashCode = 1002415977;
        hashCode = hashCode * -1521134295 + ChangementType.GetHashCode();
        hashCode = hashCode * -1521134295 + ChangementInt.GetHashCode();
        hashCode = hashCode * -1521134295 + IDCard.GetHashCode();
        return hashCode;
    }
}
