using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerifyCartePioche {

    /// <summary>
    /// oID de la carte qui devait être piochée
    /// </summary>
    public string oID;

    /// <summary>
    ///  Passe à true si la carte a bien été piochée. 
    /// </summary>
    public bool pioche = false; 

    public VerifyCartePioche() {

    }

    public VerifyCartePioche(string oID) : this() {
        this.oID = oID;
    }
}
