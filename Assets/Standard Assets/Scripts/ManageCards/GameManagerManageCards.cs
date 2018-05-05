
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using Antinomia.GameSparksScripts; 
using Antinomia.Battle;

namespace Antinomia.ManageCards {

    /// <summary>
    /// Game Manager de la scène où l'on peut manager ses cartes. 
    /// Gère les éléments de UI. 
    /// </summary>
    public class GameManagerManageCards : MonoBehaviour {

        /// <summary>
        /// Dictionnaire des cartes uniques
        /// </summary>
        private Dictionary<string, List<GameObject>> uniqueCards;

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
        private float longueurCartesAndroid = 55f;
#pragma warning restore CS0414 // Le champ 'GameManagerManageCards.longueurCartesAndroid' est assigné, mais sa valeur n'est jamais utilisée
        private float longueurCartesOrdinateur = 35f;
        private float longueurAllCards;

        public float rapportLongueurHauteurCarte = 1.5f;

        /// <summary>
        /// Tous les decks du joueur
        /// </summary>
        List<Deck> allDecksGlobal = new List<Deck>();

        /// <summary>
        /// Toutes les cartes du joueur
        /// </summary>
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

        /// <summary>
        /// Nom du deck.
        /// </summary>
        private GameObject DeckName;

        /// <summary>
        /// Boutons de confirmation de changement de nom du deck. 
        /// </summary>
        private GameObject BoutonsConfirmationDeck;

        /// <summary>
        /// Bouton pour faire defiler les deck et pouvoir en changer. 
        /// </summary>
        private GameObject DefilerDeck;

        /// <summary>
        /// Info sur le numero du deck courant. 
        /// Remplace les boutons de decks une fois le deck choisi. 
        /// </summary>
        private GameObject currentDeckInfoNumber;

        public Dictionary<string, List<GameObject>> UniqueCards {
            get {
                return uniqueCards;
            }

            set {
                uniqueCards = value;
            }
        }

        public GameObject ListeFiltreElementaires;

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

            DeckName = GameObject.Find("DeckName");
            BoutonsConfirmationDeck = GameObject.Find("BoutonsConfirmationDeck");
            BoutonsConfirmationDeck.SetActive(false);

            DefilerDeck = GameObject.Find("DefilerDeck");
            currentDeckInfoNumber = GameObject.Find("CurrentDeckNumber");
            ListeFiltreElementaires = GameObject.Find("FiltresElementaires");
            ListeFiltreElementaires.SetActive(false);

            longueurAllCards = ContentAllCards.transform.parent.parent.gameObject.GetComponent<RectTransform>().sizeDelta.x;

            // On change la taille de la card prefab

#if (UNITY_ANDROID)
        CardPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(longueurCartesAndroid,
            CardPrefab.GetComponent<RectTransform>().sizeDelta.y / CardPrefab.GetComponent<RectTransform>().sizeDelta.x * longueurCartesAndroid);
#else
            CardPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(longueurCartesOrdinateur,
                CardPrefab.GetComponent<RectTransform>().sizeDelta.y / CardPrefab.GetComponent<RectTransform>().sizeDelta.x * longueurCartesOrdinateur);
#endif

            StartCoroutine(setAllDecks(1));
        }

        /// <summary>
        /// Ajouter toutes les cartes dans le cadre des cartes
        /// </summary>
        /// <returns></returns>
        IEnumerator setAllCards() {

            yield return playerInfo.WaitForPlayerCards(CardPrefab);
            List<GameObject> AllCards = playerInfo.GetAllCards(CardPrefab);
            listAllCards = AllCards;
            // On attent l'arrivée des données. 
            // yield return new WaitForSeconds (1f); 

            UniqueCards = getAllUniqueCards(AllCards);

            ReorganiseCardsInScrollView(ContentAllCards, UniqueCards, 0f, initialize: true);

        }

        /// <summary>
        /// Ajouter tous les decks dans le cadre des decks
        /// </summary>
        /// <param name="deckNumber"></param>
        /// <returns></returns>
        IEnumerator setAllDecks(int deckNumber) {
            /*
             * Inventaire des decks, situé en bas. 
             */

            // On update le currentDeckNumber
            currentDeckNumber = deckNumber;

            Debug.Log("DECKS");
            yield return playerInfo.WaitForPlayerDecks(CardPrefab);
            List<Deck> allDecks = playerInfo.GetAllDecks(CardPrefab);
            // On récupère le nombre de decks. 
            this.deckNumberTotal = allDecks.Count;
            SetButtonsDecks();
            allDecksGlobal = allDecks;
            // Debug.Log (allDecks.Count.ToString() + "               fhzihfuzehfuozhfoeznfojzbùefhzbe"); 
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < allDecksGlobal.Count; ++i) {
                for (int j = 0; j < allDecksGlobal[i].Cartes.Count; ++j) {
                    if (i == deckNumber - 1) {
                        allDecksGlobal[i].Cartes[j].SetActive(true);
                    }
                    else {
                        allDecksGlobal[i].Cartes[j].SetActive(false);
                    }
                }
            }

            Debug.Log("nombre de deck" + allDecks.Count);
            Debug.Log("deck demande " + deckNumber);
            ReorganiseCardsInScrollView(ContentAllDecks, allDecks[deckNumber - 1].Cartes, 225f);

            AfficherNumeroDeck(currentDeckNumber);

            StartCoroutine(setAllCards());
        }

        public void AfficherNumeroDeck(int numeroDeck) {
            DecksButton.SetActive(false);
            currentDeckInfoNumber.SetActive(true);
            currentDeckInfoNumber.transform.Find("Text").gameObject.GetComponent<Text>().text = "DECK " + numeroDeck.ToString();
        }

        /// <summary>
        /// Changer le deck que l'on voit
        /// </summary>
        /// <param name="deckNumber"></param>
        public void changeDeckView(int deckNumber) {
            Debug.Log("DECK NUMBER" + (deckNumber - 1).ToString());
            for (int i = 0; i < allDecksGlobal.Count; ++i) {
                for (int j = 0; j < allDecksGlobal[i].Cartes.Count; ++j) {
                    if (i == deckNumber - 1) {
                        allDecksGlobal[i].Cartes[j].SetActive(true);
                    }
                    else {
                        allDecksGlobal[i].Cartes[j].SetActive(false);
                    }
                }
            }

            currentDeckNumber = deckNumber;
            // On montre aussi le nom du nouveau deck.
            Debug.Log("Le nom de ce deck " + allDecksGlobal[deckNumber - 1].Nom);
            if (allDecksGlobal[deckNumber - 1].Nom != null) {
                DeckName.GetComponent<InputField>().text = allDecksGlobal[deckNumber - 1].Nom;
            }
            else {
                DeckName.GetComponent<InputField>().text = "";
            }
            BoutonsConfirmationDeck.SetActive(false);
            ReorganiseCardsInScrollView(ContentAllDecks, allDecksGlobal[deckNumber - 1].Cartes, 225f);
            // On update aussi toutes les cartes en fonction du nombre qui ont été enlevées dans le deck qu'on est 
            // en train de regarder. 
            ReorganiseCardsInScrollView(ContentAllCards, UniqueCards, 0, false);

            DisplayDeckInfo(deckNumber);
            // Une fois que le joueur a choisi son nouveau deck on enlève de sa vue. 
            FaireDefilerDeck(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScrollView"></param>
        /// <param name="Cards"></param>
        /// <param name="yStartPosition"></param>
        public void ReorganiseCardsInScrollView(GameObject ScrollView, List<GameObject> Cards, float yStartPosition,
                bool unique = false) {

            PutCardsInScrollView(ScrollView, Cards, yStartPosition);
            CardPrefab.SetActive(false);

            if (!unique) {
                foreach (GameObject Carte in Cards) {
                    // On désactive le numéro
                    Carte.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }

        public void ReorganizeCardsInAllCardsUnique() {
            ReorganiseCardsInScrollView(ContentAllCards, UniqueCards, 0, true);
        }

        /// <summary>
        /// Mettre la liste de cartes dans la scrollView
        /// </summary>
        /// <param name="ScrollView"></param>
        /// <param name="Cards">Liste de cartes à insérer</param>
        /// <param name="yStartPosition"></param>
        /// <param name="reorganize">Si on doit réorganiser la scollView</param>
        private void PutCardsInScrollView(GameObject ScrollView, List<GameObject> Cards, float yStartPosition, bool reorganize = false) {
            longueurAllCards = ScrollView.transform.parent.parent.gameObject.GetComponent<RectTransform>().sizeDelta.x;
            Debug.Log("On met les cartes dans la scrollView " + ScrollView.name);

            // On desactive les cartes deja présentes dans le scrollView
            foreach (Transform t in ScrollView.transform) {
                t.gameObject.SetActive(false);
            }

            // On change le parent des cartes
            for (int i = 0; i < Cards.Count; ++i) {
                Cards[i].SetActive(true);
                Cards[i].transform.SetParent(ScrollView.transform, false);
            }

            // Si les cartes sont déjà dans la scrollview, exemple : tri par puissance, tri par AKA
            // On change tous les siblings index. 
            if (reorganize) {
                for (int i = 0; i < Cards.Count; i++) {
                    Cards[i].transform.SetSiblingIndex(i);
                }
            }

            int nombreCartesParLigne;
#if (UNITY_ANDROID)
		nombreCartesParLigne = (int)(longueurAllCards/longueurCartesAndroid);
                ScrollView.GetComponent<GridLayoutGroup>().cellSize = new Vector2(longueurCartesAndroid, longueurCartesAndroid * rapportLongueurHauteurCarte);

        // On change la taille de l'objet qui accueille les cartes. 
        ScrollView.transform.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
            ScrollView.transform.gameObject.GetComponent<RectTransform>().sizeDelta.x, longueurCartesAndroid *
            rapportLongueurHauteurCarte * Cards.Count / nombreCartesParLigne + 200f); 

#else
            if (Cards.Count == 0) {
                nombreCartesParLigne = (int)(longueurAllCards / longueurCartesOrdinateur);
                //scale = Cards[0].GetComponent<RectTransform>().transform.localScale.x; 
            }
            else {
                nombreCartesParLigne = (int)(longueurAllCards / (longueurCartesOrdinateur));
            }
            ScrollView.GetComponent<GridLayoutGroup>().cellSize = new Vector2(longueurCartesOrdinateur, longueurCartesOrdinateur * rapportLongueurHauteurCarte);
            // On change la taille de l'objet qui accueille les cartes. 
            ScrollView.transform.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                ScrollView.transform.gameObject.GetComponent<RectTransform>().sizeDelta.x, longueurCartesOrdinateur *
                rapportLongueurHauteurCarte * Cards.Count / nombreCartesParLigne + 200f); // 100f rajouté artificiellement
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
            bool initialize = false) {

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


        public void ReorganiseDeckCards(int deckNumber) {
            ReorganiseCardsInScrollView(ContentAllDecks, allDecksGlobal[deckNumber - 1].Cartes, 225f);
            Debug.Log("On reorganise");
        }

        /// <summary>
        /// Call lorsqu'un objet va être drag and drop
        /// </summary>
        /// <param name="draggedObject">Objet drag</param>
        /// <param name="indexSibling">index de draggedObject dans la hiérarchie.</param>
        public void ObjectBeingDragged(GameObject draggedObject, int indexSibling) {
            /*
             * Lorsqu'un objet va être drag and drop,
             * on en crée un autre qui va prendre sa place 
             * et celui qui est drag va devenir un objet temporaire. 
             * 
             */

            for (int i = 0; i < listAllCards.Count; ++i) {
                if (draggedObject == listAllCards[i]) {
                    GameObject newDraggedObject = Instantiate(draggedObject);
                    newDraggedObject.transform.SetParent(ContentAllCards.transform);
                    newDraggedObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    newDraggedObject.GetComponent<RectTransform>().localScale = draggedObject.GetComponent<RectTransform>().localScale;
                    listAllCards.Insert(i, newDraggedObject);
                    listAllCards.Remove(draggedObject);

                    UniqueCards[draggedObject.GetComponent<Carte>().Name].Remove(draggedObject);
                    UniqueCards[newDraggedObject.GetComponent<Carte>().Name].Insert(0, newDraggedObject);

                    newDraggedObject.transform.SetParent(ContentAllCards.transform, false);
                    newDraggedObject.transform.SetSiblingIndex(indexSibling);
                    break;
                }
            }
        }

        /// <summary>
        /// Quand une carte est drag pour être mise dans un scrollView de deck, 
        /// on en instantie une autre pour la remplacer, 
        /// à la fin deu drag on remet la carte d'origine. 
        /// </summary>
        /// <param name="Card"></param>
        private void ReplaceCardScrollViewAllCards(GameObject Card) {
            GameObject CarteTemporaire = null;
            bool carteTrouvee = false;
            foreach (Transform t in ContentAllCards.transform) {
                try {
                    if (t.gameObject.activeInHierarchy) {
                        if (t.gameObject.GetComponent<Carte>().Name == Card.GetComponent<Carte>().Name) {
                            CarteTemporaire = t.gameObject;
                            carteTrouvee = true;
                            break;
                        }
                    }
                }
                catch (NullReferenceException e) {
                    Debug.Log(e);
                }
            }
            Debug.Log("Carte trouvée " + carteTrouvee);
            int index = CarteTemporaire.transform.GetSiblingIndex();
            Debug.Log("sibling index" + index.ToString());
            Destroy(CarteTemporaire);
            Card.transform.SetParent(ContentAllCards.transform, false);
            Card.transform.SetSiblingIndex(index);
        }

        /// <summary>
        /// Ajouter une carte à un deck. 
        /// La liste des decks commence à être numérotée à 0. 
        /// </summary>
        /// <param name="NouvelleCarte"></param>
        /// <param name="deckNumber"></param>
        public void AjoutCarte(GameObject NouvelleCarte, int deckNumber) {
            Debug.Log("APPPEL DE LA FONCTION");
            // on recherche une des cartes qui n'est pas déjà dans le deck 
            GameObject CarteAAjouterAuDeck = null;
            foreach (GameObject g in UniqueCards[NouvelleCarte.GetComponent<Carte>().Name]) {
                // Si on en trouve une, on l'ajoute
                if (CheckIfCardNotInDeck(deckNumber, g)) {
                    // Ajout dans l'objet Deck
                    CarteAAjouterAuDeck = g;
                    // allDecksGlobal[deckNumber - 1].AjouterCarte(g); La carte est ajoutée 5 lignes en dessous. 
                    // Ajout dans la base de données. 
                    playerInfo.AddCardToDeck(GetIDAllCards(g), deckNumber);
                    break;
                }
            }
            Destroy(NouvelleCarte);
            if (CarteAAjouterAuDeck != null) {
                GameObject Carte = Instantiate(CarteAAjouterAuDeck);
                Carte.transform.SetParent(ContentAllDecks.transform, false);
                allDecksGlobal[currentDeckNumber - 1].AjouterCarte(Carte);
            }
            // ReplaceCardScrollViewAllCards(NouvelleCarte);
            ReorganiseCardsInScrollView(ContentAllCards, UniqueCards, 0, true);
            ReorganiseCardsInScrollView(ContentAllDecks, allDecksGlobal[currentDeckNumber - 1].Cartes, 0);
            Debug.Log("Carte Ajoutee au deck " + deckNumber.ToString());
        }

        /// <summary>
        /// Vérifier si la carte n'est pas déjà dans le deck.
        /// </summary>
        /// <param name="deckNumber"></param>
        /// <param name="Card"></param>
        /// <returns>False si la carte est déjà dans le deck, 
        /// True sinon. </returns>
        bool CheckIfCardNotInDeck(int deckNumber, GameObject Card) {
            Deck _deck = allDecksGlobal[deckNumber - 1];
            for (int i = 0; i < _deck.Cartes.Count; ++i) {
                if (GetIDAllCards(Card) == GetIDAllCards(_deck.Cartes[i])) {
                    //La carte est déjà dans le deck
                    if (GameObject.Find("Info") != null) {
                        GameObject.Find("Info").GetComponent<Text>().text = "La carte est déjà présente dans le deck";
                    }
                    return false;
                }
            }

            return true;
        }

        public void EnleverCarte(GameObject AncienneCarte, int deckNumber) {
            /*
             * Enlever une carte d'un deck. 
             * la liste des decks commence à être numérotée à 0. 
             */
            allDecksGlobal[deckNumber - 1].EnleverCarte(AncienneCarte);
            playerInfo.RemoveCardFromDeck(GetIDAllCards(AncienneCarte), deckNumber);
            Debug.Log("NP");

            Destroy(AncienneCarte);

            ReorganiseCardsInScrollView(ContentAllCards, UniqueCards, 0, true);
            ReorganiseCardsInScrollView(ContentAllDecks, allDecksGlobal[currentDeckNumber - 1].Cartes, 0);
        }

        /// <summary>
        /// Retourner au menu. 
        /// </summary>
        public void GoBackToMenu() {
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Recupérer l'ID all cards de la carte
        /// </summary>
        /// <param name="_carte"></param>
        /// <returns></returns>
        public int GetIDAllCards(GameObject _carte) {
            return _carte.GetComponent<Carte>().IDAllCards;
        }

        /// <summary>
        /// Créer les boutons de decks. 
        /// </summary>
        public void SetButtonsDecks() {
            // On enleve tous les boutons actifs dans la hierarchie avant : 
            int count = 0;
            foreach (Transform t in DecksButton.transform.Find("Viewport").Find("Content")) {
                Destroy(t.gameObject);
                count++;
            }
            Debug.Log("On a detruit " + count + " boutons");
            // on ajoute les nouveaux boutons ensuite. 
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

        /// <summary>
        /// Changer la couleur d'un bouton de deck. 
        /// </summary>
        /// <param name="number"></param>
        public void changeColorDeckButton(int number) {
            for (int i = 0; i < allDecksButtonsList.Count; i++) {
                if (i == number) {
                    allDecksButtonsList[i].GetComponent<Image>().color = Color.green;
                }
                else {
                    allDecksButtonsList[i].GetComponent<Image>().color = Color.white;
                }
            }
        }

        /// <summary>
        /// Ajouter un deck au joueur
        /// </summary>
        public void AjouterDeck() {
            // On crée un deck 
            StartCoroutine(AjouterDeckRoutine());
        }

        private IEnumerator AjouterDeckRoutine() {
            yield return playerInfo.createDeckRoutine();

            // On update l'UI, en laissant le deck courant. 
            StartCoroutine(setAllDecks(currentDeckNumber));
        }

        /// <summary>
        /// Enlever un deck au joueur
        /// </summary>
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
                StartCoroutine(RemoveDeckOKRoutine());
            }
            SupprimerDeck.SetActive(false);
        }

        private IEnumerator RemoveDeckOKRoutine() {
            yield return playerInfo.removeDeckRoutine(currentDeckNumber);
            // On reset la vie du deck.
            yield return setAllDecks(1);
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
                }
                else {
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
                Carte.GetComponent<CardCardManager>().MakeColored();
            }
            else {
                Carte.GetComponent<CardCardManager>().MakeBlackAndWhite();
            }
        }

        /// <summary>
        /// Recupérer le nombre de fois ou une instance de la carte est dans le deck
        /// </summary>
        /// <param name="Carte"></param>
        /// <param name="deckNumber"></param>
        /// <returns></returns>
        public int getNombreOfCartesInDeck(GameObject Carte, int deckNumber) {
            return getNombreOfCartesinDeck(Carte.GetComponent<Carte>().Name, deckNumber);
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
        /// Afficher les informations sur un deck lors d'un clique sur le changemennt de deck.
        /// Les informations sont masquées après 10 secondes
        /// </summary>
        /// <param name="deckNumber">Numero du deck de 1 à n</param>
        private void DisplayDeckInfo(int deckNumber) {
            DeckInfoObject.SetActive(true);
            DeckInfoObject.GetComponent<DeckInfo>().setDeckInfo(allDecksGlobal[deckNumber - 1]);
        }

        /// <summary>
        /// Enlever toutes les cartes d'un deck. 
        /// </summary>
        /// <param name="deckNumber"></param>
        public void clearDeck() {
            foreach (GameObject g in allDecksGlobal[currentDeckNumber - 1].Cartes) {
                Destroy(g);
            }
            allDecksGlobal[currentDeckNumber - 1].ResetDeck();
            playerInfo.clearDeck(currentDeckNumber);
        }

        /// <summary>
        /// Lorsque le jouer veut changer le nom du deck. 
        /// </summary>
        public void ChangementNomDeck() {
            if (!BoutonsConfirmationDeck.activeInHierarchy) {
                BoutonsConfirmationDeck.SetActive(true);
            }
        }

        /// <summary>
        /// Dans le cas d'un changement de deck. 
        /// Pour que le changement soit effectif, il faut confirmer le changement. 
        /// Grâce à un des deux boutons qui apparaissent à un chanegement de lettre. 
        /// </summary>
        /// <param name="changer"></param>
        public void ChangerNomDeck(bool changer) {
            // Dans le cas d'une confirmation
            if (changer) {
                playerInfo.changerNomDeck(currentDeckNumber, DeckName.GetComponent<InputField>().text);
                Debug.Log("le nouveau nom du deck" + DeckName.GetComponent<InputField>().text);
                allDecksGlobal[currentDeckNumber - 1].Nom = DeckName.GetComponent<InputField>().text;
            }
            // Dans le cas où le joueur ne veut pas poursuivre le changement de nom
            else {
                DeckName.GetComponent<InputField>().text = allDecksGlobal[currentDeckNumber - 1].Nom;
            }
            BoutonsConfirmationDeck.SetActive(false);
        }

        /// <summary>
        /// Montrer les decks possibles au joueur. 
        /// </summary>
        public void FaireDefilerDeck(bool defiler = false) {
            // Dans le cas où le joueur voit déjà les decks
            if (DefilerDeck.transform.Find("Image").gameObject.GetComponent<Image>().color == Color.red) {
                AfficherNumeroDeck(currentDeckNumber);
                float deckButtonHeight = DecksButton.transform.Find("Viewport").Find("Content").gameObject.
                    GetComponent<GridLayoutGroup>().cellSize.y;
                DecksButton.GetComponent<RectTransform>().sizeDelta = new Vector2(DecksButton.GetComponent<RectTransform>().sizeDelta.x
                    , deckButtonHeight);
                DefilerDeck.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
            }
            // Dans le cas où on ne voit pas les decks. 
            else {
                currentDeckInfoNumber.SetActive(false);
                DecksButton.SetActive(true);
                int nombreDeDecks = allDecksGlobal.Count;
                float deckButtonHeight = DecksButton.transform.Find("Viewport").Find("Content").gameObject.
                    GetComponent<GridLayoutGroup>().cellSize.y;
                DecksButton.GetComponent<RectTransform>().sizeDelta = new Vector2(DecksButton.GetComponent<RectTransform>().sizeDelta.x
                    , nombreDeDecks * deckButtonHeight + 10);
                DefilerDeck.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.red;
            }
        }

        public void PutCardsInAllCards(List<GameObject> Cartes, bool reorganize) {
            PutCardsInScrollView(ContentAllCards, Cartes, 0, reorganize);
        }

        public void PutCardsInAllDecks(List<GameObject> Cartes, bool reorganize) {
            PutCardsInScrollView(ContentAllDecks, Cartes, 0, reorganize);
        }
    }

}
