using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AntinomiaException; 

/// <summary>
/// Objet permettant de verifier que toutes les cartes ont bien été piochées.
/// </summary>
public class ListCarteVerify {

    public List<VerifyCartePioche> liste = new List<VerifyCartePioche>();
    
    public ListCarteVerify() {

    }

    public void Add(VerifyCartePioche carte) {
        liste.Add(carte); 
    }

    /// <summary>
    /// Met que la carte a bien été piochée
    /// </summary>
    public void CartePiocheOK(string oID) {
        Debug.Log(liste.Count);
        Debug.Log("Recherchee " + oID); 
        for (int i = 0; i < liste.Count; i++) {
            Debug.Log(liste[i].oID); 
            if (liste[i].oID.Equals(oID)) {
                Debug.Log("Carte déjà piochée " + liste[i].pioche); 
            }
            if (liste[i].oID.Equals(oID) && !liste[i].pioche) {
                liste[i].pioche = true;
                return;
            }
        }
        throw new UnusualBehaviourException("Carte introuvable");
    }

    /// <summary>
    /// Retourne une liste d'OID des cartes qui n'ont pas été piochées. 
    /// </summary>
    /// <returns></returns>
    public List<string> oIDNonPiochees() {
        List<string> ListeRetour = new List<string>(); 
        for (int i = 0; i < liste.Count; i++) {
            if (!liste[i].pioche) {
                ListeRetour.Add(liste[i].oID);
                liste.Remove(liste[i]); 
            }
        }
        return ListeRetour; 
    }

    /// <summary>
    /// Retourne true si toutes les cartes ont été piochées, false sinon. 
    /// </summary>
    /// <returns></returns>
    public bool PiocheOK() {
        Debug.Log("On verifie toutes les cartes"); 
        foreach (VerifyCartePioche v in liste) {
            if (!v.pioche) {
                Debug.Log(v.oID + "not sOK");
                return false; 
            } else {
                Debug.Log(v.oID + "OK"); 
            }
        }
        return true; 
    }
}
