
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

/// <summary>
/// Informations sur les cartes
/// </summary>
public class InfoCarteBattle : MonoBehaviour {

    public void SetInfoCarte(string shortCode, string Infos) {
        transform.Find("ImageCarteInfo").gameObject.GetComponent<ImageCard>().setImage(shortCode);
        transform.Find("TextCarteInfo").gameObject.GetComponent<Text>().text = Infos; 
    }
}
