﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Certains éléments et surtout méthodes sont communs/essentiels pour
/// beaucoup d'objets du projet (autant que les méthodes Start et Update de la Network Behaviour), 
/// on crée donc une classe "De base" qui hérite de la classe unity MonoBehaviour. 
/// </summary>
public class MonoBehaviourAntinomia : MonoBehaviour {

	// Use this for initialization
	public virtual void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
	}

    /// <summary>
    /// Récupérer un objet Carte grâce à son IDCardGame. 
    /// </summary>
    /// <param name="_ID_">IDCardGame de la carte recherchée</param>
    /// <returns>la carte si elle a été trouvée, crée une exception sinon. </returns>
    public GameObject FindCardWithID(int _ID_) {
        /*
		 * Trouver la carte avec la bonne ID. 
         * Doit être la même méthode que dans player (à relier). 
		 */
        CarteType[] AllCartesType = FindObjectsOfType(typeof(CarteType)) as CarteType[];
        List<GameObject> AllCartes = new List<GameObject>();

        for (int i = 0; i < AllCartesType.Length; ++i) {
            GameObject NewCarte = AllCartesType[i].gameObject;
            if (NewCarte.GetComponent<CarteType>().instanciee) {
                AllCartes.Add(NewCarte);
            }
        }

        for (int i = 0; i < AllCartes.Count; ++i) {
            // On cherche la carte avec le bon ID
            switch (AllCartes[i].GetComponent<CarteType>().thisCarteType) {
                case CarteType.Type.ASSISTANCE:
                    if (AllCartes[i].GetComponent<Assistance>().IDCardGame == _ID_) {
                        return AllCartes[i];
                    }
                    break;
                case CarteType.Type.SORT:
                    if (AllCartes[i].GetComponent<Sort>().IDCardGame == _ID_) {
                        return AllCartes[i];
                    }
                    break;
                case CarteType.Type.ENTITE:
                    if (AllCartes[i].GetComponent<Entite>().IDCardGame == _ID_) {
                        return AllCartes[i];
                    }
                    break;
            }
        }

        throw new Exception("La carte n'a pas été trouvée");
    }

    /// <summary>
    /// Recuperer l'objet correspondant au joueur local
    /// </summary>
    /// <returns>Joueur local</returns>
    public GameObject FindLocalPlayer() {
        /*
		 * Trouver le joueur local, pour lui faire envoyer les fonctions [Command(channel=0)]
		 */
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (Players[0].GetComponent<Player>().isLocalPlayer) {
            return Players[0];
        }
        else {
            return Players[1];
        }
    }

}
