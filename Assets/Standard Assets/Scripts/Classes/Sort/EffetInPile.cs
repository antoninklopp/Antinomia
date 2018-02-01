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
/// Deplacement vers le sanctuaire : -3
/// Changement de Phase: -4
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
    /// IDCardGame de l'objet qui produit l'effet.
    /// </summary>
    public int IDObjectCardGame = -1; 

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
    /// Certains objets que l'on met dans la pile peuvent ne pas appartenir au flux. 
    /// Exemple: Un changement de phase. 
    /// </summary>
    public bool AppartientAuFlux = false;

    public bool effetTermine = false;

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

        if (numeroEffet != -4) {
            // Pour un changement de phase, il n'y a pas de cartes associées
            ObjetEffet = FindCardWithID(_IDCardGame);
        }

        PhraseDecritEffet = CreerPhraseDecritEffet();

        Debug.Log(ObjetEffet);
        Debug.Log("Voici l'IDCardGame de l'objet qui crée l'effet : " + _IDCardGame.ToString());

        // getGameManager().GetComponent<GameManager>().CheckAllEffetsCartes();

        MontrerCartesDeplacement(); 
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

        // Dans le cas des effets "spéciaux". 
        if (numeroEffet == -4) {
            return "Passer à la phase suivante"; 
        } else if (numeroEffet == -3) {
            return ObjetEffet.GetComponent<Entite>().Name + " se déplace au sanctuaire"; 
        } else if (numeroEffet == -2) {
            return ObjetEffet.GetComponent<Entite>().Name + " se déplace sur le champ de Batailles";
        }

        try {
            // QUI JOUE L'EFFET
            phraseDecritEffet += ObjetEffet.GetComponent<Carte>().Name; 
        } catch (NullReferenceException e) {
            Debug.LogWarning(e); 
        }

        // QUEL EST L'EFFET
        for (int i = 0; i < effetJoue.AllActionsEffet.Count; ++i) {
            phraseDecritEffet += effetJoue.AllActionsEffet[i].ActionAction.ToString() + " " +
                effetJoue.AllActionsEffet[i].properIntAction.ToString() + " ; "; 
        }

        // A
        phraseDecritEffet += " A "; 

        // SUR QUI A-T-IL EFFET?
        for (int i = 0; i < CibleEffet.Count; ++i) {
            phraseDecritEffet += CibleEffet[i].GetComponent<Carte>().Name;
        }

        return phraseDecritEffet; 
    }

    /// <summary>
    /// Coroutine qui execute les effets. 
    /// A la fin de cette coroutine, les effets devront être parfaitement execute sur les deux machines. 
    /// </summary>
    /// <returns>NONE</returns>
    public IEnumerator JouerEffet(int playerID, bool jeuEnPause=false) {
        // On vérifie que c'est bien le joueur qui a créé l'effet qui le joue. 
        DestructionUnitesTemporaires();
        if (PlayerIDAssocie == FindLocalPlayer().GetComponent<Player>().PlayerID) {
            Debug.Log("On joue l'effet depuis chez ce joueur");
            // Dans certains cas, changement de phase par exemple, 
            // aucune carte n'a demandé l'effet. 
            if (effetTermine) {
                // Si l'effeta déjà été joué
                GameObject.FindGameObjectWithTag("Pile").SendMessage("EffetTermine");
                yield break; 
            }
            
            if (isCardAlive(IDObjectCardGame) || numeroEffet == -1 || numeroEffet == -4) {
                Debug.Log("Depuis cette carte");

                // On recrée la liste des cibles
                for (int i = 0; i < IDCardGameCibleEffet.Count; ++i) {
                    Debug.Log(i);
                    if (IDCardGameCibleEffet[i] != -1) {
                        // -1 est l'identifiant du joueur adverse. 
                        CibleEffet.Add(FindCardWithID(IDCardGameCibleEffet[i]));
                        break;
                    }
                    else {
                        CibleEffet.Add(FindNotLocalPlayer());
                    }
                }
                GameObject thisCarte = new GameObject(); 
                if (IDObjectCardGame > 0) {
                    thisCarte = FindCardWithID(IDObjectCardGame);
                }

                List<GameObject> allCardsPlayer = getAllCardsFromPlayerBoardSanctuaire(thisCarte);

                Debug.Log(numeroEffet); 
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
                        GameObject.FindGameObjectWithTag("Pile").SendMessage("EffetTermine");
                        effetTermine = true; 
                        yield break;
                    // Déplacement vers le champ de bataille. 
                    case -2:
                        // On dépose la carte sur le champ de bataille. 
                        thisCarte.GetComponent<Entite>().MoveToChampBataille(defairePile: true);
                        // TODO: Ici il faudra vérifier que la carte a bien été bougée. 
                        yield return new WaitForSeconds(0.5f);
                        // On joue les effets de dépose de la carte. 
                        GererEffetsPonctuelPile(thisCarte, 2);
                        GameObject.FindGameObjectWithTag("Pile").SendMessage("EffetTermine");
                        effetTermine = true; 
                        yield break;
                    // Déplacement vers le sanctuaire 
                    case -3:
                        thisCarte.GetComponent<Entite>().MoveToSanctuaire(defairePile: true);
                        yield return new WaitForSeconds(0.5f);

                        Debug.Log("On est sur un changement de position sur le sanctuaire");
                        // On joue les effets de dépose de la carte. 
                        GererEffetsPonctuelPile(thisCarte, 1);
                        GameObject.FindGameObjectWithTag("Pile").SendMessage("EffetTermine");
                        effetTermine = true; 
                        yield break;
                    // Passage à une nouvelle phase
                    case -4:
                        // Si le jeu a été mis en pause, on ne change pas de phase. 
                        if (!jeuEnPause) {
                            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GoToNextPhase(defairePile: true);
                            AntinomiaLog("Appel à GoToNextPhase");
                            // Debug.Log(FindLocalPlayer().GetComponent<Player>().PlayerID);
                            // Debug.Log(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Tour);
                            // Debug.Log("On va à la prochaine phase");
                        } else {
                            // Sinon , il faut quand même réactiver la pile
                            getGameManager().GetComponent<GameManager>().ReactivateButtonPhase();
                        }
                        yield return new WaitForSeconds(0.2f);
                        GameObject.FindGameObjectWithTag("Pile").SendMessage("EffetTermine");
                        effetTermine = true;
                        yield break;
                }

                Debug.Log("<color=yellow> On joue l'effet de cette carte </color>"); 
                yield return thisCarte.GetComponent<Carte>().GererActionsCoroutine(effetJoue.AllActionsEffet, numeroEffet,
                                                                                                EffetListNumber, CibleEffet);
                yield return new WaitForSeconds(0.5f); 
                Debug.Log("<color=yellow> L'effet joué par cette carte est terminé</color>"); 
                effetTermine = true; 
            }
            else {
                AntinomiaLog("La carte a été détruite");
                effetTermine = true; 
            }
            Debug.Log("<color=yellow> On passe à l'effet suivant. </color>");
            GameObject.FindGameObjectWithTag("Pile").SendMessage("EffetTermine");

        } 
    }

    /// <summary>
    /// Gestion des effets ponctuels des cartes. 
    /// </summary>
    /// <param name="_carte"></param>
    /// <param name="deposeCarte"></param>
    public void GererEffetsPonctuelPile(GameObject _carte, int deposeCarte=0) {
        switch (_carte.GetComponent<CarteType>().thisCarteType) {
            case CarteType.Type.ENTITE:
                _carte.GetComponent<Entite>().GererEffetsPonctuel(debut: true, deposeCarte: deposeCarte);
                break;
            case CarteType.Type.ASSISTANCE:
                _carte.GetComponent<Assistance>().GererEffetsPonctuel(debut: true);
                break;
            case CarteType.Type.SORT:
                _carte.GetComponent<Sort>().GererEffetsPonctuel(debut: true);
                break;
            default:
                throw new Exception("Ce type d'exception n'est pas encore géré");
        }
    }

    /// <summary>
    /// Creation des entités temporaires. 
    /// </summary>
    public void MontrerCartesDeplacement() {
        /*
         * Dans le cas où un effet est un déplacement de carte, 
         * on montre la position dans laquelle la carte va pouvoir potentiellement être.
         */
        if (numeroEffet == -2 || numeroEffet == -3) {
            GameObject CarteDeplacee = FindCardWithID(IDObjectCardGame);
            GameObject newCarte = Instantiate(CarteDeplacee); 
            if (newCarte.GetComponent<CarteType>().thisCarteType == CarteType.Type.ENTITE) {
                Entite _entite = newCarte.GetComponent<Entite>();
                EntiteTemporaire _entiteTemporaire = newCarte.AddComponent<EntiteTemporaire>();
                _entiteTemporaire.setInfoEntiteTemporaire(_entite);
                Destroy(_entite);
            }
            if (numeroEffet == -2) {
                // Deplacement vers le board. 
                Transform ChampBataille; 
                ChampBataille= FindPlayerWithID(PlayerIDAssocie).GetComponent<Player>().GetChampBatailleJoueur();
                newCarte.transform.SetParent(ChampBataille, false);
                ChampBataille.gameObject.GetComponent<CartesBoard>().CmdReordonnerCarte();
                newCarte.GetComponent<EntiteTemporaire>().carteState = Entite.State.CHAMPBATAILLE; 
            } else if (numeroEffet == -3) {
                // Deplacement vers le sanctuaire. 
                Transform Sanctuaire; 
                Sanctuaire = FindPlayerWithID(PlayerIDAssocie).GetComponent<Player>().GetSanctuaireJoueur();
                newCarte.transform.SetParent(Sanctuaire, false);
                Sanctuaire.gameObject.GetComponent<Sanctuaire>().ReordonnerCarte();
                newCarte.GetComponent<EntiteTemporaire>().carteState = Entite.State.SANCTUAIRE;
            }
        }

    }

    /// <summary>
    /// Detruire les unités temporaires, créées par la pile. 
    /// </summary>
    private void DestructionUnitesTemporaires() {
        EntiteTemporaire[] temporaires = FindObjectsOfType<EntiteTemporaire>();
        foreach (EntiteTemporaire temp in temporaires) {
            Destroy(temp.gameObject);
        }
    }

    /// <summary>
    /// On regarde si la carte est encore vivante. 
    /// </summary>
    /// <param name="IDCardGame"></param>
    /// <returns></returns>
    public bool isCardAlive(int IDCardGame) {
        if (IDCardGame == -1 || numeroEffet == -4) {
            return false;
        } else if (FindCardWithID(IDCardGame) == null) {
            return false; 
        } else {
            GameObject Card = FindCardWithID(IDCardGame);
            Debug.Log("check " + Card.GetComponent<Carte>().Name + "vivante");
            if (Card.GetComponent<Entite>() != null) {
                Debug.Log(Card.GetComponent<Entite>().getState());
            }
            if ((Card.GetComponent<Entite>() != null) && (Card.GetComponent<Entite>().getState() == Entite.State.CIMETIERE)) {
                return false;
            }
            else if ((Card.GetComponent<Sort>() != null) && (Card.GetComponent<Sort>().sortState == Sort.State.CIMETIERE)) {
                return false;
            }
        }
        return true; 
    }

}
