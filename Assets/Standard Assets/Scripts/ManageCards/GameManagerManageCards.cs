using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

/// <summary>
/// Game Manager de la scène où l'on peut manager ses cartes. 
/// Gère les éléments de UI. 
/// </summary>
public class GameManagerManageCards : MonoBehaviour {

	GameObject CarteZoom; 
	GameObject AllCards;
	GetPlayerInfoGameSparks playerInfo;

	GameObject CardPrefab; 
	// Content du scrollview qui contient toutes les cartes
	GameObject ContentAllCards; 
	GameObject ContentAllDecks; 

#pragma warning disable CS0414 // Le champ 'GameManagerManageCards.longueurCartesAndroid' est assigné, mais sa valeur n'est jamais utilisée
	private float longueurCartesAndroid = 70f; 
#pragma warning restore CS0414 // Le champ 'GameManagerManageCards.longueurCartesAndroid' est assigné, mais sa valeur n'est jamais utilisée
	private float longueurCartesOrdinateur = 35f;
	private float longueurAllCards;

    public float rapportLongueurHauteurCarte = 1.5f;

	List<Deck> allDecksGlobal = new List<Deck>();
	List<GameObject> listAllCards = new List<GameObject>(); 

	// Le deck qui est en train d'être regardé. 
	public int currentDeckNumber = 1; 

	// Use this for initialization
	void Start () {
		CarteZoom = GameObject.Find ("CarteZoom"); 
		AllCards = GameObject.Find ("AllCards"); 
		playerInfo = GetComponent<GetPlayerInfoGameSparks> ();
		CardPrefab = GameObject.Find ("Carte");
		ContentAllCards = GameObject.Find ("ContentAllCards"); 
		ContentAllDecks = GameObject.Find ("ContentAllDecks"); 

		longueurAllCards = ContentAllCards.transform.parent.parent.gameObject.GetComponent<RectTransform> ().sizeDelta.x; 

		// On change la taille de la card prefab

#if (UNITY_ANDROID)
		CardPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(longueurCartesAndroid,
			CardPrefab.GetComponent<RectTransform>().sizeDelta.y/CardPrefab.GetComponent<RectTransform>().sizeDelta.x * longueurCartesAndroid); 
#else 
		CardPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(longueurCartesOrdinateur,
			CardPrefab.GetComponent<RectTransform>().sizeDelta.y/CardPrefab.GetComponent<RectTransform>().sizeDelta.x * longueurCartesOrdinateur);
#endif

        StartCoroutine(setAllCards ()); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator setAllCards(){
		/*
		 * Inventaire de toutes les cartes, situé en haut
		 * 
		 */ 
		Debug.ClearDeveloperConsole (); 
		yield return playerInfo.WaitForPlayerCards (CardPrefab); 
		List<GameObject> AllCards = playerInfo.GetAllCards (CardPrefab); 
		listAllCards = AllCards; 
		// On attent l'arrivée des données. 
		// yield return new WaitForSeconds (1f); 

		ReorganiseCardsInScrollView (ContentAllCards, AllCards, 0f);

		StartCoroutine (setAllDecks (1)); 
	}

	IEnumerator setAllDecks(int deckNumber){
		/*
		 * Inventaire des decks, situé en bas. 
		 */ 
		yield return playerInfo.WaitForPlayerDecks (CardPrefab); 
		List<Deck> allDecks = playerInfo.GetAllDecks (CardPrefab); 
		allDecksGlobal = allDecks; 
		// Debug.Log (allDecks.Count.ToString() + "               fhzihfuzehfuozhfoeznfojzbùefhzbe"); 
		yield return new WaitForSeconds (0.1f); 
		for (int i = 0; i < allDecksGlobal.Count; ++i) {
			for (int j = 0; j < allDecksGlobal [i].Cartes.Count; ++j) {
				if (i == deckNumber - 1){
					allDecksGlobal [i].Cartes [j].SetActive (true); 
				} else {
					allDecksGlobal [i].Cartes [j].SetActive (false); 
				}
			}
		}
		ReorganiseCardsInScrollView (ContentAllDecks, allDecks[deckNumber - 1].Cartes, 225f); 
	}

	public void changeDeckView(int deckNumber){
		/*
		 * On active les bonnes cartes et on désactive les autres. 
		 */
		for (int i = 0; i < allDecksGlobal.Count; ++i) {
			for (int j = 0; j < allDecksGlobal [i].Cartes.Count; ++j) {
				if (i == deckNumber - 1){
					allDecksGlobal [i].Cartes [j].SetActive (true); 
				} else {
					allDecksGlobal [i].Cartes [j].SetActive (false); 
				}
			}
		}
		ReorganiseCardsInScrollView (ContentAllDecks, allDecksGlobal [deckNumber - 1].Cartes, 225f); 
		currentDeckNumber = deckNumber; 
	}

	void ReorganiseCardsInScrollView(GameObject ScrollView, List<GameObject> Cards, float yStartPosition){
        /*
		 * Organiser les cartes dans un scrollview
		 * 
		 */
        CardPrefab.SetActive(true);
		longueurAllCards = ScrollView.transform.parent.parent.gameObject.GetComponent<RectTransform> ().sizeDelta.x;

        float scale = 1f; 
		// On change le parent des cartes
		for (int i = 0; i < Cards.Count; ++i) {
			Cards [i].transform.SetParent (ScrollView.transform, false); 
		}

		int j = -1; 
		int nombreCartesParLigne;
#if (UNITY_ANDROID)
		nombreCartesParLigne = (int)(longueurAllCards/longueurCartesAndroid);
                ContentAllCards.GetComponent<GridLayoutGroup>().cellSize = new Vector2(longueurCartesAndroid, longueurCartesAndroid * rapportLongueurHauteurCarte); 
#else
        if (Cards.Count == 0) {
            nombreCartesParLigne = (int)(longueurAllCards / longueurCartesOrdinateur);
            //scale = Cards[0].GetComponent<RectTransform>().transform.localScale.x; 
        } else {
            nombreCartesParLigne = (int)(longueurAllCards / (longueurCartesOrdinateur));
        }
        ContentAllCards.GetComponent<GridLayoutGroup>().cellSize = new Vector2(longueurCartesOrdinateur, longueurCartesOrdinateur * rapportLongueurHauteurCarte); 
#endif

        Debug.Log("nombre cartes par ligne" + nombreCartesParLigne.ToString());
        Debug.Log(longueurAllCards); 
		// On les réorganise; 
		for (int i = 0; i < Cards.Count; ++i) {
			// On passe sur une nouvelle ligne. 
			if (i % nombreCartesParLigne == 0) {
				j++; 
			}

            CardPrefab.GetComponent<RectTransform>().localScale = Vector3.one;
            // INUTILE Avec les grid layout group!
#if (UNITY_ANDROID)
            Cards[i].GetComponent<RectTransform>().position = new Vector2(CardPrefab.GetComponent<RectTransform>().position.x +
            (i % nombreCartesParLigne) * (longueurCartesAndroid + 1),
            CardPrefab.GetComponent<RectTransform>().position.y + yStartPosition + j * (CardPrefab.GetComponent<RectTransform>().sizeDelta.y + 1)); 
#else
            Cards [i].GetComponent<RectTransform> ().position = new Vector2 (CardPrefab.GetComponent<RectTransform> ().position.x + 
				(i % nombreCartesParLigne) * (longueurCartesOrdinateur/scale), 
				CardPrefab.GetComponent<RectTransform> ().position.y - yStartPosition - j * ((CardPrefab.GetComponent<RectTransform>().sizeDelta.y + 1)/scale));
#endif
            
        }
        CardPrefab.SetActive(false); 
        //Time.timeScale = 0f;
    }

	public void ReorganiseDeckCards(int deckNumber){
		ReorganiseCardsInScrollView (ContentAllDecks, allDecksGlobal [deckNumber - 1].Cartes, 225f); 
		Debug.Log ("On reorganise"); 
	}

	void ObjectBeingDragged(GameObject draggedObject){
		/*
		 * Lorsqu'un objet va être drag and drop,
		 * on en crée un autre qui va prendre sa place 
		 * et celui qui est drag va devenir un objet temporaire. 
         * 
		 */ 

		for (int i = 0; i < listAllCards.Count; ++i) {
			if (draggedObject == listAllCards [i]) {
				GameObject newDraggedObject = Instantiate (draggedObject);
				newDraggedObject.transform.SetParent (ContentAllCards.transform);
				newDraggedObject.GetComponent<CanvasGroup> ().blocksRaycasts = true; 
				newDraggedObject.GetComponent<RectTransform> ().localScale = draggedObject.GetComponent<RectTransform> ().localScale; 
				listAllCards.Insert (i, newDraggedObject); 
				listAllCards.Remove (draggedObject);
                newDraggedObject.transform.SetParent(ContentAllCards.transform, false);
                Debug.Log(draggedObject.transform.GetSiblingIndex()); 
                newDraggedObject.transform.SetSiblingIndex(draggedObject.transform.GetSiblingIndex()); 
				//ReorganiseCardsInScrollView (ContentAllCards, listAllCards, 0f); 
				break; 
			}
		}

	}

	public void AjoutCarte(GameObject NouvelleCarte, int deckNumber){
		/*
		 * Ajouter une carte à un deck. 
		 * La liste des decks commence à être numérotée à 0. 
		 */ 
		Debug.Log ("APPPEL DE LA FONCTION");
		if (CheckIfCardNotInDeck(deckNumber, NouvelleCarte)) {
			allDecksGlobal [deckNumber - 1].AjouterCarte (NouvelleCarte);
			playerInfo.AddCardToDeck (GetIDAllCards(NouvelleCarte), deckNumber); 
			Debug.Log ("NP"); 
		}

		Debug.Log ("Carte Ajoutee au deck " + deckNumber.ToString ()); 
	}

	bool CheckIfCardNotInDeck(int deckNumber, GameObject Card){
		/*
		 * Vérifier si la carte n'est pas déjà dans le deck.
		 */ 
		Deck _deck = allDecksGlobal [deckNumber - 1]; 
		for (int i = 0; i < _deck.Cartes.Count; ++i) {
			if (GetIDAllCards(Card) == GetIDAllCards(_deck.Cartes [i])) {
				//La carte est déjà dans le deck
				GameObject.Find("Info").GetComponent<Text>().text = "La carte est déjà présente dans le deck"; 
				Destroy (Card); 
				return false; 
			}
		} 

		return true; 
	}

	public void EnleverCarte(GameObject AncienneCarte, int deckNumber){
		/*
		 * Enlever une carte d'un deck. 
		 * la liste des decks commence à être numérotée à 0. 
		 */ 
		allDecksGlobal [deckNumber - 1].EnleverCarte (AncienneCarte); 
		playerInfo.RemoveCardFromDeck (GetIDAllCards(AncienneCarte), deckNumber); 
		Debug.Log ("NP"); 

		Destroy (AncienneCarte); 
	}

	public void GoBackToMenu(){
		SceneManager.LoadScene ("MainMenu"); 
	}

    public int GetIDAllCards(GameObject _carte) {
        /*
         * Retourne l'IDAllCards de la carte en cherchant si la carte est un sort ou une carte "normale"; 
         */
        if (_carte.GetComponent<Entite>() != null) {
            return _carte.GetComponent<Entite>().IDAllCards;
        } else if (_carte.GetComponent<Sort>() != null) {
            return _carte.GetComponent<Sort>().IDAllCards;
        } else if (_carte.GetComponent<Assistance>() != null){
            return _carte.GetComponent<Assistance>().IDAllCards;
        } else {
            throw new System.Exception("Probleme lors de la récupération de l'IDAllCards");
        }

    }
}
