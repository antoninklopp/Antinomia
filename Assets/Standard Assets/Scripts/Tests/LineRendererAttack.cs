using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A mettre sur la carte. 
/// </summary>
public class LineRendererAttack : MonoBehaviour {

    public LineRenderer lineRenderer; 

	// Use this for initialization
	void Start () {
        LineRendererSetup();
	}
	
	// Update is called once per frame
	void Update () {
        LineRendererUpdate(); 
	}

    void LineRendererSetup() {
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1, transform.position); 

        lineRenderer.sortingOrder = 3;
    }

    void LineRendererUpdate() {
        // Debug.Log(lineRenderer); 

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, lineRenderer.transform.parent.position);
    }
}
