using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core; 
using System; 

/// <summary>
/// Classe qui sert à récupérer les informations de la base de données.
/// </summary>
public class GetPlayerInfoGameSparks : MonoBehaviour {
	/*
	 * Récupérer toutes les informations reliées au cartes du joueur sur la base de données. 
	 * Attention: Lors de l'implémentation de la méthode de récupération des données pour les achats in-game,
	 * La méthode employée ici: 
	 * Fontion GameSparks
	 * Coroutine pour attendre l'arrivée des données. 
	 * Fonction qui retourne l'objet et qui démarre la coroutine, 
	 * 
	 * ne fonctionne pas, 
	 * 
	 * Il faudra peut-être également implémenter une méthode de type
	 * Fonction GameSparks
	 * Coroutine qui attend l'arrivée des données, 
	 * on démarre la coroutine depuis un autre script et on récupère la variable. 
	 * 
	 * Si des problèmes arrivent ou lors d'une "refonte" du code. 
     * 
     * CETTE METHODE A ETE IMPLEMENTEE
	 * 
	 */ 

    /// <summary>
    /// Permet de stocker un deck du joueur. 
    /// </summary>
	List<GameObject> deck = new List<GameObject> (); 

    /// <summary>
    /// Permet de stocker tous les decks du joueur. 
    /// </summary>
	List<Deck> allDecks = new List<Deck> (); 

    /// <summary>
    /// Variable qui permet de vérifier si les informations sont bien arrivées depuis la base de données.
    /// Tru si c'est la cas
    /// False sinon. 
    /// </summary>
	bool finish = false; 

    /// <summary>
    /// Une carte spécifique récupérée par la base de données. 
    /// </summary>
	GameObject CardoID; 
	public bool cardoIDOk; 

    /// <summary>
    /// Récupérer les cartes du joueur. 
    /// </summary>
    /// <param name="CardToInstantiate">L'objet spécifique sur lequel mettre l'information (carte du manager, de la battle
    /// du choix de la main... )</param>
	public void LoadPlayerCards(GameObject CardToInstantiate){
		/*
		 * Récupérer les cartes du deck du joueur. 
		 */ 

		new GameSparks.Api.Requests.LogEventRequest ()
			.SetEventKey ("getPlayerAllCards")
			.Send ((response) => {

			    if (!response.HasErrors) {
				    Debug.Log ("Received Player Data From GameSparks... "); 
				    // On récupère une liste des cartes du joueur
				    List<GSData> dataList = response.ScriptData.GetGSDataList ("cards"); 

				    for (int i = 0; i < dataList.Count; ++i) {
                        GSData data = dataList[i];
					    GameObject newCarte = Instantiate (CardToInstantiate);
                        DestroyComposants(newCarte); 

                        GSDataToCard(newCarte, data); 
			} 
				finish = true; 	
			} else {
                Debug.Log(response.Errors.JSON); 
				Debug.Log ("Not Received"); 
			}
		}); 
	}

    /// <summary>
    /// Prend en entrée l'information reçu par la base de données sous forme de GS Data
    /// Et attache à un objet un script contenant cette information. 
    /// Puis ajoute la carte à un deck.
    /// </summary>
    /// <param name="newCarte">L'objet sur lequel mettre l'information</param>
    /// <param name="data">L'information</param>
    /// <param name="ID"></param>
    void GSDataToCard(GameObject newCarte, GSData data, string ID="") {
        Debug.Log(newCarte); 
        if (newCarte.GetComponent<CarteType>() != null) {
            newCarte.GetComponent<CarteType>().setTypeFromString(data.GetString("type"));
        } else {
            Debug.Log("Le Composant n'a pas été trouvé");
        }
        if (data.GetString("type") == "entité" || data.GetString("type") == "entite") {
            // Dans le cas d'une carte entité
            Entite _carte = newCarte.AddComponent<Entite>();
            _carte.coutElementaire = data.GetInt("COUT").Value;
            _carte.STAT = data.GetInt("STAT").Value;
            _carte.shortCode = data.GetString("shortCode");
            _carte.Name = data.GetString("name");
            _carte.carteAscendance = stringToAscendance(data.GetString("Element"));
            _carte.carteElement = stringToElement(data.GetString("Element"));
            _carte.stringToEffetList(data.GetString("Effet"));
            _carte.stringToEffetAstral(data.GetString("Astral"));
            _carte.stringToEffetMalefique(data.GetString("Malefique"));
            _carte.AllEffetsAstralStringToDisplay = data.GetString("AstralString");
            _carte.AllEffetsMalefiqueStringToDisplay = data.GetString("MalefiqueString");
            _carte.AllEffetsStringToDisplay = data.GetString("EffetString");
            _carte.AllEffetsString = data.GetString("Effet");
            _carte.AllEffetsAstralString = data.GetString("Astral");
            _carte.AllEffetsMalefiqueString = data.GetString("Malefique"); 
            _carte.CoutAKA = data.GetInt("AKA").Value;

            if (data.GetString("oID") != null) {
                /*
                 * Une fois que la carte a été instanciée sur le réseau, c'est la fonction 
                 * GetCardByIdSparks qui est appelée. 
                 * Elle recherche la carte dans la metaCollection et dans ce cas il n'y a pas de oID.
                 * Mais elle n'est plus nécessaire une fois la carte jouée et instanciée. 
                 */ 
                //Debug.Log(data.GetInt("card_ID").Value);
                _carte.oID = data.GetString("oID");
                _carte.IDAllCards = data.GetInt("card_ID").Value;
            }
            if (ID != "") {
                // L'oID de la carte peut être utile dans le cas où on aurait changé des élements sur la carte 
                // et qu'on voudrait récupérer les infos de base. 
                //Debug.Log("On set l'OID de la carte" + ID);
                _carte.oID = ID;
            }
        } else if (data.GetString("type") == "emanation") {
            // Dans le cas d'une carte entité
            // TODO : On pourrait peut-être regrouper entité et emanation. 
            Emanation _carte = newCarte.AddComponent<Emanation>();
            _carte.coutElementaire = data.GetInt("COUT").Value;
            _carte.STAT = data.GetInt("STAT").Value;
            _carte.shortCode = data.GetString("shortCode");
            _carte.Name = data.GetString("name");
            _carte.carteAscendance = stringToAscendance(data.GetString("Element"));
            _carte.carteElement = stringToElement(data.GetString("Element"));
            _carte.stringToEffetList(data.GetString("Effet"));
            _carte.stringToEffetAstral(data.GetString("Astral"));
            _carte.stringToEffetMalefique(data.GetString("Malefique"));
            _carte.AllEffetsAstralStringToDisplay = data.GetString("AstralString");
            _carte.AllEffetsMalefiqueStringToDisplay = data.GetString("MalefiqueString");
            _carte.AllEffetsStringToDisplay = data.GetString("EffetString");
            _carte.AllEffetsString = data.GetString("Effet");
            _carte.AllEffetsAstralString = data.GetString("Astral");
            _carte.AllEffetsMalefiqueString = data.GetString("Malefique");
            _carte.CoutAKA = data.GetInt("AKA").Value;

            if (data.GetString("oID") != null) {
                /*
                 * Une fois que la carte a été instanciée sur le réseau, c'est la fonction 
                 * GetCardByIdSparks qui est appelée. 
                 * Elle recherche la carte dans la metaCollection et dans ce cas il n'y a pas de oID.
                 * Mais elle n'est plus nécessaire une fois la carte jouée et instanciée. 
                 */
                //Debug.Log(data.GetInt("card_ID").Value);
                _carte.oID = data.GetString("oID");
                _carte.IDAllCards = data.GetInt("card_ID").Value;
            }
            if (ID != "") {
                // L'oID de la carte peut être utile dans le cas où on aurait changé des élements sur la carte 
                // et qu'on voudrait récupérer les infos de base. 
                //Debug.Log("On set l'OID de la carte" + ID);
                _carte.oID = ID;
            }
        } else if (data.GetString("type") == "sort") {
            // Dans le cas d'une carte sort
            Sort _sort = newCarte.AddComponent<Sort>();
            _sort.shortCode = data.GetString("shortCode");
            _sort.Name = data.GetString("name");
            _sort.Niveau = data.GetInt("Niveau").Value;
            //_sort.Effet = data.GetString("Effet");
            //_sort.Condition = data.GetString("Condition");
            _sort.ConditionSort = data.GetString("Condition"); 
            _sort.stringToEffetList(data.GetString("Effet"));
            _sort.AllEffetsString = data.GetString("Effet");
            _sort.AllEffetsStringToDisplay = data.GetString("EffetString"); 

            if (data.GetString("oID") != null) {
                _sort.IDAllCards = data.GetInt("card_ID").Value;
                _sort.oID = data.GetString("oID");
            }
            newCarte.tag = "Sort"; 
        } else if (data.GetString("type") == "assistance") {
            Assistance _assistance = newCarte.AddComponent<Assistance>();
            _assistance.shortCode = data.GetString("shortCode");
            _assistance.Name = data.GetString("name");
            _assistance.STAT = data.GetInt("STAT").Value;
            _assistance.stringToEffetList(data.GetString("Effet"));
            _assistance.AllEffetsString = data.GetString("Effet");
            _assistance.AllEffetsStringToDisplay = data.GetString("EffetString"); 
            if (data.GetString("oID") != null) {
                _assistance.IDAllCards = data.GetInt("card_ID").Value;
                _assistance.oID = data.GetString("oID");
            }
            newCarte.tag = "Assistance";
        }
        else {
            throw new Exception("Type de carte inconnu");
        }
        deck.Add(newCarte);
    }

    /// <summary>
    /// Ajouter une carte random aux cartes du joueur sur la base de données. 
    /// Utile lors de l'ouverture d'un paquet de carte par exemple. 
    /// </summary>
	public void addCardRandom(){
		/*
		 * Ajouter une carte Random
		 */ 
		new GameSparks.Api.Requests.LogEventRequest ()
			.SetEventKey ("addCard")
			.SetEventAttribute("shortCode", "random")
			.Send ((response) => {

				if (!response.HasErrors) {
					Debug.Log("Random Card added"); 
				} 	
				else {
					Debug.Log("Card Not Added"); 
				}
			}); 
	}

    /// <summary>
    /// Attendre la réception des infos. 
    /// </summary>
    /// <param name="CardPrefab">Carte à instancier</param>
    /// <returns>None</returns>
	public IEnumerator WaitForPlayerCards(GameObject CardPrefab){
		Debug.Log ("WaitForPlayerCards"); 
		LoadPlayerCards (CardPrefab); 
		while (!finish) {
			yield return new WaitForSeconds (0.05f); 	
		}
		// on met finish à false pour pouvoir recommencer une autre tâche plus tard. 
		finish = false; 
	}

    /// <summary>
    /// Récupérer toutes les cartes d'un joueur sous la forme d'un deck. 
    /// On ne fait que récupérer l'objet après avoir appelé LoadPlayerDecks
    /// </summary>
    /// <param name="CardPrefab">Prefab de la carte à instancier dans le deck.</param>
    /// <returns>Deck contenant tooutes les cartes du joueur</returns>
	public List<GameObject> GetAllCards(GameObject CardPrefab){
		/*
		 * METHODE A UTILISER.
		 * Récupérer toutes les cartes du joueur
		 */ 
		// StartCoroutine(WaitForPlayerCards (CardPrefab)); 
		return deck; 
	}

    /// <summary>
    /// Récuperer toutes les cartes en tant que deck.
    /// On ne fait que récupérer l'objet après avoir appelé LoadPlayerDecks
    /// </summary>
    /// <param name="CardPrefab"></param>
    /// <returns></returns>
	public Deck GetAllCardsAsDeck(GameObject CardPrefab){
		/*
		 * Récupère toutes les cartes mais renvoie un deck avec le nombre 0 comme attribut. 
         * METHODE A FUSIONNER AVEC GETALLCARDS. 
		 */ 

		finish = false; 
		Deck newDeck = new Deck (deck, 0); 
		return newDeck; 
	}


    /// <summary>
    /// Recupérer tous les decks d'un joueur. 
    /// </summary>
    /// <param name="CardToInstantiate">Prefab de la carte à instancier</param>
    /// <param name="deckNumber">Numéro du deck à récupérer, si égal à 0, on récupère toutes les cartes.</param>
	public void LoadPlayerDecks(GameObject CardToInstantiate, int deckNumber){

		new GameSparks.Api.Requests.LogEventRequest ()
			.SetEventKey ("getDecks")
			.SetEventAttribute ("number", deckNumber)
			.Send ((response) => {

				if (!response.HasErrors) {
					Debug.Log ("Received Player Decks From GameSparks... "); 
					// On récupère une liste des cartes du joueur
					List<GSData> dataList = response.ScriptData.GetGSDataList ("decks"); 
                    
                    // Iteration sur chaque deck. 
					for (int j = 0; j < dataList.Count; j++){
						List<GameObject> CartesDeck = new List<GameObject>(); 
						int numeroDeck = 0; 

						numeroDeck = dataList[j].GetInt("number").Value; 
						List<GSData> Cartes = dataList[j].GetGSDataList("cards"); 

                        // Iteration sur chaque carte du deck. 
						for (int i = 0; i < Cartes.Count; ++i) {
							GSData data = Cartes[i]; 
							GameObject newCarte = Instantiate (CardToInstantiate);
                            DestroyComposants(newCarte); 
                            GSDataToCard(newCarte, data); 

                            // On crée la carte inactive. 
                            newCarte.SetActive(false); 
							CartesDeck.Add (newCarte); 
						} 
						Deck thisDeck = new Deck(CartesDeck, numeroDeck);
						allDecks.Add(thisDeck); 
					}
					finish = true; 	
					} else {
                    Debug.Log(response.Errors.JSON); 
					Debug.Log ("Not Received"); 
				}
			}); 
	}

    /// <summary>
    /// Attendre les decks du joueur. 
    /// Attendre la transmission des données. 
    /// </summary>
    /// <param name="CardPrefab"></param>
    /// <param name="deckNumber"></param>
    /// <returns></returns>
	public IEnumerator WaitForPlayerDecks(GameObject CardPrefab, int deckNumber=0){
		LoadPlayerDecks (CardPrefab, deckNumber); 
		while (!finish) {
			yield return new WaitForSeconds (0.05f); 
		}
		finish = false; 
	}

    /// <summary>
    /// Recuperer tous les decks. 
    /// </summary>
    /// <param name="CardPrefab"></param>
    /// <returns></returns>
	public List<Deck> GetAllDecks(GameObject CardPrefab){
		/*
		 * METHODE A UTILISER.
		 * Récupérer tous les decks du joueur. 
		 */ 
		// StartCoroutine(WaitForPlayerDecks (CardPrefab)); 
		return allDecks; 
	}

    /// <summary>
    /// Recuperer tout un deck. 
    /// </summary>
    /// <param name="CardPrefab">Le prehfab de la carte à utilier pour instantier les objets. INUTILISE PAR LA FONCTION</param>
    /// <param name="number">Le numéro du deck. </param>
    /// <returns></returns>
	public Deck GetDeck(GameObject CardPrefab, int number){
		/*
		 * Pour l'instant on récupère tous les decks mais c'est pas la peine.
		 * Faire attention au -1. (liste commence à ); 
		 */ 
		//StartCoroutine (WaitForPlayerDecks (CardPrefab)); 
		return allDecks [0]; 
	}

    /// <summary>
    /// Recuperer une carte grâce à son ID dans la metaCollection. 
    /// </summary>
    /// <param name="ID">ID de la carte dans la metaCollection</param>
    /// <param name="CardToInstantiate">Prefab de l'objet à instancier</param>
	public void GetCardByIDSparks(string ID, GameObject CardToInstantiate){
		/*
		 * Méthode utile lors du spawn de la carte sur le serveur, 
		 * Comme on ne peut pas passer le gameObject à instantier, 
		 * On passe l'ID de l'object et on récupère la carte dans la metaCollection sur le serveur. 
		 * 
		 * Cette méthode utilise beaucoup de données. 
		 * Attention au nombre de données utilisateurs et au coût du serveur. 
		 * 
		 */ 

		new GameSparks.Api.Requests.LogEventRequest ()
			.SetEventKey ("getCardByoID")
			.SetEventAttribute("ID", ID)
			.Send ((response) => {

				if (!response.HasErrors) {
					GSData card = response.ScriptData.GetGSData("cards");  
					CardoID = Instantiate (CardToInstantiate);
                    DestroyComposants(CardoID); 
                    GSDataToCard(CardoID, card, ID);
					} 	
				else {
					Debug.Log(response.Errors.JSON); 
					Debug.Log("Impossible de récupérer la carte."); 
				}
				finish = true; 
			});
	}

    /// <summary>
    /// Recupérer une carte et attendre que ses infos sur le serveur soient mises à jour. 
    /// </summary>
    /// <param name="ID">ID de la carte dans la base de données gamesparks</param>
    /// <param name="CardToInstantiate">Le prefab de la carte à instantier. </param>
    /// <returns></returns>
	public IEnumerator WaitForCardoID(string ID, GameObject CardToInstantiate){
#if UNITY_EDITOR
        int time = 0; 
#endif
        cardoIDOk = false; 
		GetCardByIDSparks (ID, CardToInstantiate); 
		while (!finish) {
			yield return new WaitForSeconds (0.05f);
#if UNITY_EDITOR
            time++;
#endif
        }
        finish = false;
		cardoIDOk = true;
        Debug.Log("<color=green> LA CARTE EST RECUPEREE ICI </color>");

#if UNITY_EDITOR
        WriteOutputFile write = new WriteOutputFile();
        // write.WriteFileTestFunction("WaitForCardoID", time);
#endif

    }

    /// <summary>
    /// Demarre la coroutine WaitForCardoID. 
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="CardToInstantiate"></param>
	public void StartCardByoID(string ID, GameObject CardToInstantiate){
		/*
		 * Récupérer une carte grâce à son oID unique dans la metaCollection, 
		 * méthode à utiliser.
		 */ 
		StartCoroutine (WaitForCardoID (ID, CardToInstantiate)); 
	}

	public GameObject GetCardoID(){
		return CardoID; 
	}

    /// <summary>
    /// Ajouter ue carte dans un deck. 
    /// </summary>
    /// <param name="IDAllCards">ID de la carte par rapport aux cartes du joueur, dans la RuntimeCollection
    /// La base de données a besoin de cette information pour ne pas ajouter deux fois la même carte dans un même deck. </param>
    /// <param name="deckNumber">Numero du deck du joueur dans lequel il veut ajouter la carte</param>
	public void AddCardToDeck(int IDAllCards, int deckNumber){
		/*
		 * Ajouter une carte à un deck dans la base de données. 
		 * Il faudra avoir vérifié au préalable que la carte ne se trouve pas déjà dans le deck. 
		 */
		new GameSparks.Api.Requests.LogEventRequest ()
			.SetEventKey ("addCardToDeck")
			.SetEventAttribute ("cardID", IDAllCards)
			.SetEventAttribute ("deckID", deckNumber)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("Carte ajoutée"); 
			} else {
				throw new Exception ("La carte n'a pas pu être ajoutée dans le base de données"); 
			}
		}); 
	}

    /// <summary>
    /// Enlever une carte d'un dek. 
    /// </summary>
    /// <param name="IDAllCards">ID de la carte par rapports aux carte du joueur, dans la RuntimeCollection</param>
    /// <param name="deckNumber">Le numéro du deck duquel la carte va être enlevée. </param>
	public void RemoveCardFromDeck(int IDAllCards, int deckNumber){
		/*
		 * Enlever une carte d'un deck.
		 */ 
		new GameSparks.Api.Requests.LogEventRequest ()
			.SetEventKey ("deleteCardFromDeck")
			.SetEventAttribute ("cardID", IDAllCards)
			.SetEventAttribute ("deckID", deckNumber)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("La carte a été enlevée avec succès"); 
			} else {
				Debug.Log(IDAllCards); 
				Debug.Log(deckNumber); 
				Debug.Log(response.Errors.JSON); 
				throw new Exception ("La carte n'a pas pu être enlevée de la base de données"); 
			}
		}); 
	}


	/*  ======================================================================================================
	*
	*											METHODES ANNEXES
	*
	*	======================================================================================================
	*/

    /// <summary>
    /// Transforme un string en un enum Entite.Element. 
    /// </summary>
    /// <param name="ElementString">Le string de la base de données.</param>
    /// <returns>Un Element de Entite.Element</returns>
	public static Entite.Element stringToElement(string ElementString){
		/*
		 * Transformer le string récupéré en un carte.Element. 
		 * 
		 */

		Entite.Element _element = Entite.Element.AUCUN; 
		switch (ElementString) {
		case "FEU":
			_element = Entite.Element.FEU; 
			break; 
		case "AIR":
			_element = Entite.Element.AIR; 
			break; 
		case "TERRE":
			_element = Entite.Element.TERRE;
			break; 
		case "EAU":
			_element = Entite.Element.EAU; 
			break; 
		default:
			// Si la carte est neutre, astrale ou maléfique. 
			_element = Entite.Element.AUCUN; 
			break; 
		}

		return _element; 
	}

	public Entite.Ascendance stringToAscendance(string AscendanceString){
		/*
		 * Transformer le string récupéré en un carte.Ascendance. 
		 * 
		 */
		Entite.Ascendance _ascendance = Entite.Ascendance.NEUTRE; 
		switch (AscendanceString) {
		case "NEUTRE":
			_ascendance = Entite.Ascendance.NEUTRE; 
			break; 
		case "ASTRALE":
			_ascendance = Entite.Ascendance.ASTRALE; 
			break; 
		case "MALEFIQUE":
			_ascendance = Entite.Ascendance.MALEFIQUE; 
			break; 
		default:
			// Si la carte est élémentaire (dans nature, il y aura écrit, FEU, TERRE, AIR ou EAU); 
			_ascendance = Entite.Ascendance.ELEMENTAIRE; 
			break; 
		}

		return _ascendance; 
	}

	public Dictionary<Capacite, int> stringToCapacitesDictionnary(string stringCapacite){
		/*
		 * S'il y a plusieurs capacités, 
		 * le string envoyé par la base de données devra être:
		 * Capcite1, CapciteInt1, Capcite2, CapaciteInt2, etc...
		 * 
		 * Il suffit maintenant de découper et de renvoyer un dictionnaire. 
         * 
         * OBSOLETE, utiliser la classe capacite
		 * 
		 */ 
		string[] AllCapcitesString = stringCapacite.Split (','); 
		Dictionary<Capacite, int> AllCapacite = new Dictionary<Capacite, int> (); 
		if (AllCapcitesString.Length % 2 != 0) {
			// A chaque capacité, il doit y avoir un int associé. 
			throw new Exception ("Les Capacités doivent être de la forme suivante : Capacite1, IntCapacite1, Capacite2, IntCapacite2 ... ");
		} else {
			// Sinon
			for (int i = 0; i < AllCapcitesString.Length / 2; ++i) {
				// On boucle sur toutes les capacités. 


			}
		}

		return AllCapacite; 

	}

    /// <summary>
    /// Detruire les composants inutilisés. 
    /// Le prefab a tous les composants attachés au départ pour pouvoir l'instancier sur le réseau avec tous les composants. 
    /// Ici on rajoute un script donc on détruit tous les composants. 
    /// </summary>
    /// <param name="newCarte"></param>
    public void DestroyComposants(GameObject newCarte) {
        if (newCarte.GetComponent<Entite>() != null) {
            Destroy(newCarte.GetComponent<Entite>());
        }
        if (newCarte.GetComponent<Sort>() != null) {
            Destroy(newCarte.GetComponent<Sort>());
        }
        if (newCarte.GetComponent<Assistance>() != null) {
            Destroy(newCarte.GetComponent<Assistance>());
        }
    }

    /// <summary>
    /// Créer un nouveau deck dans la base de données. 
    /// </summary>
    private void createDeck() {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("createDeck")
            .Send((response) => {
                if (!response.HasErrors) {
                    Debug.Log("Deck créé " + response.ScriptData.GetInt("deck created").Value.ToString()); 
                } else {
                    Debug.Log("Le deck n'a pas pu être créé."); 
                }
                finish = true; 
            }); 
    }

    public IEnumerator createDeckRoutine() {
        finish = false; 
        createDeck();
        while (!finish) {
            yield return new WaitForSeconds(0.1f); 
        }
    }

    public void removeDeck(int deckNumber) {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("removeDeck")
            .SetEventAttribute("deckID", deckNumber)
            .Send((response) => {
                if (!response.HasErrors) {
                    Debug.Log("Deck retiré " + response.ScriptData.GetInt("deck created").Value.ToString());
                }
                else {
                    Debug.Log("Le deck n'a pas pu être retiré.");
                }
                finish = true;
            });
    }


}
