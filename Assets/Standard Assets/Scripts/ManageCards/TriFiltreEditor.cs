
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI; 

/// <summary>
/// Tri et Filtre des cartes du CardManager. 
/// </summary>
public class TriFiltreEditor : MonoBehaviour {

    public enum Tri {
        AKA, 
        PUISSANCE, 
        NOM, 
        NONE
    }

    private Tri tri; 

    /// <summary>
    /// Aucun si aucun filtre de type de carte n'est activé
    /// Sinon le nom du filtre
    /// </summary>
    private CarteType.Type filtreType = CarteType.Type.AUCUN;

    /// <summary>
    /// None si aucun filtre élémentaire n'est activé. 
    /// Sinon le nom du filtre
    /// Si != None, le filtreType doit être égal à élémentaire
    /// </summary>
    private Entite.Element filtreElementaire = Entite.Element.NONE;

    /// <summary>
    /// Aucun si aucun filte d'ascendance est activé. 
    /// Sinon le nom du filtre
    /// Si != None, le filtre de type doit être égal à élémentaire. 
    /// </summary>
    private Entite.Ascendance filtreAscendance = Entite.Ascendance.NONE;

    public CarteType.Type FiltreType {
        get {
            return filtreType;
        }

        set {
            filtreType = value;
        }
    }

    /// <summary>
    /// Liste des filtres élémentaires 
    /// </summary>
    private GameObject ListeFiltreElementaires;

    private GameObject BoutonsTris; 

    public void Start() {
        ListeFiltreElementaires = GameObject.Find("FiltresElementaires");
        if (ListeFiltreElementaires == null) {
            ListeFiltreElementaires = GetComponent<GameManagerManageCards>().ListeFiltreElementaires;
        }
        BoutonsTris = GameObject.Find("ClasserCriteres");
    }

    /// <summary>
    /// Sous - tri de <see cref="TriFiltreEditor.Trier(string)"/>
    /// </summary>
    /// <param name="cartes">Cartes uniques non triées</param>
    public void TrierAKA(List<GameObject> Cartes) {
        for (int i = 0; i < Cartes.Count; i++) {
            for (int j = 0; j < i; j++) {
                if ((Cartes[j].GetComponent<Carte>().GetType() == typeof(Sort) ||
                    Cartes[j].GetComponent<Carte>().GetType() == typeof(Assistance))
                    && Cartes[i].GetComponent<Carte>().GetType() == typeof(Entite)) {
                    GameObject Carte = Cartes[i];
                    Cartes[i] = Cartes[j];
                    Cartes[j] = Carte;
                }
                else if ((Cartes[j].GetComponent<Carte>().GetType() == typeof(Entite) &&
                    Cartes[i].GetComponent<Carte>().GetType() == typeof(Entite)) &&
                    Cartes[j].GetComponent<Entite>().CoutAKA > Cartes[i].GetComponent<Entite>().CoutAKA) {
                    GameObject Carte = Cartes[i];
                    Cartes[i] = Cartes[j];
                    Cartes[j] = Carte;
                }
            }
        }
        tri = Tri.NOM; 
        Debug.Log("La liste a été triée par AKA");
        PutCardsInScrollView("allCards", Cartes, true);
    }

    /// <summary>
    /// Sous - tri de <see cref="TriFiltreEditor.Trier(string)"/>
    /// </summary>
    /// <param name="cartes">Cartes uniques non triées. </param>
    public void TrierPuissance(List<GameObject> Cartes) {
        for (int i = 0; i < Cartes.Count; i++) {
            for (int j = 0; j < i; j++) {
                if ((Cartes[j].GetComponent<Carte>().GetType() == typeof(Sort) ||
                    Cartes[j].GetComponent<Carte>().GetType() == typeof(Assistance))
                    && Cartes[i].GetComponent<Carte>().GetType() == typeof(Entite)) {
                    GameObject Carte = Cartes[i];
                    Cartes[i] = Cartes[j];
                    Cartes[j] = Carte;
                }
                else if ((Cartes[j].GetComponent<Carte>().GetType() == typeof(Entite) &&
                    Cartes[i].GetComponent<Carte>().GetType() == typeof(Entite)) &&
                  Cartes[j].GetComponent<Entite>().getPuissance() > Cartes[i].GetComponent<Entite>().getPuissance()) {
                    GameObject Carte = Cartes[i];
                    Cartes[i] = Cartes[j];
                    Cartes[j] = Carte;
                }
            }
        }
        tri = Tri.PUISSANCE; 
        // On doit mettre les autres tris à false
        Debug.Log("La liste a été triée par Puissance");
        PutCardsInScrollView("allCards", Cartes, true);
    }

    /// <summary>
    /// Sous - tri de <see cref="TriFiltreEditor.Trier(string)"/>
    /// </summary>
    /// <param name="cartes">Cartes uniques non triées. </param>
    public void TrierNom(List<GameObject> Cartes) {
        for (int i = 0; i < Cartes.Count; i++) {
            for (int j = 0; j < i; j++) {
                if (String.Compare(Cartes[j].GetComponent<Carte>().Name, Cartes[i].GetComponent<Carte>().Name) > 0) {
                    GameObject Carte = Cartes[i];
                    Cartes[i] = Cartes[j];
                    Cartes[j] = Carte;
                }
            }
        }
        tri = Tri.NOM;
        // On doit mettre les autres tris à false
        Debug.Log("La liste a été triée par nom");
        PutCardsInScrollView("allCards", Cartes, true);
    }


    public void PutCardsInScrollView(string scrollView, List<GameObject> Cartes, bool reorganize) {
        if (scrollView == "allDecks") {
            GetComponent<GameManagerManageCards>().PutCardsInAllDecks(Cartes, reorganize); 
        } else if (scrollView == "allCards") {
            GetComponent<GameManagerManageCards>().PutCardsInAllCards(Cartes, reorganize);
        } else {
            throw new Exception("Ce content n'existe pas"); 
        }
    }

    /// <summary>
    /// Filtrer les cartes selon un type d'entité. 
    /// Methode appelé par un bouton de sélection. 
    /// </summary>
    /// <param name="type"></param>
    public void FiltreParType(string type) {
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

        FiltreType = typeCarte;

        if (BoutonPresse.GetComponent<Image>().color == Color.green) {
            // Le filtre est actif
            // on le desactive
            BoutonPresse.GetComponent<Image>().color = Color.white;
            GetComponent<GameManagerManageCards>().ReorganizeCardsInAllCardsUnique();
            FiltreType = CarteType.Type.AUCUN;
        }
        else {
            // Le filtre n'est pas actif. 
            BoutonPresse.GetComponent<Image>().color = Color.green;
            // On desactive les couleurs d'autres filtres si ils étaient actifs
            GameObject Filtres = GameObject.Find("FiltresCartes");
            for (int i = 0; i < Filtres.transform.childCount; i++) {
                if (Filtres.transform.GetChild(i).gameObject != BoutonPresse) {
                    Filtres.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                }
            }
            List<GameObject> CartesTypes = CartesCorrespondantesType(typeCarte);
            Debug.Log("Ce filtrage a amené " + CartesTypes.Count + " cartes");
            PutCardsInScrollView("allCards", CartesTypes, true);

            if (type == "entite") {
                StartCoroutine(AfficherFiltresElementaires());
            }

            ResetFiltres();
            filtreType = typeCarte;

        }
    }

    /// <summary>
    /// Filtrer les entités par type d'élémentaires. 
    /// </summary>
    /// <param name="filtre"></param>
    public void FiltreElementaire(string filtre) {
        // Dans le cas du filtre d'une entité élementaire
        if (filtre == "TERRE" || filtre == "EAU" || filtre == "FEU" || filtre == "AIR") {
            Entite.Element elementFiltre = Entite.Element.NONE;
            switch (filtre) {
                case "TERRE":
                    elementFiltre = Entite.Element.TERRE;
                    break;
                case "FEU":
                    elementFiltre = Entite.Element.FEU;
                    break;
                case "EAU":
                    elementFiltre = Entite.Element.EAU;
                    break;
                case "AIR":
                    elementFiltre = Entite.Element.AIR;
                    break;
                default:
                    Debug.LogError("Ce cas ne peut pas arriver");
                    break;
            }
            ResetFiltres(); 
            filtreElementaire = elementFiltre; 
            PutCardsInScrollView("allCards", CartesCorrespondantesElement(elementFiltre), true);
        }
        else if (filtre == "ASTRAL" || filtre == "MALEFIQUE") {
            Entite.Ascendance ascendanceFiltre = Entite.Ascendance.NONE;
            if (filtre == "ASTRAL") {
                ascendanceFiltre = Entite.Ascendance.ASTRALE;
            }
            else {
                ascendanceFiltre = Entite.Ascendance.MALEFIQUE;
            }
            ResetFiltres();
            filtreAscendance = ascendanceFiltre; 
            PutCardsInScrollView("allCards", CartesCorrespondantesAscendance(ascendanceFiltre), true);
        }
        else {
            throw new Exception("Ce filtre n'existe pas");
        }

        ListeFiltreElementaires.SetActive(false); 
        tri = Tri.NONE; 

    }

    /// <summary>
    /// Récupérer toutes les cartes d'un élément donné
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public List<GameObject> CartesCorrespondantesElement(Entite.Element element) {
        Dictionary<string, List<GameObject>> uniqueCards = getUniqueCards();
        List<GameObject> entites = CartesCorrespondantesType(CarteType.Type.ENTITE);
        List<GameObject> elementList = new List<GameObject>();
        foreach (GameObject g in entites) {
            if (g.GetComponent<Entite>().EntiteElement == element) {
                elementList.Add(g);
            }
        }
        return elementList;
    }

    /// <summary>
    /// Récupérer toutes les cartes d'une ascendance donnée. 
    /// </summary>
    /// <param name="ascendance"></param>
    /// <returns></returns>
    public List<GameObject> CartesCorrespondantesAscendance(Entite.Ascendance ascendance) {
        Dictionary<string, List<GameObject>> uniqueCards = getUniqueCards();
        List<GameObject> entites = CartesCorrespondantesType(CarteType.Type.ENTITE);
        List<GameObject> ascendanceList = new List<GameObject>();
        foreach (GameObject g in entites) {
            if (g.GetComponent<Entite>().EntiteAscendance == ascendance) {
                ascendanceList.Add(g);
            }
        }
        return ascendanceList;
    }

    /// <summary>
    /// Rechercher des cartes et les mettre dans le scrollView
    /// </summary>
    /// <param name="research"></param>
    public void RechercheCarte(string research) {
        List<GameObject> CartesCorres = CartesCorrespondantes(research);
        PutCardsInScrollView("allCards", CartesCorres, false);
    }

    /// <summary>
    /// Recuperer la liste des cartes qui correspondent à une recherche du joueur
    /// </summary>
    /// <param name="research"></param>
    /// <returns></returns>
    public List<GameObject> CartesCorrespondantes(string research) {
        Dictionary<string, List<GameObject>> uniqueCards = getUniqueCards();
        List<GameObject> listeCartes = new List<GameObject>();
        foreach (KeyValuePair<string, List<GameObject>> cartesUniques in uniqueCards) {
            if ((getStringWithoutAccent(cartesUniques.Key).IndexOf(getStringWithoutAccent(research)) != -1)
                && ((filtreType == CarteType.Type.AUCUN)
                || ((cartesUniques.Value[0].GetComponent<Carte>().GetType() == typeof(Entite) && filtreType == CarteType.Type.ENTITE)
                || (cartesUniques.Value[0].GetComponent<Carte>().GetType() == typeof(Sort) && filtreType == CarteType.Type.SORT)
                || (cartesUniques.Value[0].GetComponent<Carte>().GetType() == typeof(Assistance) && filtreType == CarteType.Type.ASSISTANCE))
                )) {
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
    /// Recupérer les cartes qui correspondent à un type précis. 
    /// </summary>
    /// <param name="typeCarte"></param>
    /// <returns></returns>
    public List<GameObject> CartesCorrespondantesType(CarteType.Type typeCarte) {
        Dictionary<string, List<GameObject>> uniqueCards = getUniqueCards(); 
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

    public Dictionary<string, List<GameObject>> getUniqueCards() {
        return GetComponent<GameManagerManageCards>().UniqueCards; 
    }

    public List<GameObject> getUniqueCardsAsList() {
        List<GameObject> listeUnique = new List<GameObject>(); 
        foreach (KeyValuePair<string, List<GameObject>> carte in getUniqueCards()) {
            listeUnique.Add(carte.Value[0]); 
        }
        return listeUnique; 
    }

    private IEnumerator AfficherFiltresElementaires() {
        ListeFiltreElementaires.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (ListeFiltreElementaires.activeInHierarchy) {
            ListeFiltreElementaires.SetActive(false);
        }
    }


    /// <summary>
    /// Trier les cartes par critère. 
    /// Tri des cartes dans le content All Cards
    /// Pour les decks les méthodes sont implémentées dans la classe Deck 
    /// <see cref="Deck.TrierDeckCritere(string)"/>
    /// </summary>
    /// <param name="critere"></param>
    public void Trier(string critere) {
        List<GameObject> listeTriee = new List<GameObject>();

        if (filtreElementaire != Entite.Element.NONE) {
            listeTriee = CartesCorrespondantesElement(filtreElementaire); 
        } else if (filtreAscendance != Entite.Ascendance.NONE) {
            listeTriee = CartesCorrespondantesAscendance(filtreAscendance); 
        } else if (filtreType != CarteType.Type.AUCUN){
            listeTriee = CartesCorrespondantesType(filtreType); 
        } else {
            listeTriee = getUniqueCardsAsList(); 
        }

        ResetBoutonsTris(); 
        switch (critere) {
            case "AKA":
                TrierAKA(listeTriee);
                BoutonsTris.transform.Find("TriAKA").gameObject.GetComponent<Image>().color = Color.green; 
                break;
            case "Puissance":
            case "puissance":
                TrierPuissance(listeTriee);
                BoutonsTris.transform.Find("TriPuissance").gameObject.GetComponent<Image>().color = Color.green;
                break;
            case "Nom":
                TrierNom(listeTriee);
                BoutonsTris.transform.Find("TriNom").gameObject.GetComponent<Image>().color = Color.green;
                break;
        }
    }

    public void ResetBoutonsTris() {
        foreach (Transform t in BoutonsTris.transform) {
            t.gameObject.GetComponent<Image>().color = Color.white; 
        }
    }

    public void ResetFiltres() {
        filtreAscendance = Entite.Ascendance.NONE;
        filtreElementaire = Entite.Element.NONE;
        filtreType = CarteType.Type.AUCUN; 
    }
}
