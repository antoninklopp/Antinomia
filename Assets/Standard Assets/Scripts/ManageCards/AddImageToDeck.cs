
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI; 

public class AddImageToDeck : MonoBehaviour, IDropHandler {

	/*
	 * Ajouter une carte dans le deck grâce au Manager de cartes
	 * Grâce au système de drag and drop
	 * Et appel de la fonction sur la base de données pour update en ligne.
	 */ 

	float longueurAllCards; 
	float longueurCartesOrdinateur = 50; 
#pragma warning disable CS0414 // Le champ 'AddImageToDeck.longueurCartesAndroid' est assigné, mais sa valeur n'est jamais utilisée
	float longueurCartesAndroid = 100; 
#pragma warning restore CS0414 // Le champ 'AddImageToDeck.longueurCartesAndroid' est assigné, mais sa valeur n'est jamais utilisée
	GameObject CardPrefab;

	GameObject CardAddedText; 

	GameObject GameManagerObject; 

	List<GameObject> AllCards = new List<GameObject> (); 

	void Start(){
		CardPrefab = GameObject.Find ("Carte"); 
		CardAddedText = GameObject.Find ("Info"); 
		GameManagerObject = GameObject.Find ("GameManager"); 
	}

	#region IDropHandler implementation
	public void OnDrop(PointerEventData eventData){

		AllCards.Add (eventData.pointerDrag.gameObject);
		eventData.pointerDrag.transform.SetParent (transform); 

        if (eventData.pointerDrag.GetComponent<DragImage>() == null) {
            Debug.Log("Erreur de touche");
            return; 
        }

		if (eventData.pointerDrag.GetComponent<DragImage> ().inDeck == false) {
			AjoutCarte (eventData.pointerDrag); 
		}

		//ReorganiseCardsInScrollView (transform.GetChild(0).GetChild(0).gameObject, AllCards, 0f); 
	}
	#endregion

	void AjoutCarte(GameObject objectAdded){
        string cardName = " ";
        if (GetComponent<Entite>() != null) {
           cardName = objectAdded.GetComponent<Entite>().Name;
        } else if (GetComponent<Sort>() != null) {
            cardName = objectAdded.GetComponent<Sort>().Name;
        }
		int deckNumber = GameObject.Find ("GameManager").GetComponent<GameManagerManageCards> ().currentDeckNumber;
        if (CardAddedText != null) {
            CardAddedText.GetComponent<Text>().text = "Nouvelle Carte Ajoutée" + cardName + " au deck " + deckNumber.ToString();
        }

		GameManagerObject.GetComponent<GameManagerManageCards>().AjoutCarte(objectAdded, deckNumber); 
		// GameManagerObject.SendMessage("ReorganiseDeckCards", deckNumber); 
	}

	void ReorganiseCardsInScrollView(GameObject ScrollView, List<GameObject> Cards, float yStartPosition){
		/*
		 * Organiser les cartes dans un scrollview
		 * 
		 */ 
		longueurAllCards = ScrollView.transform.parent.parent.gameObject.GetComponent<RectTransform> ().sizeDelta.x;

		// On change le parent des cartes
		for (int i = 0; i < Cards.Count; ++i) {
			print ("ok");
			Cards [i].transform.SetParent (ScrollView.transform); 
		}

		int j = -1; 
		int nombreCartesParLigne; 
		#if (UNITY_ANDROID)
		nombreCartesParLigne = (int)(longueurAllCards/longueurCartesAndroid); 
		#else 
		nombreCartesParLigne = (int)(longueurAllCards/longueurCartesOrdinateur);
		#endif

		Debug.Log ("NOMBRE DE CARTES : " + Cards.Count); 
		// On les réorganise; 
		for (int i = 0; i < Cards.Count; ++i) {
			Debug.Log (Cards [i]); 
			// On passe sur une nouvelle ligne. 
			if (i % nombreCartesParLigne == 0) {
				j++; 
			}
#if (UNITY_ANDROID)
            Cards[i].GetComponent<RectTransform>().position = new Vector2(CardPrefab.GetComponent<RectTransform>().position.x +
            (i % nombreCartesParLigne) * (longueurCartesAndroid + 1),
            CardPrefab.GetComponent<RectTransform>().position.y + yStartPosition + j * (CardPrefab.GetComponent<RectTransform>().sizeDelta.y + 1)); 
#else 
			Cards [i].GetComponent<RectTransform> ().localPosition = new Vector2 (
				(i % nombreCartesParLigne) * (longueurCartesOrdinateur + 1),  j * (CardPrefab.GetComponent<RectTransform>().sizeDelta.y + 1)); 
#endif
		}
	}


}
