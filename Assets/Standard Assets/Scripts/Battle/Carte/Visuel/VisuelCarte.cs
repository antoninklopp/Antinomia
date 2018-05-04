
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

/// <summary>
/// Un script qui met en place le visuel de la carte lors de son apparition. 
/// </summary>
public class VisuelCarte : MonoBehaviour{

    /// <summary>
    /// Flag pour ne pas refaire toute l'operation, si ça a déjà été fait. 
    /// </summary>
    private bool VisuelOK = false;

    /// <summary>
    /// Créer le visuel de la carte. 
    /// </summary>
    public void SetUpVisuel() {
        if (VisuelOK) {
            Debug.Log("Le visuel est déjà en place");
            return;
        }

        StartCoroutine(SetUpVisuelRoutine());
    }

    private IEnumerator SetUpVisuelRoutine() {
        
        // Récupération de tous les attributs à changer. 
        GameObject Nom;
        GameObject Puissance;
        GameObject TexteCarte;
        // Cout AKA ou rang pour les sorts. 
        GameObject CoutAKA;

        GameObject FondCarte; 

        while (GetComponent<Carte>().Name.Length <= 1) {
            // On attend l'arrivée des informations
            yield return new WaitForSeconds(0.1f); 
        }

        // On vérifie que tous les objets existent bien.
        try {
            Nom = transform.Find("Name").gameObject;
            Puissance = transform.Find("Puissance").gameObject;
            TexteCarte = transform.Find("TextCarte").gameObject;
            CoutAKA = transform.Find("CoutAKA").gameObject;
            FondCarte = transform.Find("FondCarte").gameObject; 
        } catch (NullReferenceException) {
            Debug.LogError("Les objets enfants sont introuvables.\n Impossible de créer les attributs de carte");
            Destroy(this);
            yield break;
        }

        // On réactive les composants dans le cas où ils auraient été désactivés
        // Dans le cas d'un déplacement de l'adversaire par exemple
        Nom.SetActive(true);
        Puissance.SetActive(true);
        TexteCarte.SetActive(true);
        CoutAKA.SetActive(true);

        Nom.GetComponent<TextMesh>().text = SetUpString(GetComponent<Carte>().Name, 25);
        
        // Mise en place de la puissance de la carte. (/niveau pour les sorts). 
        if (GetComponent<Carte>().IsEntite()) {
            Puissance.GetComponent<TextMesh>().text = GetComponent<Entite>().getPuissance().ToString();
        } else if (GetComponent<Carte>().IsAssistance()) {
            Puissance.GetComponent<TextMesh>().text = GetComponent<Assistance>().Puissance.ToString(); 
        }
        // Equivalent de la puissance pour les sorts => Niveau. 
        else if (GetComponent<Carte>().IsSort()) {
            Puissance.GetComponent<TextMesh>().text = GetComponent<Sort>().Niveau.ToString(); 
        } else {
            // Sinon on desative le TextMesh (ne devrait pas arriver normalement).
            Puissance.SetActive(false);
        }

        // Cout en AKA
        if (GetComponent<Carte>().IsEntite()) {
            CoutAKA.GetComponent<TextMesh>().text = GetComponent<Entite>().CoutAKA.ToString();
        } else if (GetComponent<Carte>().IsSort()) {
            CoutAKA.GetComponent<TextMesh>().text = GetComponent<Sort>().Rang.ToString();
        } else {
            CoutAKA.SetActive(false);
        }

        // Mise en place du texte de la carte. 
        // A afficher en jeu? (si trop de texte). 
        TexteCarte.GetComponent<TextMesh>().text = SetUpString(GetComponent<Carte>().GetInfoCarte(), 25);


        // On fait des vérifications dans l'éditeur. 
#if (UNITY_EDITOR)
        if (!CheckTailleCarte()) {
            Debug.LogError("Probleme avec le format de la carte"); 
        }
#endif

        FondCarte.GetComponent<SpriteRenderer>().color = GetCouleurCarte(); 

        VisuelOK = true;
    }

    /// <summary>
    /// Si une carte est une carte de l'adversaire dans sa main, 
    /// on ne veut pas que le joueur adverse ait toutes les informations sur la carte. 
    /// </summary>
    public void DisableVisuel() {
        transform.Find("Name").gameObject.SetActive(false);
        transform.Find("Puissance").gameObject.SetActive(false);
        transform.Find("TextCarte").gameObject.SetActive(false);
        transform.Find("CoutAKA").gameObject.SetActive(false);
        VisuelOK = false; 
    }

    /// <summary>
    /// Cette méthode est TEMPORAIRE. 
    /// Elle permet juste d'ajouter une touche de visuel. 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="ascendance"></param>
    /// <returns></returns>
    public Color GetCouleurCarte() {
        // Si c'est une entité. 
        if (GetComponent<Carte>().IsEntite()) {
            Entite.Element element = GetComponent<Entite>().EntiteElement;
            Entite.Ascendance ascendance = GetComponent<Entite>().EntiteAscendance;
            Entite.Nature nature = GetComponent<Entite>().EntiteNature; 

            switch (element) {
                case Entite.Element.AIR:
                    // On retourne une teinte verte
                    return new Color(0, 1, 0);
                case Entite.Element.EAU:
                    return new Color(0, 0, 1); 
                case Entite.Element.FEU:
                    return new Color(1, 0, 0); 
                case Entite.Element.TERRE:
                    return new Color(1, 190f / 255f, 73f / 255f);
                case Entite.Element.NONE:
                    // On ne met pas de couleur ici 
                    break;
            }

            switch (ascendance) {
                case Entite.Ascendance.ASTRALE:
                    return Color.white; 
                case Entite.Ascendance.MALEFIQUE:
                    // Couleur grise. 
                    return new Color(0.5f, 0, 1f); 
                case Entite.Ascendance.NONE:
                    // On ne met aps de couleur ici.
                    break;
            }

            if (nature == Entite.Nature.NEUTRE) {
                return new Color(0.4f, 0.4f, 0.4f);
            }
        }
        // Si c'est un sort. 
        else if (GetComponent<Carte>().IsSort()) {
            return Color.white;
        }
        // Si c'est une assistance. 
        else if (GetComponent<Carte>().IsAssistance()) {
            return Color.white;
        }

        Debug.LogError("On ne devrait pas arriver ici normalement. "); 
        return Color.white;
    }


    /// <summary>
    /// A IMPLEMENTER.
    /// 
    /// Dans l'editor il faudra bien checker que toutes les cartes aient le bon format. 
    /// </summary>
    /// <returns></returns>
    public bool CheckTailleCarte() {
        return true; 
    }

    /// <summary>
    /// Formater le nom pour qu'il rentre dans toutes les cartes
    /// </summary>
    /// <param name="name"></param>
    private string SetUpString(string name, int nombreCharacParLigne) {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++) {
            if (i % nombreCharacParLigne == 0)
                sb.Append('\n');
            sb.Append(name[i]);
        }
        return sb.ToString();
    }
}
