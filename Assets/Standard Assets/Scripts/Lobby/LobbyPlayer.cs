using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 


/// <summary>
/// Représentation du joueur dans le lobby. 
/// </summary>
public class LobbyPlayer : NetworkLobbyPlayer {

    public string Name;
    public string ID;
    public bool online;
    // Doit être égal à 1 ou à 2. 
    public int peerID;

    public string matchMakingID; 

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

    public LobbyPlayer(int _peerID, string _matchMakingID) {
        matchMakingID = _matchMakingID;
        peerID = _peerID; 
    }

}
