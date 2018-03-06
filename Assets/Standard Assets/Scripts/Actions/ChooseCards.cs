
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.UI; 
using UnityEngine.Networking; 

/// <summary>
/// Montrer des cartes pour que le joueur puisse les choisir. 
/// </summary>
public class ChooseCards : NetworkBehaviour {
	/*
	 * On fait une zoom sur les cartes pour choisir certaines cartes. 
	 * Comme par exemple, les cartes à révéler pour pouvoir invoquer une carte de type AIR en montrant des cartes à son adversaire. 
	 * 
	 */ 

    /// <summary>
    /// Ecart entre les cartes lorsqu'on les montre au joueur. (inutilisé). 
    /// </summary>
	public float ecart = 1.2f; 

    /// <summary>
    /// Prefab de la carte à montrer 
    /// </summary>
	public GameObject CartePrefab; 
    
    /// <summary>
    /// Liste des cartes à montrer au joueur. 
    /// </summary>
	private List<GameObject> AllCardsToShow; 

    /// <summary>
    /// Liste des cartes à retourner à l'objet demandeur
    /// </summary>
	private List<GameObject> AllCardsToReturn = new List<GameObject>();

    private List<GameObject> AllCardsGiven; 

	private List<string> AllCardsToShowOther = new List<string> ();

    // Toutes les cartes à retourner avec les ID en game des cartes
    private List<int> AllCardsToReturnID = new List<int>(); 

    /// <summary>
    /// Référence du bouton de fin du choix
    /// </summary>
	GameObject FiniButton; 

	private SyncListString newListSync = new SyncListString(); 

	private string[] newList;

    // un objet peut demander une carte, après la demande c'est à lui qu'on revoit l'objet sélectionné. 
    GameObject ObjectAsking;

    /// <summary>
    /// true si le joueur a local a un effet en cours. 
    /// </summary>
    private bool effetEnCours;

    /// <summary>
    /// Nombre de cartes à choisir par le joueur.
    /// </summary>
    private int nombreDeCartesAChoisir;

    GameObject DisplayString;

    GameObject TextChooseCards;

	// Use this for initialization
	public void Start () {
		FiniButton = GameObject.Find ("Fini"); 
		FiniButton.SetActive (false);
        TextChooseCards = GameObject.Find("TextChooseCards");
        TextChooseCards.SetActive(false); 
	}

    /// <summary>
    /// Montrer les cartes que le joueur doit pouvoir choisir pour divers effets
    /// </summary>
    /// <param name="_AllCardsGiven">Les objets que le joueur peut choisir</param>
    /// <param name="_ObjectAsking">L'objet demandeur de l'effet</param>
    /// <param name="stringToDisplay">Le string à montrer au joueur pour expliquer le choix qu'il doit faire. </param>
    /// <param name="deactivateAfter">Desactive l'objet directement après dans le cas où on veut d'abord proposer 
    /// au joueur de jouer un effet.</param>
	public void ShowCardsToChoose(List<GameObject> _AllCardsGiven, GameObject _ObjectAsking=null, string stringToDisplay="", 
                                    int _nombreDeCartesAChoisir=1, bool deactivateAfter=false){
        /*
		 * On crée toutes les images à partir de la carte. 
		 */

        DetruireCartesPrevious(); 

        nombreDeCartesAChoisir = _nombreDeCartesAChoisir; 
        effetEnCours = true; 

        ObjectAsking = _ObjectAsking;
		FiniButton.SetActive (true); 
		AllCardsGiven = _AllCardsGiven; 
		AllCardsToShow = new List<GameObject> (); 
		float widthPrefab = CartePrefab.GetComponent<RectTransform> ().sizeDelta.x;
        TextChooseCards.SetActive(true);
        TextChooseCards.GetComponent<Text>().text = stringToDisplay;

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
            newCarte.SetActive(true); 
			// Ensuite on met leur position avec une demi carte entre chaque carte. 
			newCarte.transform.SetParent(transform);
            newCarte.GetComponent<CarteChooseShow>().shortCode = AllShortCodes[i];
            newCarte.GetComponent<CarteChooseShow>().StringToDisplay = AllCardsGiven[i].GetComponent<Carte>().GetInfoCarte(); 
            // newCarte.transform.localPosition = new Vector3 (-AllShortCodes.Count * widthPrefab * ecart / 2f + i * widthPrefab * ecart, 0f, 0f); 
            AllCardsToShow.Add (newCarte); 
		}
		for (int i = 0; i < AllCardsToShow.Count; ++i) {
            // On met l'image sur toutes les cartes. 
            AllCardsToShow[i].SetActive(true); 
			AllCardsToShow[i].SendMessage("setImage", AllShortCodes[i]); 
			AllCardsToShow [i].SendMessage ("setOnIntListener", i);
		}

        // On change la taille du grid Layout.
        // 75 est la taille d'une cellule. 
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((_AllCardsGiven.Count + 1) * 75f, 100f);

        if (deactivateAfter) {
            EmpecherInteraction(); 
        }
	}

    /// <summary>
    /// Permettre au joueur d'interagir. 
    /// </summary>
    public void PermettreInteraction() {
        TextChooseCards.SetActive(true);
        FiniButton.SetActive(true);
    }

    /// <summary>
    /// Empecher l'interaction au joueur
    /// </summary>
    public void EmpecherInteraction() {
        TextChooseCards.SetActive(false);
        FiniButton.SetActive(false);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Ajouter une carte à la liste des cartes à retourner à l'objet demandeur
    /// </summary>
    /// <param name="number">Numéro de la carte</param>
	void AddNewCardToReturn(int number){
        /*
		 * Envoi d'une information de l'image cliquée. 
		 * Le joueur choisit cette carte. 
		 */
        Debug.Log(number + " " + AllCardsGiven.Count); 
		AllCardsToReturn.Add (AllCardsGiven [number]); 
		AllCardsToShowOther.Add (AllCardsGiven[number].GetComponent<Carte>().shortCode);
        AllCardsToReturnID.Add(AllCardsGiven[number].GetComponent<Carte>().IDCardGame);
    }

    /// <summary>
    /// Enlever une carte de la liste de cartes à retourner à l'objet demandeur
    /// </summary>
    /// <param name="number">Numéro de la carte</param>
	void RemoveCardToReturn(int number){
        /*
		 * Envoi d'une information de l'image cliquée
		 * Le joueur ne choisit plus cette carte. 
		 */
        Debug.Log("On remove ici"); 
		GameObject CardToRemove = AllCardsGiven[number]; 
		try{
			AllCardsGiven.Remove(CardToRemove); 
			AllCardsToShowOther.Remove(CardToRemove.GetComponent<Carte>().shortCode);
            AllCardsToReturnID.Remove(CardToRemove.GetComponent<Carte>().IDCardGame); 
		} catch (NullReferenceException e){
            Debug.Log(e); 
			throw new Exception ("La carte n'a pas pu être enlevée"); 
		}
	}

    /// <summary>
    /// Envoie de la liste de cartes à l'objet qui a demandé les cartes
    /// pour un effet. 
    /// </summary>
	public void SendAllCardsChoosen(){
		/*
		 * Lors du clique de fin. 
		 * On envoie toutes les cartes!
		 */ 

        if (nombreDeCartesAChoisir > AllCardsToReturnID.Count) {
            TextChooseCards.GetComponent<Text>().text = "Vous devez encore choisir " +
                (AllCardsToReturnID.Count - nombreDeCartesAChoisir).ToString() + "cartes";
            return; 
        } else if (nombreDeCartesAChoisir < AllCardsToReturnID.Count) {
            TextChooseCards.GetComponent<Text>().text = "Vous avez choisi" +
                (AllCardsToReturnID.Count - nombreDeCartesAChoisir).ToString() + "cartes en trop. Supprimez en."; 
            return;
        }
 
		FiniButton.SetActive (false);
        TextChooseCards.SetActive(false); 
		newList = new string[AllCardsToShowOther.Count];  
		for (int i = 0; i < AllCardsToShowOther.Count; ++i) {
			newList[i] = AllCardsToShowOther [i]; 
		}
		
        if (ObjectAsking == null){
            FindLocalPlayer().GetComponent<Player>().CmdSendCards (newList, "Cartes choisies", GameManager.FindLocalPlayerID());
        } else {
            Debug.Log("On envoie à l'objet qui demande " + AllCardsToReturnID.Count); 
            ObjectAsking.SendMessage("CartesChoisies", AllCardsToReturnID); 
        }
        StartCoroutine(FinShowCards(0f, AllCardsToShow)); 
		AllCardsToShowOther = new List<string> ();
        ObjectAsking = null;
        effetEnCours = false;

        Debug.Log("Cartes envoyées");
	}

    /// <summary>
    /// Montrer les cartes choisies par un joueur lors d'un effet à un autre joueur. 
    /// </summary>
    /// <returns></returns>
	IEnumerator ShowAllCardsChosen(){
		/*
		 * Montrer les cartes choisies par l'autre joueur. 
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
            newCarte.GetComponent<CarteChooseShow>().shortCode = AllShortCodes[i]; 
			// newCarte.transform.localPosition = new Vector3 (-AllShortCodes.Count * widthPrefab * ecart / 2f + i * widthPrefab * ecart, 0f, 0f); 
			AllCardsToShow.Add (newCarte); 
			Debug.Log ("J'ai instancié la carte"); 
		}
		for (int i = 0; i < AllCardsToShow.Count; ++i) {
			// On met l'image sur toutes les cartes. 
			AllCardsToShow[i].SendMessage("setImage", AllShortCodes[i]); 
			AllCardsToShow [i].SendMessage ("setOnIntListener", i);
		}

		Debug.Log ("Cartes OK");
        if (!effetEnCours) {
            // S'il y n'y a pas d'effete en cours, on arête de montrer les cartes. 
            StartCoroutine(FinShowCards(3f, AllCardsToShow));
        } 
	}

    /// <summary>
    /// Arrêter de montrer les cartes au joueur. 
    /// </summary>
    /// <param name="nbSeconds"></param>
    /// <param name="AllCardsToDestroy"></param>
    /// <returns></returns>
	IEnumerator FinShowCards(float nbSeconds, List<GameObject> AllCardsToDestroy){
		/*
		 * Arrêter de montrer les cartes. 
		 */ 
		Debug.Log (AllCardsToDestroy.Count); 
		yield return new WaitForSeconds (nbSeconds); 
		for (int i = 0; i < transform.childCount; ++i) {
			if (FiniButton != transform.GetChild (i).gameObject && transform.GetChild(i).gameObject.activeSelf) {
				Destroy (transform.GetChild (i).gameObject); 
			}
		}
	}

    /// <summary>
    /// Appelée pas le GameManager dans un [ClientRpc(channel=0)] pour montrer les cartes choisies par un joueur à l'autre.
    /// </summary>
    /// <param name="allCards"></param>
    public void RpcShowCardsToOtherPlayer(string[] allCards, string message="") {
        AllCardsToShowOther = new List<string>();
        for (int i = 0; i < allCards.Length; ++i) {
            Debug.Log(allCards[i]);
            AllCardsToShowOther.Add(allCards[i]);
        }
        //AllCardsToShowOther = newListSync; 
        Debug.Log("CARTES QUE J'AI CHOISIES");
        //FinShowCards (0.1f, AllCardsToShow); 
        StartCoroutine(ShowAllCardsChosen());

        TextChooseCards.SetActive(true);
        TextChooseCards.GetComponent<Text>().text = message;
    }

    GameObject FindLocalPlayer() {
        /*
		 * Trouver le joueur local, pour lui faire envoyer les fonctions [Command(channel=0)]
		 */
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (Players[0].GetComponent<Player>().isLocalPlayer) {
            return Players[0];
        }
        else {
            return Players[1];
        }
    }

    /// <summary>
    /// On detruit les cartes présentes précedemment. 
    /// </summary>
    private void DetruireCartesPrevious() {
        foreach (Transform t in transform) {
            if (t.gameObject.activeSelf) {
                Destroy(t.gameObject); 
            }
        }
    }

}

