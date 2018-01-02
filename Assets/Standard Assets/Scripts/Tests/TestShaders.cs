using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShaders : MonoBehaviour {

    public Shader blackAndWhite; 

	// Use this for initialization
	void Start () {
        GetComponent<SpriteRenderer>().material.shader = blackAndWhite;  
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
