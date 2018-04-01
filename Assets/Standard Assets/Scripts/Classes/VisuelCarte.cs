using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

/// <summary>
/// Un script qui met en place le visuel de la carte lors de son apparition. 
/// </summary>
public class VisuelCarte : MonoBehaviour{

    /// <summary>
    /// Créer le visuel de la carte. 
    /// </summary>
    public void SetUpVisuel() {
        // Récupération de tous les attributs à changer. 
        GameObject Nom;
        GameObject Puissance;
        GameObject TexteCarte;
        // Cout AKA ou rang pour les sorts. 
        GameObject CoutAKA; 

        // On vérifie que tous les objets existent bien.
        try {
            Nom = transform.Find("Name").gameObject;
            Puissance = transform.Find("Puissance").gameObject;
            TexteCarte = transform.Find("TextCarte").gameObject;
            CoutAKA = transform.Find("CoutAKA").gameObject; 
        } catch (NullReferenceException) {
            Debug.LogError("Les objets enfants sont introuvables.\n Impossible de créer les attributs de carte"); 
            return; 
        }

        Nom.GetComponent<TextMesh>().text = GetComponent<Carte>().Name;
        
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
        TexteCarte.GetComponent<TextMesh>().text = GetComponent<Carte>().GetInfoCarte();
    }

    /// <summary>
    /// Cette méthode est TEMPORAIRE. 
    /// Elle permet juste d'ajouter une touche de visuel. 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="ascendance"></param>
    /// <returns></returns>
    public Color GetCouleurCarte(Entite.Element element, Entite.Ascendance ascendance, Entite.Nature nature) {
        // Si c'est une entité. 
        if (GetComponent<Carte>().IsEntite()) {
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

}
