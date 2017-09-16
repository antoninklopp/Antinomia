using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class InfoCarteBattle : MonoBehaviour {
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetInfoCarte(string shortCode, string Infos) {
        transform.Find("ImageCarteInfo").gameObject.GetComponent<ImageCard>().setImage(shortCode);
        transform.Find("TextCarteInfo").gameObject.GetComponent<Text>().text = Infos; 

    }
}
