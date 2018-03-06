
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
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

    public Shader blackAndWhite;
    private Shader baseShader; 

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

    /// <summary>
    /// Indiquer au joueur qu'il n'a plus de cartes disponibles, à l'aide d'un shader blanc et noir. 
    /// </summary>
    public void MakeBlackAndWhite() {
        baseShader = GetComponent<Image>().material.shader;
        GetComponent<Image>().material = Instantiate(Resources.Load("Material/BlackWhite") as Material); 
        // Si on a pas assez de cartes, on la rend "intouchable"
        // Pour ne pas s'embeteer à faire des vérifications tout le temps de si la carte est dispo
        GetComponent<Button>().interactable = false;
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<Image>().raycastTarget = false;
    }

    /// <summary>
    /// Indiquer au joueur qu'il lui reste des cartes disponible, en la mettant colorée. 
    /// </summary>
    public void MakeColored() {
        // S'il y a assez de cartes, on la rend "touchable"
        GetComponent<Image>().material = Instantiate(Resources.Load("Material/NormalMaterial") as Material);
        GetComponent<Button>().interactable = true;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<Image>().raycastTarget = true;
    }
}
