using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Créer un objet sur la carte. 
/// </summary>
public class LineRendererObject : MonoBehaviour {

    bool isDragging = false; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isDragging) {
            Dragging(); 
        }
        Debug.Log(isDragging); 
	}

    private void OnMouseDrag() {
        isDragging = true; 
    }

    private void OnMouseDown() {
        isDragging = true; 
    }

    private void OnMouseUp() {
        isDragging = false; 
    }

    /// <summary>
    /// Deplacer la carte. 
    /// La carte suit la souris. 
    /// </summary>
    public void Dragging() {
        /*
		 * Déplacement de la carte qui suit la souris. 
		 */
        Vector3 MousePosition = Input.mousePosition;
        MousePosition.z = 15;
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);
        transform.position = mouseWorldPoint;
    }
}
