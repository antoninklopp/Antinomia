using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System; 

/// <summary>
/// Effet que l'on peut rajouter dans une pile. 
/// 
/// 
/// Chose qui peut être un effet:
/// Changement de position, 
/// attaque, 
/// effet, 
/// changement de phase, 
/// changement de tour, 
/// diverses réponses, 
/// 
/// 
/// NOTE : Il n'y a que quelques effet qui peuvent être faits par toutes les cartes:
/// Attaquer une autre carte ou un joueur : -1
/// Déplacement vers le board : -2
/// Deplacemet vers le sanctuaire : -3
/// Le nombre sera celui du numéro de l'effet, le numéro de la liste sera 0. 
/// 
/// 
/// </summary>
public class EffetInPile : NetworkBehaviourAntinomia {

    /// <summary>
    /// L'objet qui a fait l'effet. PAS FORCEMENT NECESSAIRE
    /// </summary>
    [HideInInspector]
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

    /// <summary>
    /// Le numero de la liste qui contient l'effet
    /// 0 pour les effets "normaux"
    /// 1 pour les effets astraux
    /// 2 pour les effets maléfiques
    /// </summary>
    public int EffetListNumber;

    /// <summary>
    /// Numero de l'effet dans la liste des effets. 
    /// </summary>
    public int numeroEffet;

    /// <summary>
    /// Creer un effet dans la pile. 
    /// </summary>
    /// <param name="_IDCardGame">IDCardGame de la carte ayant créé l'effet</param>
    /// <param name="effet">Effet joué</param>
    /// <param name="_IDCardGameCibleEffet">List des IDCardGame des cibles</param>
    /// <param name="_numeroEffet">Le numero de l'effet, sa poosition dans la liste des effets</param>
    /// <param name="numeroListEffet"> Le numero de la liste qui contient l'effet
    /// 0 pour les effets "normaux"
    /// 1 pour les effets astraux
    /// 2 pour les effets maléfiques </param>
    public void CreerEffetInPile(int _IDCardGame, Effet effet, List<int> _IDCardGameCibleEffet, int _numeroEffet, 
                                    int numeroListEffet, int playerID) {
        effetJoue = effet;
        IDObjectCardGame = _IDCardGame;
        IDCardGameCibleEffet = _IDCardGameCibleEffet; 
        GameObject Pile = GameObject.FindGameObjectWithTag("Pile");
        transform.SetParent(Pile.transform, false);
        EffetListNumber = numeroListEffet;
        numeroEffet = _numeroEffet; 
        etatEffet = 1;
        setPlayerIDAssociee(playerID);
        ObjetEffet = FindCardWithID(_IDCardGame); 

        PhraseDecritEffet = CreerPhraseDecritEffet();

        Debug.Log("Voici l'IDCardGame de l'objet qui crée l'effet : " + _IDCardGame.ToString()); 

    }

    public void setPlayerIDAssociee(int _playerID) {
        PlayerIDAssocie = _playerID; 
    }

    /// <summary>
    /// Créer la phrase qui décrit l'effet pour pouvoir la transmettre à l'autre joueur. 
    /// Pour l'instant sous forme de phrase, cette information devra être visuelle.
    /// </summary>
    public string CreerPhraseDecritEffet() {
        string phraseDecritEffet = "";
        Debug.Log(ObjetEffet); 
        try {
            // QUI JOUE L'EFFET
            switch (ObjetEffet.GetComponent<CarteType>().thisCarteType) {
                case CarteType.Type.ENTITE:
                    phraseDecritEffet += ObjetEffet.GetComponent<Entite>().Name;
                    break;
                case CarteType.Type.ASSISTANCE:
                    phraseDecritEffet += ObjetEffet.GetComponent<Assistance>().Name;
                    break;
                case CarteType.Type.SORT:
                    phraseDecritEffet += ObjetEffet.GetComponent<Sort>().Name;
                    break;
                default:
                    throw new Exception("Ce type de carte n'existe pas");
            }
        } catch (NullReferenceException e) {
            Debug.LogWarning(e); 
        }

        // QUEL EST L'EFFET
        for (int i = 0; i < effetJoue.AllActionsEffet.Count; ++i) {
            phraseDecritEffet += effetJoue.AllActionsEffet[i].ActionAction.ToString() + " " +
                effetJoue.AllActionsEffet[i].properIntAction + " ; "; 
        }

        // A
        phraseDecritEffet += " A "; 

        // SUR QUI A-T-IL EFFET?
        for (int i = 0; i < CibleEffet.Count; ++i) {
            switch (CibleEffet[i].GetComponent<CarteType>().thisCarteType) {
                case CarteType.Type.ENTITE:
                    phraseDecritEffet += CibleEffet[i].GetComponent<Entite>().Name;
                    break;
                case CarteType.Type.ASSISTANCE:
                    phraseDecritEffet += CibleEffet[i].GetComponent<Assistance>().Name;
                    break;
                case CarteType.Type.SORT:
                    phraseDecritEffet += CibleEffet[i].GetComponent<Sort>().Name;
                    break;
                default:
                    throw new Exception("Ce type de carte n'existe pas");
            }
        }

        return phraseDecritEffet; 
    }

    /// <summary>
    /// Coroutine qui execute les effets. 
    /// A la fin de cette coroutine, les effets devront être parfaitement execute sur les deux machines. 
    /// </summary>
    /// <returns>NONE</returns>
    public IEnumerator JouerEffet(int playerID) {
        // On vérifie que c'est bien le joueur qui a créé l'effet qui le joue. 
        Debug.Log(PlayerIDAssocie);
        Debug.Log(FindLocalPlayer().GetComponent<Player>().PlayerID); 
        if (PlayerIDAssocie == FindLocalPlayer().GetComponent<Player>().PlayerID) {
            Debug.Log("On joue l'effet depuis chez ce joueur");
            if (FindCardWithID(IDObjectCardGame) != null) {
                Debug.Log("Depuis cette carte");

                // On recrée la liste des cibles
                for (int i = 0; i < IDCardGameCibleEffet.Count; ++i) {
                    Debug.Log(i);
                    if (IDCardGameCibleEffet[i] != -1) {
                        // -1 est l'identifiant du joueur adverse. 
                        CibleEffet.Add(FindCardWithID(IDCardGameCibleEffet[i]));
                        break; 
                    } else {
                        CibleEffet.Add(FindNotLocalPlayer()); 
                    }
                }

                GameObject thisCarte = FindCardWithID(IDObjectCardGame);
                switch (numeroEffet) {
                    // On vérifie d'abord que l'effet ne soit pas un effet "spécial"
                    case -1:
                        // Attaque d'un entité. 
                        if (CibleEffet.Count != 1) {
                            Debug.Log("Nombre d'attaques " + CibleEffet.Count.ToString());
                            Debug.LogWarning("Pas le bon nombre d'attaques");
                        }
                        Debug.Log(thisCarte); 
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Attack(true, thisCarte, CibleEffet[0]);
                        yield return new WaitForSeconds(0.5f);
                        yield break; 
                }

                switch (thisCarte.GetComponent<CarteType>().thisCarteType) {
                    case CarteType.Type.ENTITE:
                        yield return thisCarte.GetComponent<Entite>().GererActionsCoroutine(effetJoue.AllActionsEffet, numeroEffet,
                                                                                                EffetListNumber, CibleEffet);
                        break;
                    case CarteType.Type.SORT:
                        yield return thisCarte.GetComponent<Sort>().GererActionsCoroutine(effetJoue.AllActionsEffet, numeroEffet,
                                                                                                EffetListNumber, CibleEffet);
                        break;
                    case CarteType.Type.ASSISTANCE:
                        yield return thisCarte.GetComponent<Assistance>().GererActionsCoroutine(effetJoue.AllActionsEffet, numeroEffet,
                                                                                                EffetListNumber, CibleEffet);
                        break;
                }
            } else {
                Debug.Log("La carte a été détruite"); 
            }
            GameObject.FindGameObjectWithTag("Pile").SendMessage("EffetTermine");
        }
    }


}
