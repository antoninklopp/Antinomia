
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		foreach (RectTransform r in GetComponent<RectTransform>()) {
            Debug.Log(r.gameObject.name); 
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
