
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System; 


public class DontDestroyObjectIntro : MonoBehaviour {

    private void Awake() {
        try {
            DontDestroyOnLoad(transform.gameObject);
            transform.Find("GameManager").gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        } catch (NullReferenceException e) {
            Debug.Log(e);
            Destroy(gameObject); 
        }
    }
}
