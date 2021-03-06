﻿
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace Antinomia.Battle {


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

        bool effetTermine = false;

        public override void Start() {
            base.Start();
        }

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
                                            int numeroEffet, int numeroListeEffet = 0, bool ProposerDefairePile = true) {

            AntinomiaLog("On ajoute un effet à la pile dans effetInPile");
            AntinomiaLog(ObjetEffet);
            int IDObjetEffet = GetObjetIDCardGame(ObjetEffet);
            int[] ListeIDCardGameCible = null;
            if (ObjetCible != null) {
                // On crée une liste qui regroupe les IDCardGame des cibles. 
                ListeIDCardGameCible = new int[ObjetCible.Count];
                for (int i = 0; i < ObjetCible.Count; ++i) {
                    Debug.Log("Cible ici");
                    ListeIDCardGameCible[i] = GetObjetIDCardGame(ObjetCible[i]);
                }
            }

            // Un joueur ne pourra ajouter qu'un effet de ses propres cartes! 
            int playerID = FindLocalPlayer().GetComponent<Player>().PlayerID;

            if (hasAuthority) {
                CmdAjouterEffetALaPile(IDObjetEffet, ListeIDCardGameCible, numeroEffet, numeroListeEffet, playerID, ProposerDefairePile);
            }
            else {
                FindLocalPlayer().GetComponent<Player>().CmdAjouterEffetALaPile(IDObjetEffet,
                                            ListeIDCardGameCible, numeroEffet, numeroListeEffet, playerID, ProposerDefairePile);
            }
        }

        /// <summary>
        /// Ajouter un effet à la pile d'effet. 
        /// </summary>
        /// <param name="IDObjetEffet">IDCardGame de la carte qui a créé l'effet</param>
        /// <param name="ListeObjetsCible">Liste des IDCardGame des objets visés par l'effet</param>
        /// <param name="numeroEffet">Le numero de l'effet dans la liste</param>
        /// <param name="numeroListeEffet">Le numero de la liste d'effets</param>
        /// <param name="PlayerID">L'ID du joueur qui joue l'effet</param>
        [Command(channel = 0)]
        void CmdAjouterEffetALaPile(int IDObjetEffet, int[] ListeObjetsCible, int numeroEffet,
                                          int numeroListeEffet, int PlayerID, bool ProposerDefairePile) {
            RpcAjouterEffetALaPile(IDObjetEffet, ListeObjetsCible, numeroEffet, numeroListeEffet, PlayerID, ProposerDefairePile);
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
        [ClientRpc(channel = 0)]
        public void RpcAjouterEffetALaPile(int IDObjetEffet, int[] ListeObjetsCible, int numeroEffet,
                                          int numeroListeEffet, int PlayerID, bool ProposerDefairePile) {
            AjouterEffetALaPile(IDObjetEffet, ListeObjetsCible, numeroEffet, numeroListeEffet, PlayerID, ProposerDefairePile);
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
        /// <param name="ProposeDefairePile">Si True alors cet effet est le dernier joué, il doit proposer de défaire la pile</param>
        public void AjouterEffetALaPile(int IDObjetEffet, int[] ListeObjetsCible, int numeroEffet,
                                          int numeroListeEffet, int PlayerID, bool ProposeDefairePile) {

            GameObject NouveauEffetInPile = Instantiate(EffetInPilePrefab);
            Effet effetJoue = new Effet();
            switch (numeroEffet) {
                // On vérifie que ce n'est pas un effet "spécial" tel qu'une attaque ou un déplacement.
                // Attaque
                case -1:
                    effetJoue = new Effet {
                        AllActionsEffet = new List<Action> {
                        new Action(Action.ActionEnum.ATTAQUE, 0)
                    }
                    };
                    break;
                // Changement de phase
                case -4:

                    break;
                // Changement de position vers le sanctuaire ou vers le champ de bataille
                case -3:
                case -2:

                    break;
                default:
                    effetJoue = GetEffetFromCarte(FindCardWithID(IDObjetEffet), numeroEffet, numeroListeEffet);
                    break;
            }

            // On crée une liste d'entier au lieu d'un tableau. 
            List<int> ListeObjetsCibleInt = new List<int>();
            for (int i = 0; i < ListeObjetsCible.Length; ++i) {
                Debug.Log("Cible la");
                ListeObjetsCibleInt.Add(ListeObjetsCible[i]);
            }

            AntinomiaLog("ID de l'objet effet  " + IDObjetEffet);
            NouveauEffetInPile.GetComponent<EffetInPile>().CreerEffetInPile(IDObjetEffet, effetJoue, ListeObjetsCibleInt, numeroEffet,
                                                                                  numeroListeEffet, PlayerID);
            pileEffets.Add(NouveauEffetInPile);
            CartesAssociees.Add(IDObjetEffet);

            AntinomiaLog(PlayerID);
            AntinomiaLog(FindLocalPlayer().GetComponent<Player>().PlayerID);

            Debug.Log("On va regarder les effets");


            Debug.Log("Defaire Pile " + ProposeDefairePile);
            AntinomiaLog("Defaire Pile " + ProposeDefairePile);

            if (PlayerID != FindLocalPlayer().GetComponent<Player>().PlayerID) {
                // On propose à tous les joueurs de répondre/ajouter des effets à la pile
                if (ProposeDefairePile) {
                    AntinomiaLog("On propose");
                    Debug.Log("On propose");
                    InformerAjoutEffetPile(NouveauEffetInPile.GetComponent<EffetInPile>().CreerPhraseDecritEffet());
                }
                else {
                    Debug.Log("On ne propose pas");
                    AntinomiaLog("One ne propose pas");
                    InformerSansPause(NouveauEffetInPile.GetComponent<EffetInPile>().CreerPhraseDecritEffet());
                }
                AntinomiaLog("L'effet devrait être display");
            }
            // On ne regarde les effets que si on est chez le joueur qui a créé l'effet. 
            else {
                GameObject thisCarte = NouveauEffetInPile.GetComponent<EffetInPile>().ObjetEffet;

                List<GameObject> allCardsPlayer = getAllCardsFromPlayerBoardSanctuaire(thisCarte);

                int deposeCarte = 0;
                // On regarde les effets des cartes. 
                switch (numeroEffet) {
                    case -2:
                    case -3:
                        ////////////// GESTION DES EFFETS PONCTUELS ////////////////
                        switch (numeroEffet) {
                            case -2:
                                // On dépose une carte sur le board
                                Debug.Log("Depose carte sur le board.");
                                deposeCarte = 2;
                                break;
                            case -3:
                                // On dépose une carte sur le sanctuaire
                                Debug.Log("<color=red>Depose Carte 1</color>");
                                deposeCarte = 1;
                                break;
                            default:
                                // Si l'effet n'est d'aucun de ces deux types, 
                                // c'est qu'il n'y a aucun effet à jouer lors d'une dépose de carte
                                // Mais il peut quand même y avoir d'autres effets.
                                break;
                        }
                        break;
                }

                StartCoroutine(CheckEffetCartesApresAjout(thisCarte, deposeCarte, allCardsPlayer, NouveauEffetInPile));
            }

        }

        /// <summary>
        /// On a besoin d'attendre que tous les effets soient finis avant de pouvoir jouer ces effets là. 
        /// </summary>
        /// <param name="thisCarte"></param>
        /// <param name="deposeCarte"></param>
        /// <param name="allCardsPlayer"></param>
        /// <param name="NouveauEffetInPile"></param>
        /// <returns></returns>
        private IEnumerator CheckEffetCartesApresAjout(GameObject thisCarte, int deposeCarte,
            List<GameObject> allCardsPlayer, GameObject NouveauEffetInPile) {

            while (!getGameManager().GetComponent<GameManager>().IsAllEffetsFinis()) {
                yield return new WaitForSeconds(0.3f);
            }

            getGameManager().GetComponent<GameManager>().ResetEventManager();
            for (int i = 0; i < allCardsPlayer.Count; i++) {
                Debug.Log("On gere les effets ponctuels de la pile");
                NouveauEffetInPile.GetComponent<EffetInPile>().GererEffetsPonctuelPile(allCardsPlayer[i], deposeCarte);
            }
            if (deposeCarte == 0) {
                if (thisCarte != null) {
                    NouveauEffetInPile.GetComponent<EffetInPile>().GererEffetsPonctuelPile(thisCarte, deposeCarte);
                }
            }
            getGameManager().GetComponent<GameManager>().JouerEventManager();

            yield break;
        }

        /// <summary>
        /// Informer le joueur adverse de l'ajout d'un effet à la pile.
        /// </summary>
        void InformerAjoutEffetPile(string Phrase) {
            StartCoroutine(getGameManager().GetComponent<GameManager>().ProposeToPauseGame(message: Phrase));
        }

        /// <summary>
        /// Informer le joueur adverse d'un ajout d'effet dans la pile sans lui permettre de faire Pause. 
        /// </summary>
        /// <param name="Phrase"></param>
        void InformerSansPause(string Phrase) {
            getGameManager().GetComponent<GameManager>().InformerEffet(message: Phrase);
        }

        /// <summary>
        /// Defaire la pile d'effets. 
        /// </summary>
        /// <param name="jeuEnPause">true si le jeu a été mis en pause au moins une fois pendant le flux. </param>
        public void DefaireLaPile(bool jeuEnPause = false) {
            StartCoroutine(JouerLesEffets(jeuEnPause));
        }

        /// <summary>
        /// Quand tous les joueurs ont joué leurs effets successifs. 
        /// </summary>
        private IEnumerator JouerLesEffets(bool jeuEnPause = false) {

            if (hasAuthority) {
                AntinomiaLog("J'ai autorité sur la pile");
                int NombreEffet = CartesAssociees.Count;
                Debug.Log("Il y a " + NombreEffet.ToString() + " effets dans la pile");
                for (int i = CartesAssociees.Count - 1; i >= 0; --i) {
                    effetTermine = false;
                    Debug.Log("On joue un effet dans la boucle for ici.");
                    CmdJouerEffetPile(i, FindLocalPlayer().GetComponent<Player>().PlayerID, jeuEnPause: jeuEnPause);
                    while (!effetTermine) {
                        yield return new WaitForSeconds(0.1f);
                    }
                    yield return new WaitForSeconds(0.5f);
                }

                FindLocalPlayer().GetComponent<Player>().CmdDetruirePile(gameObject);
                Debug.Log("<color=red> La pile devrait être détruite </color>");

            }
            else {
                // Si le dernier effet est chez quelqu'un qui n'a pas instancié la pile
                // On doit envoyer l'information à l'autre joueur de défaire la pile. 
                AntinomiaLog("Je n'ai pas autorité je passe à l'autre joueur.");
                FindLocalPlayer().GetComponent<Player>().CmdJouerEffet();
            }
        }

        /// <summary>
        /// Jouer un effet dans la pile.
        /// Execute sur le serveur. 
        /// </summary>
        /// <param name="effetI"></param>
        /// <param name="_PlayerID"></param>
        [Command(channel = 0)]
        private void CmdJouerEffetPile(int effetI, int _PlayerID, bool jeuEnPause) {
            Debug.Log("Serveur CmdJouerEffetPile");
            RpcJouerEffetPile(effetI, _PlayerID, jeuEnPause);
        }

        /// <summary>
        /// Jouer un effet dans la pile. 
        /// Execute sur les clients. 
        /// </summary>
        /// <param name="effetI"></param>
        /// <param name="_PlayerID"></param>
        [ClientRpc(channel = 0)]
        private void RpcJouerEffetPile(int effetI, int _PlayerID, bool jeuEnPause) {
            AntinomiaLog("On joue l'effet");
            StartCoroutine(pileEffets[effetI].GetComponent<EffetInPile>().JouerEffet(_PlayerID, jeuEnPause));
        }

        /// <summary>
        /// Permettre à l'autre joueur de répondre à l'effet.
        /// </summary>
        public void DroitDeReponseAutreJoueur() {


        }

        /// <summary>
        /// Quand l'effet est termine on met effetTermine à true pour pouvoir faire l'effet Suivant.
        /// </summary>
        public void EffetTermine() {
            FindLocalPlayer().GetComponent<Player>().CmdSetEffetTermine();
        }

        public void setEffetTermineFromPlayer(bool termine) {
            effetTermine = termine;
        }

        /// <summary>
        /// regarder si le dernier effet vient du joueur. 
        /// </summary>
        public bool DernierEffetVientJoueur(int playerID) {
            // On assure qu'il y ait au moins un effet dans la pile
            Debug.Assert(pileEffets.Count > 0);
            GameObject lastEffet = pileEffets[pileEffets.Count - 1];
            if (lastEffet.GetComponent<EffetInPile>().PlayerIDAssocie == playerID) {
                return true;
            }
            else {
                return false;
            }

        }

        /// <summary>
        /// On regarde si défaire la pile nécessite l'approbation des joueurs. 
        /// </summary>
        /// <returns>true si défaire la pile nécessite l'approbation du joueur, false sinon</returns>
        public bool DefaireNecessiteApprobationJoueur() {
            // S'il y a plus de 2 effets dans la pile, il faut l'approbation. 
            if (transform.childCount > 1) {
                return true;
            }
            // Pour l'instant seul le changement de phase, le passage d'une carte au sanctuaire ou au champ de bataille, 
            // implique qu'il n'y ait pas
            // d'approbation du joueur qui a créé la pile. 
            else if (pileEffets[0].GetComponent<EffetInPile>().numeroEffet == -4
                    || pileEffets[0].GetComponent<EffetInPile>().numeroEffet == -3
                    || pileEffets[0].GetComponent<EffetInPile>().numeroEffet == -2) {
                return false;
            }
            else {
                return true;
            }
        }

        public List<GameObject> GetPileEffets() {
            return pileEffets;
        }

        /// <summary>
        /// Verification que l'information a bien été reçue par le joueur. 
        /// </summary>
        /// <param name="PlayerID">PlayerID du joueur qui a RECU l'effet. </param>
        [Command(channel = 0)]
        public void CmdEffetRecu(int PlayerID) {
            RpcEffetRecu(PlayerID);
        }

        [ClientRpc(channel = 0)]
        public void RpcEffetRecu(int PlayerID) {
            if (PlayerID != FindLocalPlayer().GetComponent<Player>().PlayerID) {

            }
        }

        /// <summary>
        /// Renvoie le nombre d'effets présents dans la pile. 
        /// A TESTER
        /// </summary>
        /// <returns></returns>
        public int NombreEffetsPile() {
            int nombre = 0;
            foreach (GameObject g in pileEffets) {
                if (g.GetComponent<EffetInPile>().numeroEffet != -4) {
                    nombre++;
                }
            }
            return nombre;
        }

    }

}
