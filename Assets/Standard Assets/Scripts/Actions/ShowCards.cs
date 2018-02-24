using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AntinomiaException; 

public class ShowCards : MonoBehaviourAntinomia {

    private GameObject TextShowCards;

    public GameObject CartePrefab;

    private GameObject FinManuelle; 

	// Use this for initialization
	public override void Start () {
        TextShowCards = GameObject.Find("TextShowCards");
        TextShowCards.SetActive(false);
        FinManuelle = GameObject.Find("ArretManuelShowCards");
        FinManuelle.SetActive(false); 
	}

    /// <summary>
    /// Montrer les cartes à l'autre joueur, sans personnalisation du message
    /// </summary>
    /// <param name="Cards"></param>
    /// <param name="fermetureManuelle"></param>
    /// <param name="playerEnvoi"></param>
    public void ShowCardsToPlayer(string[] Cards, bool fermetureManuelle=false, int playerEnvoi=1) {
        if (playerEnvoi == GameManager.FindLocalPlayerID()) {
            ShowCardsToPlayer(Cards.Length.ToString() + " de vos cartes", Cards, fermetureManuelle);
        } else {
            ShowCardsToPlayer(Cards.Length.ToString() + " cartes de votre adversaire", Cards, fermetureManuelle);
        }
    }

    /// <summary>
    /// Montrer des cartes à un joueur
    /// </summary>
    /// <param name="message"></param>
    /// <param name="Cards"></param>
    public void ShowCardsToPlayer(string message, string[] _AllCardsGiven, bool fermetureManuelle=true) {
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
            newCarte.GetComponent<CarteChooseShow>().shortCode = _AllCardsGiven[i];
            newCarte.GetComponent<CarteChooseShow>().StringToDisplay = GetInfoCarte(_AllCardsGiven[i]); 
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

        if (FinManuelle) {
            FinManuelle.SetActive(true); 
        } else {
            StartCoroutine(FinShowCards());
        }

    }

    /// <summary>
    /// Retourne l'info d'une carte en fonction de son shortCode
    /// </summary>
    /// <param name="shortCode"></param>
    /// <returns></returns>
    public string GetInfoCarte(string shortCode) {
        Carte[] AllCartesType = FindObjectsOfType(typeof(Carte)) as Carte[];
        foreach (Carte c in AllCartesType) {
            if (c.shortCode.Equals(shortCode)) {
                return c.GetInfoCarte(); 
            }
        }

        throw new UnusualBehaviourException("Cette carte devrait être trouvée"); 
    }

    /// <summary>
    /// Lorsqu'on a montré les cartes au joueur, on désactive tout. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator FinShowCards() {
        yield return new WaitForSeconds(3f);
        DesactivateShowCards(); 
    }

    /// <summary>
    /// Tout desactiver
    /// </summary>
    public void DesactivateShowCards() {
        foreach (Transform t in transform) {
            if (t.gameObject.activeSelf) {
                Destroy(t.gameObject);
            }
        }

        TextShowCards.SetActive(false);
        gameObject.SetActive(false);
        FinManuelle.SetActive(false); 
    }
}
