
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ImageCarteCimetiere : MonoBehaviour {

    private Sprite[] AllImages;

    private string info;

    private int intCarte = 0;

    /// <summary>
    /// True si la carte est choisie lors d'un choix quelconque. 
    /// </summary>
    private bool isTargeted = false;

    /// <summary>
    /// Cible créée lors d'un choix. 
    /// </summary>
    private GameObject CroixCreated;

    public string Info {
        get {
            return info;
        }

        set {
            info = value;
        }
    }

    // Use this for initialization
    void Start() {
        AllImages = Resources.LoadAll<Sprite>("Cartes");
    }

    /// <summary>
    /// Afficher l'image de la carte
    /// </summary>
    /// <param name="name">shortCode de la carte</param>
	public void setImage(string name) {
        AllImages = Resources.LoadAll<Sprite>("Cartes");
        for (int i = 0; i < AllImages.Length; ++i) {
            if (name == AllImages[i].name) {
                // On cherche la bonne image dans la liste. 
                GetComponent<Image>().sprite = AllImages[i];
                return;
            }
        }
        GetComponent<Image>().sprite = AllImages[0];
        Debug.LogWarning("La face de la carte n'a pas été trouvée" + name);
    }

    /// <summary>
    /// Mettre une image pour une carte bannie
    /// </summary>
    public void setImageBan() {
        GetComponent<Image>().sprite = Resources.Load("Ban/Ban") as Sprite; 
    }

    /// <summary>
    /// Clic sur la carte
    /// </summary>
	public void OnMouseDown() {
        GameObject.Find("GameManager").GetComponent<GameManager>().ShowCarteInfo(" ", info); 
    }
}
