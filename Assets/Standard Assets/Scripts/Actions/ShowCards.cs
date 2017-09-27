using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.UI; 
using UnityEngine.Networking; 

public class ShowCards : NetworkBehaviour {
	/*
	 * On fait une zoom sur les cartes pour choisir certaines cartes. 
	 * Comme par exemple, les cartes à révéler pour pouvoir invoquer une carte de type AIR en montrant des cartes à son adversaire. 
	 * 
	 */ 

	public float ecart = 1.2f; 

	public GameObject CartePrefab; 
	private List<GameObject> AllCardsToShow; 
	private List<GameObject> AllCardsToReturn = new List<GameObject>(); 
	private List<GameObject> AllCardsGiven; 

	private List<string> AllCardsToShowOther = new List<string> ();

    // Toutes les cartes à retourner avec les ID en game des cartes
    private List<int> AllCardsToReturnID = new List<int>(); 

	//private List<string> AllCardsToShowOther; 

	GameObject FiniButton; 
	private SyncListString newListSync = new SyncListString(); 

	//[HideInInspector]
	//[SerializeField]
	private string[] newList;

    // un objet peut demander une carte, après la demande c'est à lui qu'on revoit l'objet sélectionné. 
    GameObject ObjectAsking; 

	// Use this for initialization
	void Start () {
		FiniButton = GameObject.Find ("Fini"); 
		FiniButton.SetActive (false); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowCardsToChoose(List<GameObject> _AllCardsGiven, GameObject _ObjectAsking=null){
        /*
		 * On crée toutes les images à partir de la carte. 
		 * 
		 */
        ObjectAsking = _ObjectAsking;
		FiniButton.SetActive (true); 
		AllCardsGiven = _AllCardsGiven; 
		AllCardsToShow = new List<GameObject> (); 
		float widthPrefab = CartePrefab.GetComponent<RectTransform> ().sizeDelta.x; 

		List<string> AllShortCodes = new List<string> ();
        List<int> AllIDCards = new List<int>(); 
		for (int i = 0; i < _AllCardsGiven.Count; ++i) {
			// On met tous les shortCode dans une nouvelle liste.
			AllShortCodes.Add (_AllCardsGiven [i].GetComponent<Carte> ().shortCode);
            AllIDCards.Add(_AllCardsGiven[i].GetComponent<Carte>().IDCardGame); 
		}
		for (int i = 0; i < AllShortCodes.Count; ++i) {
			// On crée d'abord toutes les cartes
			GameObject newCarte = Instantiate(CartePrefab); 
			// Ensuite on met leur position avec une demi carte entre chaque carte. 
			newCarte.transform.SetParent(transform); 
			newCarte.transform.localPosition = new Vector3 (-AllShortCodes.Count * widthPrefab * ecart / 2f + i * widthPrefab * ecart, 0f, 0f); 
			AllCardsToShow.Add (newCarte); 
		}
		for (int i = 0; i < AllCardsToShow.Count; ++i) {
			// On met l'image sur toutes les cartes. 
			AllCardsToShow[i].SendMessage("setImage", AllShortCodes[i]); 
			AllCardsToShow [i].SendMessage ("setOnIntListener", i);
		}
	}

	void AddNewCardToReturn(int number){
		/*
		 * Envoi d'une information de l'image cliquée. 
		 * Le joueur choisit cette carte. 
		 * 
		 */ 
		AllCardsToReturn.Add (AllCardsGiven [number]); 
		AllCardsToShowOther.Add (AllCardsGiven[number].GetComponent<Carte>().shortCode);
        AllCardsToReturnID.Add(AllCardsGiven[number].GetComponent<Carte>().IDCardGame);
    }

	void RemoveCardToReturn(int number){
		/*
		 * Envoi d'une information de l'image cliquée
		 * Le joueur ne choisit plus cette carte. 
		 */ 
		GameObject CardToRemove = AllCardsGiven[number]; 
		try{
			AllCardsGiven.Remove(CardToRemove); 
			AllCardsToShowOther.Remove(CardToRemove.GetComponent<Carte>().shortCode);
            AllCardsToReturnID.Remove(CardToRemove.GetComponent<Carte>().IDCardGame); 
#pragma warning disable CS0168 // La variable 'e' est déclarée, mais jamais utilisée
		} catch (NullReferenceException e){
#pragma warning restore CS0168 // La variable 'e' est déclarée, mais jamais utilisée
			throw new Exception ("La carte n'a pas pu être enlevée"); 
		}
	}

	public void SendAllCardsChoosen(){
		/*
		 * Lors du clique de fin. 
		 * On envoie toutes les cartes!
		 */ 
		Debug.Log (AllCardsToReturn [0]); 
		FiniButton.SetActive (false);
		newList = new string[AllCardsToShowOther.Count];  
		for (int i = 0; i < AllCardsToShowOther.Count; ++i) {
			newList[i] = AllCardsToShowOther [i]; 
		}
		
        if (ObjectAsking == null){
            FindLocalPlayer().GetComponent<Player>().CmdSendCards (newList);
        } else {
            Debug.Log("Object Asking" + ObjectAsking.GetComponent<Entite>().Name); 
            ObjectAsking.SendMessage("CartesChoisies", AllCardsToReturnID); 
        }
        StartCoroutine(FinShowCards(0f, AllCardsToShow)); 
		AllCardsToShowOther = new List<string> ();

        ObjectAsking = null; 
	}


	IEnumerator ShowAllCardsChosen(){
		/*
		 * Montrer les cartes choisies par l'autre joueur. 
		 * 
		 */ 

		yield return new WaitForSeconds (0.2f); 
		Debug.Log ("Montrer les cartes"); 
		AllCardsToShow = new List<GameObject> ();
		float widthPrefab = CartePrefab.GetComponent<RectTransform> ().sizeDelta.x; 

		List<string> AllShortCodes = new List<string> (); 

		Debug.Log (AllCardsToShowOther [0]); 
		for (int i = 0; i < AllCardsToShowOther.Count; ++i) {
			AllShortCodes.Add (AllCardsToShowOther [i]); 
			Debug.Log (AllCardsToShowOther [i]); 
			Debug.Log ("boucle"); 
		}

		for (int i = 0; i < AllShortCodes.Count; ++i) {
			// On crée d'abord toutes les cartes
			GameObject newCarte = Instantiate(CartePrefab); 
			// Ensuite on met leur position avec une demi carte entre chaque carte. 
			newCarte.transform.SetParent(transform); 
			newCarte.transform.localPosition = new Vector3 (-AllShortCodes.Count * widthPrefab * ecart / 2f + i * widthPrefab * ecart, 0f, 0f); 
			AllCardsToShow.Add (newCarte); 
			Debug.Log ("J'ai instancié la carte"); 
		}
		for (int i = 0; i < AllCardsToShow.Count; ++i) {
			// On met l'image sur toutes les cartes. 
			AllCardsToShow[i].SendMessage("setImage", AllShortCodes[i]); 
			AllCardsToShow [i].SendMessage ("setOnIntListener", i);
		}

		Debug.Log ("Cartes OK"); 
		StartCoroutine (FinShowCards (3f, AllCardsToShow)); 
	}

	IEnumerator FinShowCards(float nbSeconds, List<GameObject> AllCardsToDestroy){
		/*
		 * Arrêter de montrer les cartes. 
		 */ 
		Debug.Log ("je détruis les cartes"); 
		Debug.Log (AllCardsToDestroy.Count); 
		yield return new WaitForSeconds (nbSeconds); 
		for (int i = 0; i < transform.childCount; ++i) {
			if (FiniButton != transform.GetChild (i).gameObject) {
				Destroy (transform.GetChild (i).gameObject); 
			}
		}
	}


	public void RpcShowCardsToOtherPlayer(string[] allCards){
		AllCardsToShowOther = new List<string> (); 
		for (int i = 0; i < allCards.Length; ++i) {
			Debug.Log (allCards [i]); 
			AllCardsToShowOther.Add (allCards [i]);
		}
		//AllCardsToShowOther = newListSync; 
		Debug.Log ("CARTES QUE J4AI CHOISIES"); 
		//FinShowCards (0.1f, AllCardsToShow); 
		StartCoroutine(ShowAllCardsChosen()); 
	}

	GameObject FindLocalPlayer(){
		/*
		 * Trouver le joueur local, pour lui faire envoyer les fonctions [Command]
		 */ 
		GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player"); 
		if (Players [0].GetComponent<Player> ().isLocalPlayer) {
			return Players [0]; 
		} else {
			return Players [1]; 
		}
	}
}

