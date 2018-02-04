using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarteDefinition {

    public virtual bool IsEntite() {
        return false; 
    }

    public virtual bool IsSort() {
        return false;
    }

    public virtual bool IsAssistance() {
        return false; 
    }
}
