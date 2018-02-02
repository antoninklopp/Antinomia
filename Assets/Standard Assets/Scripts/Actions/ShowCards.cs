using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ShowCards : MonoBehaviour {

    private GameObject TextShowCards;

    public GameObject CartePrefab; 

	// Use this for initialization
	void Start () {
        TextShowCards = GameObject.Find("TextShowCards");
        TextShowCards.SetActive(false); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowCardsToPlayer(string[] Cards) {
        ShowCardsToPlayer(Cards.Length.ToString() + " de votre adversaire", Cards); 
    }

    /// <summary>
    /// Montrer des cartes à un joueur
    /// </summary>
    /// <param name="message"></param>
    /// <param name="Cards"></param>
    public void ShowCardsToPlayer(string message, string[] _AllCardsGiven) {
        TextShowCards.SetActive(true);
        TextShowCards.GetComponent<Text>().text = message;

        List<GameObject> AllCardsToShow = new List<GameObject>(); 
        List<int> AllIDCards = new List<int>();

        for (int i = 0; i < _AllCardsGiven.Length; ++i) {
            // On crée d'abord toutes les cartes
            GameObject newCarte = Instantiate(CartePrefab);
            newCarte.SetActive(true); 
            // Ensuite on met leur position avec une demi carte entre chaque carte. 
            newCarte.transform.SetParent(transform);
            // newCarte.transform.localPosition = new Vector3 (-AllShortCodes.Count * widthPrefab * ecart / 2f + i * widthPrefab * ecart, 0f, 0f); 
            AllCardsToShow.Add(newCarte);
        }
        for (int i = 0; i < AllCardsToShow.Count; ++i) {
            // On met l'image sur toutes les cartes. 
            AllCardsToShow[i].SendMessage("setImage", _AllCardsGiven[i]);
            if (AllCardsToShow[i].GetComponent<Button>() != null) {
                AllCardsToShow[i].GetComponent<Button>().interactable = false;
            }
        }

        StartCoroutine(FinShowCards()); 

    }

    /// <summary>
    /// Lorsqu'on a montré les cartes au joueur, on désactive tout. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator FinShowCards() {
        yield return new WaitForSeconds(3f); 
        foreach(Transform t in transform) {
            if (t.gameObject.activeSelf) {
                Destroy(t.gameObject); 
            }
        }

        TextShowCards.SetActive(false);
        gameObject.SetActive(false); 
    }
}
