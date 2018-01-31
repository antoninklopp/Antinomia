using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

/// <summary>
/// GameManager lors de la scène du choix du deck.
/// Gère les éléments de UI.
/// 
/// 
/// TODO : Il faudra checker que les decks sont bien conformes avant de pouvoir lancer la game. 
/// En beta, on impose au moins 10 cartes. 
/// </summary>
public class GameManagerChoixDeck : MonoBehaviour {

    /// <summary>
    /// Le prefab du bouton du dekc. 
    /// </summary>
    public GameObject PrefabButtonDeck;

    /// <summary>
    /// Le prefab de la carte du deck.
    /// </summary>
    public GameObject PrefabCardDeck; 

    /// <summary>
    /// Le choix de deck courant du joueur.
    /// </summary>
	private int choixDeck = 0;

    /// <summary>
    /// Tous les decks du joueur.
    /// </summary>
    List<Deck> allDecks;

    List<GameObject> DeckButtons;

    /// <summary>
    /// L'objet qui contient tous les decks.
    /// </summary>
    private GameObject AllDecksButtonsContent;

    /// <summary>
    /// L'objet qui contient toutes les cartes du joueur.
    /// </summary>
    private GameObject AllCardsContent; 

    /// <summary>
    /// Recuperer les decks du joueur. 
    /// </summary>
    private GetPlayerInfoGameSparks playerInfo;

    /// <summary>
    /// Le panel Info du deck
    /// </summary>
    private GameObject panelInfo; 

	// Use this for initialization
	void Start () {
        SetLastChoixDeck();
        AllDecksButtonsContent = GameObject.Find("AllDecks").transform.Find("Viewport").Find("Content").gameObject;
        AllCardsContent = GameObject.Find("AllCards").transform.Find("Viewport").Find("Content").gameObject; 
        playerInfo = GetComponent<GetPlayerInfoGameSparks>();
        panelInfo = GameObject.Find("DeckInfo");

        SetLastChoixDeck(); 

        StartCoroutine(GetDecks());
	}
   
    /// <summary>
    /// Recuperer les decks du joueur
    /// et les afficher.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetDecks() {
        yield return playerInfo.WaitForPlayerDecks(PrefabCardDeck);
        allDecks = playerInfo.GetAllDecks(PrefabCardDeck); 

        // On set les cartes dans la scrollView des cartes; 
        foreach (Deck d in allDecks) {
            foreach (GameObject c in d.Cartes) {
                c.transform.SetParent(AllCardsContent.transform, false);
                c.GetComponent<ImageCarteChoixDeck>().setImage(c.GetComponent<Carte>().shortCode); 
            }
        }

        SetButtonsDecks();
        changeDeckView(choixDeck);
        changeColorDeckButton(choixDeck - 1); 
    }

    /// <summary>
    /// Aller dans le lobby. 
    /// Fonction appelée par le bouton Battle. 
    /// </summary>
	public void Go(){
        if (allDecks[choixDeck - 1].getNombreCartes() < 10) {
            return;
        }


		PlayerPrefs.SetInt ("ChoixDeck", choixDeck); 
		SceneManager.LoadScene ("SimpleLobby");

        PlayerPrefs.SetInt("LastChoixDeck", choixDeck); 
	}

    /// <summary>
    /// Retrouver le dernier choix de deck du joueur. 
    /// </summary>
    private void SetLastChoixDeck() {
        if (!PlayerPrefs.HasKey("LastChoixDeck")) {
            choixDeck = 1; 
        } else {
            choixDeck = PlayerPrefs.GetInt("LastChoixDeck"); 
        }
    }

    /// <summary>
    /// Créer les boutons de decks. 
    /// </summary>
    public void SetButtonsDecks() {
        // on ajoute les nouveaux boutons ensuite. 
        DeckButtons = new List<GameObject>();
        for (int i = 0; i < allDecks.Count; i++) {
            Debug.Log("Deck " + i); 
            GameObject newButton = Instantiate(PrefabButtonDeck);
            newButton.transform.SetParent(AllDecksButtonsContent.transform, false);
            newButton.transform.Find("Text").gameObject.GetComponent<Text>().text = "DECK " + (i + 1).ToString() + " : " + 
                allDecks[i].Nom;

            int number = i;
            // Changement de deck. 
            newButton.GetComponent<Button>().onClick.AddListener(delegate { changeDeckView(number + 1); });
            newButton.GetComponent<Button>().onClick.AddListener(delegate { changeColorDeckButton(number); });

            DeckButtons.Add(newButton);
        }
    }

    /// <summary>
    /// Changer la couleur d'un bouton de deck. 
    /// </summary>
    /// <param name="number"></param>
    public void changeColorDeckButton(int number) {
        for (int i = 0; i < DeckButtons.Count; i++) {
            if (i == number) {
                DeckButtons[i].GetComponent<Image>().color = Color.green;
            }
            else {
                DeckButtons[i].GetComponent<Image>().color = Color.white;
            }
        }
    }

    /// <summary>
    /// Changer le deck que l'on voit
    /// </summary>
    /// <param name="deckNumber"></param>
	public void changeDeckView(int deckNumber) {
        for (int i = 0; i < allDecks.Count; ++i) {
            for (int j = 0; j < allDecks[i].Cartes.Count; ++j) {
                if (i == deckNumber - 1) {
                    allDecks[i].Cartes[j].SetActive(true);
                }
                else {
                    allDecks[i].Cartes[j].SetActive(false);
                }
            }
        }

        Debug.Log("On a changé la view");
        Debug.Log("Numero du deck " + choixDeck); 

        choixDeck = deckNumber;
        panelInfo.SetActive(true); 
        panelInfo.GetComponent<DeckInfo>().setDeckInfo(allDecks[deckNumber - 1]); 
    }
}
