
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface de la carte, méthode nécessaires à implémenter
/// </summary>
public interface ICarte {

    void OnMouseDown();

    void OnMouseUp();

    void OnMouseOver();

    void OnMouseDrag();

    IEnumerator SetUpCard();

    void DetruireCarte();

}
