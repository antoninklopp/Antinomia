using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System; 

/// <summary>
/// Certains éléments et surtout méthodes sont communs/essentiels pour
/// beaucoup d'objets du projet (autant que les méthodes Start et Update de la Network Behaviour), 
/// on crée donc une classe "De base" qui hérite de la classe unity NetworkBehaviour. 
/// </summary>
public class NetworkBehaviourAntinomia : NetworkBehaviour {

	// Use this for initialization
	public virtual void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
	}


    /// <summary>
    /// Récupérer un objet Carte grâce à son IDCardGame. 
    /// </summary>
    /// <param name="_ID_">IDCardGame de la carte recherchée</param>
    /// <returns>la carte si elle a été trouvée, crée une exception sinon. </returns>
    public static GameObject FindCardWithID(int _ID_) {
        /*
		 * Trouver la carte avec la bonne ID. 
         * Doit être la même méthode que dans player (à relier). 
		 */
        CarteType[] AllCartesType = FindObjectsOfType(typeof(CarteType)) as CarteType[];
        List<GameObject> AllCartes = new List<GameObject>();

        for (int i = 0; i < AllCartesType.Length; ++i) {
            GameObject NewCarte = AllCartesType[i].gameObject;
            if (NewCarte.GetComponent<EntiteTemporaire>() != null) {

            } else {
                if (NewCarte.GetComponent<CarteType>().instanciee) {
                    AllCartes.Add(NewCarte);
                }
            }
        }

        for (int i = 0; i < AllCartes.Count; ++i) {
            // On cherche la carte avec le bon ID
            switch (AllCartes[i].GetComponent<CarteType>().thisCarteType) {
                case CarteType.Type.ASSISTANCE:
                    if (AllCartes[i].GetComponent<Assistance>().IDCardGame == _ID_) {
                        return AllCartes[i];
                    }
                    break;
                case CarteType.Type.SORT:
                    if (AllCartes[i].GetComponent<Sort>().IDCardGame == _ID_) {
                        return AllCartes[i];
                    }
                    break;
                case CarteType.Type.ENTITE:
                    if (AllCartes[i].GetComponent<Entite>().IDCardGame == _ID_) {
                        return AllCartes[i];
                    }
                    break;
            }
        }

        return null; 
    }

    /// <summary>
    /// Recuperer l'objet correspondant au joueur local
    /// </summary>
    /// <returns>Joueur local</returns>
    public GameObject FindLocalPlayer() {
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
    /// Trouver un joueur grâce à son playerID. 
    /// </summary>
    /// <param name="PlayerID"></param>
    /// <returns></returns>
    public GameObject FindPlayerWithID(int PlayerID) {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (Players[0].GetComponent<Player>().PlayerID == PlayerID) {
            return Players[0];
        }
        else {
            return Players[1];
        }
    }

    /// <summary>
    /// Recuperer l'ID carte game d'une carte (de n'importe quel type)
    /// </summary>
    /// <param name="Carte">La carte</param>
    /// <returns></returns>
    public int GetObjetIDCardGame(GameObject Carte) {
        if (Carte == null) {
            return -2; 
        }
        if (Carte.GetComponent<Player>() != null) {
            // Utilise lors de l'attaque directe sur un joueur.
            return -1; 
        }
        switch (Carte.GetComponent<CarteType>().thisCarteType) {
            case CarteType.Type.ENTITE:
                return Carte.GetComponent<Entite>().IDCardGame;
            case CarteType.Type.ASSISTANCE:
                return Carte.GetComponent<Assistance>().IDCardGame;
            case CarteType.Type.SORT:
                return Carte.GetComponent<Sort>().IDCardGame;
            default:
                throw new Exception("Ce type de carte n'existe pas");
        }
    }

    /// <summary>
    /// Retourne l'effet recherché d'une carte en fonction de plusieurs paramètres.
    /// </summary>
    /// <param name="Carte">La Carte dont on souhaite récupérer un effet.</param>
    /// <param name="numeroEffet">Le numéro de l'effet dans la liste</param>
    /// <param name="numeroListeEffet">Le numéro de la liste. 0 pour la liste normale, 
    /// 1 pour la liste astrale, 2 pour la liste maléfique</param>
    /// <returns></returns>
    public Effet GetEffetFromCarte(GameObject Carte, int numeroEffet, int numeroListeEffet = 0) {
        Effet effetRetour = new Effet(); 
        switch (Carte.GetComponent<CarteType>().thisCarteType) {
            case CarteType.Type.ENTITE:
                switch (numeroListeEffet) {
                    case 0:
                        return Carte.GetComponent<Entite>().AllEffets[numeroEffet]; 
                    case 1:
                        return Carte.GetComponent<Entite>().AllEffetsAstral[numeroEffet];
                    case 2:
                        return Carte.GetComponent<Entite>().AllEffetsMalefique[numeroEffet];
                    default:
                        throw new Exception("L'entier numeroListeEffet doit être compris entre 0 et 2 inclus"); 
                }
            case CarteType.Type.ASSISTANCE:
                return Carte.GetComponent<Assistance>().AllEffets[numeroEffet];
            case CarteType.Type.SORT:
                return Carte.GetComponent<Sort>().AllEffets[numeroEffet];
            default:
                throw new Exception("Ce type de carte n'existe pas");
        }
    }

    /// <summary>
    /// Récupérer l'objet GameManager
    /// </summary>
    /// <returns></returns>
    protected GameObject getGameManager() {
        return GameObject.FindGameObjectWithTag("GameManager");
    }

    /// <summary>
    /// Recupérer le GameObject du joueur qui n'est pas le joueur local.
    /// </summary>
    /// <returns>L'objet Player qui n'est pas celui du joueur local</returns>
    protected GameObject FindNotLocalPlayer() {
        /*
		 * Trouver le joueur qui n'est pas local, pour lui faire envoyer les fonctions [Command(channel=0)]
		 */
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (!Players[0].GetComponent<Player>().isLocalPlayer) {
            return Players[0];
        }
        else {
            return Players[1];
        }
    }

    /// <summary>
    /// Faire un log dans la console unity et dans la console Antinomia.
    /// </summary>
    /// <param name="message">Message du log (string)</param>
    protected void AntinomiaLog(string message) {
        getGameManager().GetComponent<GameManager>().Log(message); 
    }

    /// <summary>
    /// Faire un log dans la console unity et dans la console Antinomia. 
    /// </summary>
    /// <param name="message">Message du log (int)</param>
    protected void AntinomiaLog(int message) {
        getGameManager().GetComponent<GameManager>().Log(message.ToString());
    }

    /// <summary>
    /// Faire un log dans la console unity et dans la console Antinomia. 
    /// </summary>
    /// <param name="message">Message du log (GameObject)</param>
    protected void AntinomiaLog(GameObject message) {
        try {
            getGameManager().GetComponent<GameManager>().Log(message.ToString());
        } catch (NullReferenceException) {
            Debug.Log("Impossoble de faire le log"); 
        }
    }

    /// <summary>
    /// Récupérer la Pile d'effets
    /// </summary>
    /// <returns>GameObject de la pile d'effets, null si elle n'existe pas. </returns>
    protected GameObject getPile() {
        return GameObject.FindGameObjectWithTag("Pile"); 
    }

    protected List<GameObject> getAllCardsFromPlayerBoardSanctuaire(List<GameObject> CartesAEnlever=null) {

        if (CartesAEnlever == null) {
            CartesAEnlever = new List<GameObject>(); 
        }

        GameObject[] BoardSanctuaire = GameObject.FindGameObjectsWithTag("BoardSanctuaire");
        Debug.Log("Nombre de cartes board sanctuaire" + BoardSanctuaire.Length.ToString()); 
        List<GameObject> Retourner = new List<GameObject>(); 

        for (int i = 0; i < BoardSanctuaire.Length; i++) {
            switch (BoardSanctuaire[i].GetComponent<CarteType>().thisCarteType) {
                case CarteType.Type.ENTITE:
                    if (BoardSanctuaire[i].GetComponent<Entite>().isFromLocalPlayer && CartesAEnlever.IndexOf(BoardSanctuaire[i]) == -1) {
                        Retourner.Add(BoardSanctuaire[i]); 
                    }
                    break;
                case CarteType.Type.SORT:
                    if (BoardSanctuaire[i].GetComponent<Sort>().isFromLocalPlayer && CartesAEnlever.IndexOf(BoardSanctuaire[i]) == -1) {
                        Retourner.Add(BoardSanctuaire[i]);
                    }
                    break;
                case CarteType.Type.ASSISTANCE:
                    if (BoardSanctuaire[i].GetComponent<Assistance>().isFromLocalPlayer && CartesAEnlever.IndexOf(BoardSanctuaire[i]) == -1) {
                        Retourner.Add(BoardSanctuaire[i]);
                    }
                    break;
                default:
                    throw new Exception("Ce type de carte n'est pas implémenté"); 
            }
        }

        Debug.Log("Nombre de cartes retournées" + Retourner.Count); 

        return Retourner; 
    }

    protected List<GameObject> getAllCardsFromPlayerBoardSanctuaire(GameObject CarteAEnlever) {
        List<GameObject> UneCarte = new List<GameObject>() { CarteAEnlever };
        return getAllCardsFromPlayerBoardSanctuaire(UneCarte); 
    }

    /// <summary>
    /// Outil de copie d'un vecteur
    /// </summary>
    /// <param name="vecteur">Le vecteur à recopier</param>
    /// <returns>Un vecteur copié</returns>
    public static Vector3 Copy(Vector3 vecteur) {
        return new Vector3(vecteur.x, vecteur.y, vecteur.z); 
    }

}
