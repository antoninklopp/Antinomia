using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using UnityEngine.Events; 

/// <summary>
/// Game Manager de la scène où l'on peut manager ses cartes. 
/// Gère les éléments de UI. 
/// </summary>
public class GameManagerManageCards : MonoBehaviour {

    public Shader blackAndWhite;

    /// <summary>
    /// Dictionnaire des cartes uniques
    /// </summary>
    Dictionary<string, List<GameObject>> uniqueCards; 

    GameObject CarteZoom; 
	GameObject AllCards;
	GetPlayerInfoGameSparks playerInfo;

	GameObject CardPrefab; 
	
    /// <summary>
    /// ScrollView qui contient toutes les cartes du joueur
    /// </summary>
	GameObject ContentAllCards; 

    /// <summary>
    /// ScrollView qui contient tous les deck du joueur. 
    /// </summary>
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

    GameObject DecksButton;
    public GameObject DeckButtonPrefab;
    /// <summary>
    /// Nombre de decks du joueur. 
    /// </summary>
    int deckNumberTotal;

    /// <summary>
    /// Liste de tous les boutons de decks. 
    /// </summary>
    List<GameObject> allDecksButtonsList;

    private GameObject SupprimerDeck;

    /// <summary>
    /// Objet qui contient diverses informations sur le deck.
    /// </summary>
    private GameObject DeckInfoObject; 

    // Use this for initialization
    void Start() {
        CarteZoom = GameObject.Find("CarteZoom");
        AllCards = GameObject.Find("AllCards");
        playerInfo = GetComponent<GetPlayerInfoGameSparks>();
        CardPrefab = GameObject.Find("Carte");
        ContentAllCards = GameObject.Find("ContentAllCards");
        ContentAllDecks = GameObject.Find("ContentAllDecks");
        SupprimerDeck = GameObject.Find("SupprimerDeck");
        SupprimerDeck.SetActive(false); 

        DecksButton = GameObject.Find("DecksButton");
        DeckInfoObject = GameObject.Find("DeckInfo");
        DeckInfoObject.SetActive(false); 

        longueurAllCards = ContentAllCards.transform.parent.parent.gameObject.GetComponent<RectTransform>().sizeDelta.x;

        // On change la taille de la card prefab

#if (UNITY_ANDROID)
        CardPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(longueurCartesAndroid,
            CardPrefab.GetComponent<RectTransform>().sizeDelta.y / CardPrefab.GetComponent<RectTransform>().sizeDelta.x * longueurCartesAndroid);
#else
		CardPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(longueurCartesOrdinateur,
			CardPrefab.GetComponent<RectTransform>().sizeDelta.y/CardPrefab.GetComponent<RectTransform>().sizeDelta.x * longueurCartesOrdinateur);
#endif

        StartCoroutine(setAllDecks(1));
    }

    /// <summary>
    /// Ajouter toutes les cartes dans le cadre des cartes
    /// </summary>
    /// <returns></returns>
	IEnumerator setAllCards(){

		yield return playerInfo.WaitForPlayerCards (CardPrefab); 
		List<GameObject> AllCards = playerInfo.GetAllCards (CardPrefab); 
		listAllCards = AllCards;
        // On attent l'arrivée des données. 
        // yield return new WaitForSeconds (1f); 

        uniqueCards = getAllUniqueCards(AllCards); 

        ReorganiseCardsInScrollView (ContentAllCards, uniqueCards, 0f, initialize : true);
        
	}

    /// <summary>
    /// Ajouter tous les decks dans le cadre des decks
    /// </summary>
    /// <param name="deckNumber"></param>
    /// <returns></returns>
	IEnumerator setAllDecks(int deckNumber){
        /*
		 * Inventaire des decks, situé en bas. 
		 */

        Debug.Log("DECKS"); 
		yield return playerInfo.WaitForPlayerDecks (CardPrefab); 
		List<Deck> allDecks = playerInfo.GetAllDecks (CardPrefab);
        // On récupère le nombre de decks. 
        this.deckNumberTotal = allDecks.Count;
        SetButtonsDecks();
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

        StartCoroutine(setAllCards());
    }

    /// <summary>
    /// Changer le deck ue l'on voit
    /// </summary>
    /// <param name="deckNumber"></param>
	public void changeDeckView(int deckNumber){
        /*
		 * On active les bonnes cartes et on désactive les autres. 
		 */
        Debug.Log("DECK NUMBER" + (deckNumber - 1).ToString()); 
		for (int i = 0; i < allDecksGlobal.Count; ++i) {
			for (int j = 0; j < allDecksGlobal [i].Cartes.Count; ++j) {
				if (i == deckNumber - 1){
					allDecksGlobal [i].Cartes [j].SetActive (true); 
				} else {
					allDecksGlobal [i].Cartes [j].SetActive (false); 
				}
			}
		}

        currentDeckNumber = deckNumber;
        ReorganiseCardsInScrollView (ContentAllDecks, allDecksGlobal [deckNumber - 1].Cartes, 225f);
        ReorganiseCardsInScrollView (ContentAllCards, uniqueCards, 0, false);

        DisplayDeckInfo(deckNumber); 
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ScrollView"></param>
    /// <param name="Cards"></param>
    /// <param name="yStartPosition"></param>
	void ReorganiseCardsInScrollView(GameObject ScrollView, List<GameObject> Cards, float yStartPosition){
        /*
		 * Organiser les cartes dans un scrollview
		 */

        PutCardsInScrollView(ScrollView, Cards, yStartPosition); 
        CardPrefab.SetActive(false); 

        foreach( GameObject Carte in Cards) {
            // On désactive le numéro
            Carte.transform.GetChild(0).gameObject.SetActive(false); 
        }
    }

    void PutCardsInScrollView(GameObject ScrollView, List<GameObject> Cards, float yStartPosition) {
        // CardPrefab.SetActive(true);
        longueurAllCards = ScrollView.transform.parent.parent.gameObject.GetComponent<RectTransform>().sizeDelta.x;
        Debug.Log("On met les cartes dans la scrollView"); 
        // float scale = 1f;

        // On desactive les cartes deja présentes dans le scrollView
        foreach (Transform t in ScrollView.transform) {
            t.gameObject.SetActive(false);
        }

        // On change le parent des cartes
        for (int i = 0; i < Cards.Count; ++i) {
            Cards[i].SetActive(true); 
            Cards[i].transform.SetParent(ScrollView.transform, false);
        }

        int j = -1;
        int nombreCartesParLigne;
#if (UNITY_ANDROID)
		nombreCartesParLigne = (int)(longueurAllCards/longueurCartesAndroid);
                ContentAllCards.GetComponent<GridLayoutGroup>().cellSize = new Vector2(longueurCartesAndroid, longueurCartesAndroid * rapportLongueurHauteurCarte);

        // On change la taille de l'objet qui accueille les cartes. 
        ScrollView.transform.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
            ScrollView.transform.gameObject.GetComponent<RectTransform>().sizeDelta.x, longueurCartesAndroid *
            rapportLongueurHauteurCarte * Cards.Count / nombreCartesParLigne); 

#else
        if (Cards.Count == 0) {
            nombreCartesParLigne = (int)(longueurAllCards / longueurCartesOrdinateur);
            //scale = Cards[0].GetComponent<RectTransform>().transform.localScale.x; 
        }
        else {
            nombreCartesParLigne = (int)(longueurAllCards / (longueurCartesOrdinateur));
        }
        ContentAllCards.GetComponent<GridLayoutGroup>().cellSize = new Vector2(longueurCartesOrdinateur, longueurCartesOrdinateur * rapportLongueurHauteurCarte);
        // On change la taille de l'objet qui accueille les cartes. 
        ScrollView.transform.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
            ScrollView.transform.gameObject.GetComponent<RectTransform>().sizeDelta.x, longueurCartesOrdinateur *
            rapportLongueurHauteurCarte * Cards.Count / nombreCartesParLigne);
#endif
    }

    /// <summary>
    /// Equivalent de <see cref="ReorganiseCardsInScrollView(GameObject, Dictionary{string, List{GameObject}}, float)" />
    /// mais avec 
    /// </summary>
    /// <param name="ScrollView"></param>
    /// <param name="Cards"></param>
    /// <param name="yStartPosition"></param>
    void ReorganiseCardsInScrollView(GameObject ScrollView, Dictionary<string, List<GameObject>> Cards, float yStartPosition, 
        bool initialize=false) {

        List<GameObject> CartesIssues = new List<GameObject>();

        foreach (KeyValuePair<string, List<GameObject>> Carte in Cards) {
            CartesIssues.Add(Carte.Value[0]);
            // On met dans les cartes globales le nombre de cartes globales du joueur moins
            // le nombre de cartes dans le deck courant.
            updateNombreCartes(Carte.Value[0], Carte.Value.Count - getNombreOfCartesInDeck(Carte.Value[0], currentDeckNumber)); 
        }

        if (initialize) {
            Debug.Log("On initialise avec " + CartesIssues.Count.ToString() + " cartes"); 
            PutCardsInScrollView(ScrollView, CartesIssues, yStartPosition); 
        }
    }


    public void ReorganiseDeckCards(int deckNumber){
		ReorganiseCardsInScrollView (ContentAllDecks, allDecksGlobal [deckNumber - 1].Cartes, 225f); 
		Debug.Log ("On reorganise"); 
	}

    /// <summary>
    /// Call lorsqu'un objet va être drag and drop
    /// </summary>
    /// <param name="draggedObject">Objet drag</param>
    /// <param name="indexSibling">index de draggedObject dans la hiérarchie.</param>
	public void ObjectBeingDragged(GameObject draggedObject, int indexSibling){
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
                newDraggedObject.transform.SetSiblingIndex(indexSibling); 
				// ReorganiseCardsInScrollView (ContentAllCards, listAllCards, 0f); 
				break; 
			}
		}

	}

    /// <summary>
    /// Ajouter une carte à un deck. 
    /// La liste des decks commence à être numérotée à 0. 
    /// </summary>
    /// <param name="NouvelleCarte"></param>
    /// <param name="deckNumber"></param>
	public void AjoutCarte(GameObject NouvelleCarte, int deckNumber){ 
		Debug.Log ("APPPEL DE LA FONCTION");

        // on recherche une des cartes qui n'est pas déjà dans le deck 
        

		if (CheckIfCardNotInDeck(deckNumber, NouvelleCarte)) {
			allDecksGlobal [deckNumber - 1].AjouterCarte (NouvelleCarte);
			playerInfo.AddCardToDeck (GetIDAllCards(NouvelleCarte), deckNumber); 
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

    /// <summary>
    /// Créer les boutons de decks. 
    /// </summary>
    public void SetButtonsDecks() {
        allDecksButtonsList = new List<GameObject>(); 
        for (int i = 0; i < deckNumberTotal; i++) {
            GameObject newButton = Instantiate(DeckButtonPrefab);
            newButton.transform.SetParent(DecksButton.transform.Find("Viewport").Find("Content"), false);
            newButton.transform.Find("Text").gameObject.GetComponent<Text>().text = "DECK " + (i + 1).ToString();
            int number = i; 
            // Changement de deck. 
            newButton.GetComponent<Button>().onClick.AddListener(delegate { changeDeckView(number + 1); });
            newButton.GetComponent<Button>().onClick.AddListener(delegate { changeColorDeckButton(number); });
            
            allDecksButtonsList.Add(newButton); 
        }
    }
    
    public void changeColorDeckButton(int number) {
        for (int i = 0; i < allDecksButtonsList.Count; i++) {
            if (i == number) {
                allDecksButtonsList[i].GetComponent<Image>().color = Color.green; 
            } else {
                allDecksButtonsList[i].GetComponent<Image>().color = Color.white;
            }
        }
    }

    /// <summary>
    /// Ajouter un deck au joueur
    /// </summary>
    public void AjouterDeck() {
        // On crée un deck 
        StartCoroutine(playerInfo.createDeckRoutine());
        // On update l'UI, en laissant le deck courant. 
        StartCoroutine(setAllDecks(currentDeckNumber)); 
    }

    public void RemoveDeck() {
        // Il faut demander au joueur s'il est sûr de vouloir supprimer le deck. 
        SupprimerDeck.SetActive(true);
        SupprimerDeck.GetComponent<SupprimerDeck>().Supprimer(currentDeckNumber); 
    }

    /// <summary>
    /// Recupère la réponse de supprimer deck après une demande de suppression par le joueur. 
    /// </summary>
    /// <param name="reponse">1 si oui, 2 si non</param>
    public void ReponseRemoveDeck(int reponse) {
        if (reponse == 1) {
            playerInfo.removeDeck(currentDeckNumber); 
        }
        SupprimerDeck.SetActive(false); 
    }

    /// <summary>
    /// Récupérer l'ensemble des cartes uniques et les numéros associés. 
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, List<GameObject>> getAllUniqueCards(List<GameObject> AllCards) {
        Dictionary<string, List<GameObject>> UniqueCards = new Dictionary<string, List<GameObject>>(); 
        foreach (GameObject card in AllCards) {
            if (UniqueCards.ContainsKey(card.GetComponent<Carte>().Name)) {
                UniqueCards[card.GetComponent<Carte>().Name].Add(card); 
            } else {
                UniqueCards.Add(card.GetComponent<Carte>().Name, new List<GameObject>() { card }); 
            }
        }

        return UniqueCards; 
    }

    /// <summary>
    /// Changer le nombre de cartes. 
    /// </summary>
    /// <param name="Carte"></param>
    /// <param name="nombre"></param>
    public void updateNombreCartes(GameObject Carte, int nombre) {

        if (nombre < 0) {
            // Il y a un probleme dans la fonction
            Debug.LogError("Il ne peut pas y avoir de cartes avec un nombre négatif dans le deck"); 
            return; 
        }

        if (nombre > 0) {
            Carte.transform.GetChild(0).gameObject.GetComponent<Text>().text = nombre.ToString();
            // S'il y a assez de cartes, on la rend "touchable"
            Carte.GetComponent<Button>().interactable = true;
            Carte.GetComponent<CanvasGroup>().interactable = true;
            Carte.GetComponent<Image>().raycastTarget = true;
        }
        else {
            Carte.GetComponent<SpriteRenderer>().material.shader = blackAndWhite;
            // Si on a pas assez de cartes, on la rend "intouchable"
            // Pour ne pas s'embeteer à faire des vérifications tout le temps de si la carte est dispo
            Carte.GetComponent<Button>().interactable = false;
            Carte.GetComponent<CanvasGroup>().interactable = false;
            Carte.GetComponent<Image>().raycastTarget = false; 
        }
    }

    /// <summary>
    /// Recupérer le nombre de fois ou une instance de la carte est dans le deck
    /// </summary>
    /// <param name="Carte"></param>
    /// <param name="deckNumber"></param>
    /// <returns></returns>
    public int getNombreOfCartesInDeck(GameObject Carte, int deckNumber) {
        return getNombreOfCartesinDeck(Carte.GetComponent<Carte>().shortCode, deckNumber); 
    }

    /// <summary>
    /// <see cref="getNombreOfCartesInDeck(GameObject, int)"/>
    /// </summary>
    /// <param name="shortCode"></param>
    /// <param name="deckNumber"></param>
    /// <returns></returns>
    public int getNombreOfCartesinDeck(string shortCode, int deckNumber) {
        int nombreInstances = 0; 
        foreach (GameObject Carte in allDecksGlobal[deckNumber - 1].Cartes) {
            if (Carte.GetComponent<Carte>().Name.Equals(shortCode)) {
                nombreInstances++; 
            }
        }
        return nombreInstances; 
    }

    /// <summary>
    /// Rechercher des cartes et les mettre dans le scrollView
    /// </summary>
    /// <param name="research"></param>
    public void RechercheCarte(string research) {
        List<GameObject> CartesCorres = CartesCorrespondantes(research);
        PutCardsInScrollView(AllCards, CartesCorres, 0); 
    }

    /// <summary>
    /// Recuperer la liste des cartes qui correspondent à une recherche du joueur
    /// </summary>
    /// <param name="research"></param>
    /// <returns></returns>
    private List<GameObject> CartesCorrespondantes(string research) {
        List<GameObject> listeCartes = new List<GameObject>(); 
        foreach (KeyValuePair<string, List<GameObject>> cartesUniques in uniqueCards) {
            if (getStringWithoutAccent(cartesUniques.Key).IndexOf(getStringWithoutAccent(research)) != -1) {
                listeCartes.Add(cartesUniques.Value[0]); 
            }
        }
        return listeCartes; 
    }

    public static string getStringWithoutAccent(string accentedStr) {
        byte[] tempBytes;
        tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(accentedStr);
        string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
        return asciiStr; 
    }

    /// <summary>
    /// Filtrer les cartes selon un type d'entité. 
    /// Methode appelé par un bouton de sélection. 
    /// </summary>
    /// <param name="type"></param>
    public void FiltreType(string type) {
        CarteType.Type typeCarte = CarteType.Type.AUCUN;
        GameObject BoutonPresse = null; 
        switch (type) {
            case "entite":
                typeCarte = CarteType.Type.ENTITE;
                BoutonPresse = GameObject.Find("FiltreEntite"); 
                break; 
            case "sort":
                typeCarte = CarteType.Type.SORT;
                BoutonPresse = GameObject.Find("FiltreSort");
                break; 
            case "assistance":
                typeCarte = CarteType.Type.ASSISTANCE;
                BoutonPresse = GameObject.Find("FiltreAssistance");
                break;
            default:
                Debug.LogError("Il y a une erreur dans le type d'entité\n" +
                    "Type d'entité inexistant.");
                return;
        }

        if (BoutonPresse.GetComponent<Image>().color == Color.green) {
            // Le filtre est actif
            // on le desactive
            BoutonPresse.GetComponent<Image>().color = Color.white;
            ReorganiseCardsInScrollView(ContentAllCards, uniqueCards, 0);
        } else {
            // Le filtre est actif. 
            BoutonPresse.GetComponent<Image>().color = Color.green;
            // On desactive les couleurs d'autres filtres si ils étaient actifs
            GameObject Filtres = GameObject.Find("FiltresCartes");
            for (int i = 0; i < Filtres.transform.childCount; i++) {
                if (Filtres.transform.GetChild(i).gameObject != BoutonPresse) {
                    Filtres.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                }
            }
            List<GameObject> CartesTypes = CartesCorrepondantesType(typeCarte);
            Debug.Log("Ce filtrage a amené " + CartesTypes.Count + " cartes");
            PutCardsInScrollView(ContentAllCards, CartesTypes, 0);
        }
    }

    /// <summary>
    /// Recupérer les cartes qui correspondent à un type précis. 
    /// </summary>
    /// <param name="typeCarte"></param>
    /// <returns></returns>
    private List<GameObject> CartesCorrepondantesType(CarteType.Type typeCarte) {
        List<GameObject> listeCartes = new List<GameObject>();
        foreach (KeyValuePair<string, List<GameObject>> cartesUniques in uniqueCards) {
            if ((cartesUniques.Value[0].GetComponent<Carte>().GetType() == typeof(Entite) && typeCarte == CarteType.Type.ENTITE) ||
                (cartesUniques.Value[0].GetComponent<Carte>().GetType() == typeof(Sort) && typeCarte == CarteType.Type.SORT) ||
                (cartesUniques.Value[0].GetComponent<Carte>().GetType() == typeof(Assistance) && typeCarte == CarteType.Type.ASSISTANCE)) {
                listeCartes.Add(cartesUniques.Value[0]);
            }
        }
        return listeCartes; 
    }

    /// <summary>
    /// Afficher les informations sur un deck lors d'un clique sur le changemennt de deck.
    /// Les informations sont masquées après 10 secondes
    /// </summary>
    /// <param name="deckNumber">Numero du deck de 1 à n</param>
    private void DisplayDeckInfo(int deckNumber) {
        DeckInfoObject.GetComponent<DeckInfo>().setDeckInfo(allDecksGlobal[deckNumber - 1]); 
    }
}
