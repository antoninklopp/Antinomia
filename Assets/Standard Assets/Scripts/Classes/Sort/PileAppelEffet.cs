using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;


/// <summary>
/// Lorsqu'un premier effet est appelé, on crée une pile d'effet qui se résoudra dans l'ordre inverse. 
/// 
/// C'est cet effet qui se "charge de contacter" l'autre joueur pour lui permettre de répondre à un effet. 
/// 
/// La pile est créé lorsqu'un premier est joué, elle doit être détruite une fois que tous les effets sont joués. 
/// Il doit toujours y avoir au plus un objet pile. 
/// (Au moins pour l'instant, on garde ce syst_me pour pouvoir faire un historique et peut-être prochainement garder 
/// toutes les piles). 
/// </summary>
public class PileAppelEffet : NetworkBehaviourAntinomia {

    /// <summary>
    /// Liste d'effets à faire 
    /// </summary>
    List<GameObject> pileEffets = new List<GameObject>();

    /// <summary>
    /// Les cartes associées à la liste des effets. 
    /// Par exemple: l'effet 1 sera execute par l'objet 1. (ici on a l'objet grâce à son IDCardGame). 
    /// </summary>
    List<int> CartesAssociees = new List<int>();

    /// <summary>
    /// Prefab de l'objet décrivant UN effet.
    /// </summary>
    public GameObject EffetInPilePrefab; 

    /// <summary>
    /// Initialisation de la pile d'effets. 
    /// Il faut forcèment initialiser la pile d'effets avec une action.
    /// </summary>
    /// <param name="effet">Le premier element de la pile d'effets</param>
    /// <param name="Carte"> IDCardGame de la carte qui va jouer l'effet. </param>
    public PileAppelEffet() {

    }

    /// <summary>
    /// Ajouter un effet à la pile
    /// Fonction appelée depuis l'extérieur. 
    /// </summary>
    /// <param name="ObjetEffet"></param>
    /// <param name="ObjetCible"></param>
    /// <param name="numeroEffet">Le numero de l'effet dans la liste des effets</param>
    /// <param name="numeroListeEffet">0 si liste normale, 1 si astral, 2 si maléfique</param>
    public void AjouterEffetALaPile(GameObject ObjetEffet, List<GameObject> ObjetCible, 
                                        int numeroEffet, int numeroListeEffet=0) {

        int IDObjetEffet = GetObjetIDCardGame(ObjetEffet);
        List<int> ListeIDCardGameCible = new List<int>(); 
        for (int i = 0; i < ObjetCible.Count; ++i) {
            ListeIDCardGameCible.Add(GetObjetIDCardGame(ObjetCible[i])); 
        }

        // Un joueur ne pourra ajouter qu'un effet de ses propres cartes! 
        int playerID = FindLocalPlayer().GetComponent<Player>().PlayerID;

        CmdAjouterEffetALaPile(IDObjetEffet, ListeIDCardGameCible, numeroEffet, numeroListeEffet, playerID);
    }

    /// <summary>
    /// Ajouter un effet à la pile d'effet. 
    /// </summary>
    /// <param name="IDObjetEffet">IDCardGame de la carte qui a créé l'effet</param>
    /// <param name="ListeObjetsCible">Liste des IDCardGame des objets visés par l'effet</param>
    /// <param name="numeroEffet">Le numero de l'effet dans la liste</param>
    /// <param name="numeroListeEffet">Le numero de la liste d'effets</param>
    /// <param name="PlayerID">L'ID du joueur qui joue l'effet</param>
    [Command]
    void CmdAjouterEffetALaPile(int IDObjetEffet, List<int> ListeObjetsCible, int numeroEffet,
                                      int numeroListeEffet, int PlayerID) {
        RpcAjouterEffetALaPile(IDObjetEffet, ListeObjetsCible, numeroEffet, numeroListeEffet, PlayerID); 
    }

    /// <summary>
    /// Ajouter un effet à la pile d'effet. 
    /// Appelé sur le client
    /// </summary>
    /// <param name="IDObjetEffet">IDCardGame de la carte qui a créé l'effet</param>
    /// <param name="ListeObjetsCible">Liste des IDCardGame des objets visés par l'effet</param>
    /// <param name="numeroEffet">Le numero de l'effet dans la liste</param>
    /// <param name="numeroListeEffet">Le numero de la liste d'effets</param>
    /// <param name="PlayerID">L'ID du joueur qui joue l'effet</param>
    [ClientRpc]
    void RpcAjouterEffetALaPile(int IDObjetEffet, List<int> ListeObjetsCible, int numeroEffet,
                                      int numeroListeEffet, int PlayerID) {

        GameObject NouveauEffetInPile = Instantiate(EffetInPilePrefab);
        Effet effetJoue = GetEffetFromCarte(FindCardWithID(IDObjetEffet), numeroEffet, numeroListeEffet); 
        NouveauEffetInPile.GetComponent<EffetInPile>().CreerEffetInPile(IDObjetEffet, effetJoue, ListeObjetsCible);
        pileEffets.Add(NouveauEffetInPile);
        CartesAssociees.Add(IDObjetEffet); 
    }

    /// <summary>
    /// Quand tous les joueurs ont joué leurs effets successifs. 
    /// </summary>
    public void JouerLesEffets() {
        for (int i = CartesAssociees.Count; i >= 0; ++i) {
            GameObject CarteAppel = FindCardWithID(CartesAssociees[i]); 
            //if (CarteAppel.GetComponent<Carte>().isFromLocalPlayer) {
            //    CarteAppel.GetComponent<Carte>().GererActions(pileEffets[i].AllActionsEffet); 
            //} else {

            //}
        }
    }

    /// <summary>
    /// Permettre à l'autre joueur de répondre à l'effet.
    /// </summary>
    public void DroitDeReponseAutreJoueur() {


    }

    
}
