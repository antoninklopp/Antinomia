using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI; 

public class DragImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject itemBeingDragged; 
	Transform startParent; 
	Vector3 startPosition; 

	GameObject GameManagerObject;

    [HideInInspector]
	public bool inDeck; 

	void Start(){
		GameManagerObject = GameObject.Find ("GameManager"); 

	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData){
		itemBeingDragged = gameObject; 
		startPosition = transform.position;
		startParent = transform.parent; 
		transform.SetParent(GameManagerObject.transform); 
		GetComponent<CanvasGroup> ().blocksRaycasts = false;

        if (startParent.parent.parent == GameManagerObject.transform.Find("AllCards")) {
            // La carte n'est pas dans un deck au départ.
            GameManagerObject.SendMessage("ObjectBeingDragged", gameObject);
            inDeck = false;
        }
        else {
            inDeck = true;
        }
    }

	#endregion

	#region IDragHandler implementation

	public void OnDrag(PointerEventData eventData){
		transform.position = Input.mousePosition; 
	}

	#endregion

	#region IEndDraghandler implementation

	public void OnEndDrag(PointerEventData eventData){
		itemBeingDragged = null; 
		if (transform.parent == GameManagerObject.transform && !inDeck) {
			// On détruit l'objet qui était "temporaire"
			Destroy (gameObject); 
		} else if (transform.parent == GameManagerObject.transform && inDeck) {
			transform.position = startPosition; 
			transform.SetParent(startParent); 
		} else if (transform.parent == GameManagerObject.transform.Find("AllDecks") && inDeck){
			transform.position = startPosition; 
			transform.SetParent(startParent); 
		} else if (transform.parent == GameManagerObject.transform.Find("AllCards") && !inDeck){

		}

		GetComponent<CanvasGroup>().blocksRaycasts = true; 
	}

	#endregion
}
