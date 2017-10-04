using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System; 

/// <summary>
/// Effet que l'on peut rajouter dans une pile. 
/// 
/// </summary>
public class EffetInPile : NetworkBehaviourAntinomia {

    /// <summary>
    /// L'objet qui a fait l'effet. PAS FORCEMENT NECESSAIRE
    /// </summary>
    public GameObject ObjetEffet;

    /// <summary>
    /// IDCardGame del'objet qui produit l'effet.
    /// </summary>
    public int IDObjectCardGame; 

    /// <summary>
    /// Les cibles de l'effet
    /// </summary>
    public List<GameObject> CibleEffet = new List<GameObject>();

    public List<int> IDCardGameCibleEffet = new List<int>(); 

    /// <summary>
    /// Effet que le joueur a joue. 
    /// </summary>
    public Effet effetJoue; 

    /// <summary>
    /// La phrase qui décrit l'effet
    /// </summary>
    public string PhraseDecritEffet;

    /// <summary>
    /// L'état de l'effet
    /// 0 si l'effet n'a pas été créé, ou créé et pas transmis dans la pile
    /// 1 si l'effet a été créé et ajouté à la pile. 
    /// 2 si l'effet a été joué. 
    /// </summary>
    public int etatEffet = 0;

    /// <summary>
    /// L'ID du joueur qui a joué l'effet. 
    /// L'information est normalement déjà contenue dans l'IDCardGame de l'ObjetEffet. 
    /// </summary>
    public int PlayerIDAssocie; 

    public void CreerEffetInPile(int _IDCardGame, Effet effet, List<int> _IDCardGameCibleEffet) {
        effetJoue = effet;
        IDObjectCardGame = _IDCardGame;
        IDCardGameCibleEffet = _IDCardGameCibleEffet; 
        GameObject Pile = GameObject.FindGameObjectWithTag("Pile");
        transform.SetParent(Pile.transform, false);

        etatEffet = 1; 
    }

    public void setPlayerIDAssociee(int _playerID) {
        PlayerIDAssocie = _playerID; 
    }

    /// <summary>
    /// Créer la phrase qui décrit l'effet pour pouvoir la transmettre à l'autre joueur. 
    /// Pour l'instant sous forme de phrase, cette information devra être visuelle.
    /// </summary>
    private void CreerPhraseDecritEffet() {
        PhraseDecritEffet = ""; 

        // QUI JOUE L'EFFET
        switch (ObjetEffet.GetComponent<CarteType>().thisCarteType) {
            case CarteType.Type.ENTITE:
                PhraseDecritEffet += ObjetEffet.GetComponent<Entite>().Name; 
                break;
            case CarteType.Type.ASSISTANCE:
                PhraseDecritEffet += ObjetEffet.GetComponent<Assistance>().Name;
                break;
            case CarteType.Type.SORT:
                PhraseDecritEffet += ObjetEffet.GetComponent<Sort>().Name;
                break;
            default:
                throw new Exception("Ce type de carte n'existe pas"); 
        }

        // QUEL EST L'EFFET
        for (int i = 0; i < effetJoue.AllActionsEffet.Count; ++i) {
            PhraseDecritEffet += effetJoue.AllActionsEffet[i].ActionAction.ToString() + " " +
                effetJoue.AllActionsEffet[i].properIntAction + " ; "; 
        }

        // A
        PhraseDecritEffet += " A "; 

        // SUR QUI A-T-IL EFFET?
        for (int i = 0; i < CibleEffet.Count; ++i) {
            switch (CibleEffet[i].GetComponent<CarteType>().thisCarteType) {
                case CarteType.Type.ENTITE:
                    PhraseDecritEffet += CibleEffet[i].GetComponent<Entite>().Name;
                    break;
                case CarteType.Type.ASSISTANCE:
                    PhraseDecritEffet += CibleEffet[i].GetComponent<Assistance>().Name;
                    break;
                case CarteType.Type.SORT:
                    PhraseDecritEffet += CibleEffet[i].GetComponent<Sort>().Name;
                    break;
                default:
                    throw new Exception("Ce type de carte n'existe pas");
            }
        }
    }


}
