using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererSort : LineRendererAttack {

    /// <summary>
    /// Toutes les lignes instantiées pour l'effet
    /// Car un sort peut avoir plusieurs cibles.
    /// On garde la currentInstantiatedLine comme ligne courante (de la classe parent). 
    /// </summary>
    public List<GameObject> AllInstantiatedLines = new List<GameObject>();

    /// <summary>
    /// Toutes les cibles finales. 
    /// </summary>
    public List<GameObject> AllFinalTarget = new List<GameObject>();

    public bool EnCoursGlobal;

    public bool Probleme = false; 

    private int NombreCibles = 0; 

    /// <summary>
    /// 
    /// </summary>
    public void OnBeginLineRendererSort(int nombreCibles) {
        EnCoursGlobal = true; 
        NombreCibles = nombreCibles;
        OnBeginDragAttack(); 
    }

    protected override void OnMouseUp() {
        base.OnMouseUp();
        // Une cible de plus a été choisie. 
        NombreCibles--;

        // Dans le cas où l'on a pas trouvé de cible (le joueur était trop loin
        // d'une cible réelle), on annule le sort. 
        if (FinalTarget == null) {
            EnCoursGlobal = false;
            Probleme = true;
            return; 
        }

        AllFinalTarget.Add(FinalTarget);
        FinalTarget = null; 
        if (NombreCibles == 0) {
            EnCoursGlobal = true; 
        } else {
            OnBeginDragAttack(); 
        }
    }

    /// <summary>
    /// Récupérer toutes les cibles. 
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetAllFinalTarget() {
        if (Probleme) {
            return null; 
        }

        if (!EnCoursGlobal) {
            return null; 
        }
        return AllFinalTarget; 
    }

}
