
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererAssistanceEntite : MonoBehaviour {

    private LineRenderer lineRenderer;
    public GameObject _Entite;
    public GameObject _Assistance; 

	// Use this for initialization
	void Start () {
        lineRenderer = transform.GetChild(0).gameObject.GetComponent<LineRenderer>(); 

        if (_Entite != null) {
            setLine(_Entite, _Assistance); 
        }
	}

    /// <summary>
    /// Dessiner une ligne entre une entite et une assistance. 
    /// </summary>
    /// <param name="Entite"></param>
    /// <param name="Assistance"></param>
    public void setLine(GameObject Entite, GameObject Assistance) {
        lineRenderer = transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, Entite.transform.position);
        lineRenderer.SetPosition(1, Assistance.transform.position);

        //int count = 100;
        //lineRenderer.positionCount = count + 1;
        //for (int i = 0; i <= count; i++) {
        //    // On set tous les points entre. Pour faire un forme.
        //    lineRenderer.SetPosition(i, new Vector2(Entite.transform.position.x * (count - i) / count + Assistance.transform.position.x * i / count,
        //                                            Entite.transform.position.y * (count - i) / count + Assistance.transform.position.y * i / count));
        //}

    }
    
}
