
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Représentation du joueur dans le lobby. 
/// Classe Lobby Player du MatchMaking Auto de Antinomia (!= celui du Lobby). 
/// </summary>
public class LobbyPlayer : NetworkLobbyPlayer {

    public string Name;
    public string ID;
    public bool online;
    // Doit être égal à 1 ou à 2. 
    public int peerID;
    public Color color;

    public string matchMakingID;

    /// <summary>
    /// Constructeur de la classe LobbyPlayer
    /// </summary>
    public LobbyPlayer() {

    }

    /// <summary>
    /// Constructeur de la classe LobbyPlayer
    /// </summary>
    /// <param name="_Name">Nom du joueur</param>
    /// <param name="_ID">ID du joueur</param>
    /// <param name="_online">Si le joueur est encore online</param>
    /// <param name="_peerID">peerID du joueur c'est-à-dire si c'est le premier 
    /// (host la game) ou le deuxime</param>
    public LobbyPlayer(string _Name, string _ID, bool _online, int _peerID, string _matchMakingID) {
        Name = _Name;
        ID = _ID;
        online = _online;
        peerID = _peerID;
        matchMakingID = _matchMakingID;
    }

    /// <summary>
    /// Constructeur de la classe LobbyPlayer
    /// </summary>
    /// <param name="_peerID"></param>
    /// <param name="_matchMakingID"></param>
    public LobbyPlayer(int _peerID, string _matchMakingID) {
        matchMakingID = _matchMakingID;
        peerID = _peerID;
    }

    /// <summary>
    /// Obliger le joueur à passer en prêt. 
    /// </summary>
    public void SetPlayerReady() {
        Debug.Log("send");
        if (this.isLocalPlayer)
            Debug.Log("local");
        this.SendReadyToBeginMessage();
    }

    /// <summary>
    /// TODO: à changer, va être déprécier. 
    /// Permet de désactiver certains boutons au changement de scène. 
    /// </summary>
    /// <param name="level"></param>
    public void OnLevelWasLoaded(int level) {
        GameObject.Find("GameSearch").SetActive(false);
    }

}
