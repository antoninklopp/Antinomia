
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Une entité peut subir plusieurs changements de puissance. 
/// Cet objet est un historique de tous les changements de puissance; 
/// </summary>
public class PuissanceEntite {

    private List<ChangementPuissance> TousChangementsPuissance = new List<ChangementPuissance>();

    /// <summary>
    /// Puissance de base de l'entité. 
    /// </summary>
    private int puissanceBase = -1; 


    public PuissanceEntite() {

    }

    public PuissanceEntite(int STAT) : this() {
        puissanceBase = STAT; 
    }

    /// <summary>
    /// Ajout d'un changement de puissance à la liste. 
    /// </summary>
    /// <param name="_changement"></param>
    public void AjouterChangementPuissance(ChangementPuissance _changement) {
        TousChangementsPuissance.Add(_changement); 
    }

    public void EnleverChangementPuissance(ChangementPuissance _changement) {
        for (int i = 0; i < TousChangementsPuissance.Count; i++) {
            if (_changement == TousChangementsPuissance[i]) {
                TousChangementsPuissance.RemoveAt(i); 
            }
        }
    }

    /// <summary>
    /// Recuperer la puissance de l'entité à un certain moment. 
    /// </summary>
    /// <returns></returns>
    public int RecupererPuissanceEntite(int puissanceDeBase) {
        int puissanceCourante = puissanceDeBase; 
        for (int i = 0; i < TousChangementsPuissance.Count; i++) {
            switch (TousChangementsPuissance[i].getType()) {
                case ChangementPuissance.Type.MULTIPLICATION:
                    puissanceCourante *= TousChangementsPuissance[i].getValeur(); 
                    break;
                case ChangementPuissance.Type.ADDITION:
                    puissanceCourante += TousChangementsPuissance[i].getValeur();
                    break;
                default:
                    Debug.LogWarning("Ce changement n'est pas répertorié"); 
                    break; 
            }
        }
        return puissanceCourante; 
    }

    public int RecupererPuissanceEntite() {
        if (puissanceBase == -1) {
            throw new System.Exception("La puissance de cette entité n'a pas été initialisée"); 
        }
        return RecupererPuissanceEntite(puissanceBase); 
    }

}
