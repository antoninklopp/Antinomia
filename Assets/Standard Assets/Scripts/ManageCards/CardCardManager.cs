using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 

/// <summary>
/// Carte du card manager. 
/// C'est un élément de UI. 
/// </summary>
public class CardCardManager : MonoBehaviour {

	Entite CarteScript;
    Sort SortScript;
    Assistance AssistanceScript; 

	// La carte zoom associée à laquelle on envoie les informations
	public GameObject CarteZoom; 
	// Use this for initialization

	void Start () {
		try {
            if (GetComponent<Entite>() != null) {
                CarteScript = GetComponent<Entite>();
                GetComponent<ImageCard>().setImage(CarteScript.shortCode);
            } else if (GetComponent<Sort>() != null) {
                SortScript = GetComponent<Sort>();
                GetComponent<ImageCard>().setImage(SortScript.shortCode); 
            } else if (GetComponent<Assistance>() != null) {
                AssistanceScript = GetComponent<Assistance>();
                GetComponent<ImageCard>().setImage(AssistanceScript.shortCode);
            }
		} catch (NullReferenceException){
			Debug.Log ("Mince alors"); 
		}
	}

	void setInformation(){


	}

    /// <summary>
    /// Click sur la carte. 
    /// </summary>
	public void OnClickCarte(){
        /*
		 * Lors d'un clic sur la carte
		 */
        if (CarteScript != null) {
            CarteZoom.GetComponent<CarteZoom>().updateInfo(CarteScript.Name, CarteScript.shortCode,
                CarteScript.GetInfoCarte());
        } else if (SortScript != null) {
            CarteZoom.GetComponent<CarteZoom>().updateInfo(SortScript.Name, SortScript.shortCode,
                SortScript.GetInfoCarte()); 
        } else if (AssistanceScript != null) {
            CarteZoom.GetComponent<CarteZoom>().updateInfo(AssistanceScript.Name, AssistanceScript.shortCode,
                AssistanceScript.GetInfoCarte());
        }
	}
}
