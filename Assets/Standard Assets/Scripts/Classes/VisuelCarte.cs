using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


/// <summary>
/// Un script qui met en place le visuel de la carte lors de son apparition. 
/// </summary>
public class VisuelCarte : MonoBehaviour{

    /// <summary>
    /// Créer le visuel de la carte. 
    /// </summary>
    public void SetUpVisuel() {
        // Récupération de tous les attributs à changer. 
        GameObject Nom = transform.Find("Name").gameObject; 

    }

    public Color GetCouleurCarte(Entite.Element element, Entite.Ascendance ascendance) {
        if (GetComponent<Carte>().GetType() == typeof(Entite)) {

        }
        
    }

}
