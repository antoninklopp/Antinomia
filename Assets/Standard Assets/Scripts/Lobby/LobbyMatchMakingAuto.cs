using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI; 

/// <summary>
/// Lobby du matchmaking auto
/// </summary>
public class LobbyMatchMakingAuto : NetworkLobbyManager {
    /*
     * Fonctionne mais doit encore vraiment être amélioré. 
     */ 

    // Il faut qu'il y ait un composant MatchMakingGameSparks qui soit sur le gameManager
    MatchMakingGameSparks matchMaking;

    static short MsgKicked = MsgType.Highest + 1;

    System.UInt64 _currentMatchID;

    bool OtherPlayerReady;

    public LobbyPlayer _playerAssiociated;

    public void Start() {
        matchMaking = GetComponent<MatchMakingGameSparks>(); 
    }

    public void ChercherMatch() {
        // Il faudra remplacer par le skill ici. 
        matchMaking.MultiplayerMatchRequest(10);
        StartCoroutine(LaunchMatch()); 
        
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        _currentMatchID = (System.UInt64)matchInfo.networkId;
    }

    public void setOtherPlayerReady() {
        OtherPlayerReady = true; 
    }

    /// <summary>
    /// On attend qu'il y ait un autre match avant de pouvoir lancer le match. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator LaunchMatch() {
        while (!OtherPlayerReady) {
            yield return new WaitForSeconds(0.5f); 
        }
    }

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);

        conn.RegisterHandler(MsgKicked, KickedMessageHandler);
    }

    public void KickedMessageHandler(NetworkMessage netMsg) {
        // infoPanel.Display("Kicked by Server", "Close", null);
        netMsg.conn.Disconnect();
    }

    /// <summary>
    /// Lorsqu'on recupère un joueur contre lequel il faut se battre, il faut se décider de qui va joueur contre qui. 
    /// INUTILE
    /// </summary>
    /// <returns>true si string player1 > string player2, false sinon</returns>
    public static bool PlayerInferieur(string player1, string player2) {
        if (player1.Length > player2.Length) {
            return true;
        } else if (player1.Length < player2.Length) {
            return false;
        } else {
            for (int i = 0; i < player1.Length; i++) {
                if ((int)char.Parse((player1.Substring(i, 1))) > (int)char.Parse((player2.Substring(i, 1)))) {
                    return true; 
                } else if((int)char.Parse((player1.Substring(i, 1))) < (int)char.Parse((player2.Substring(i, 1)))) {
                    return false; 
                }
            }
        }
        // Normalement on ne devrait jamais arriver ici parce que les deux identifiants sont différents. 
        return true; 
    }

    public void CreateGame(LobbyPlayer _player) {
        _playerAssiociated = _player;
        // S'il a l'ID 1, c'est lui crée la Game
        if (_playerAssiociated.peerID == 1) {
            CreateMatchmakingGame(); 
        } else {
            StartCoroutine(WaitToJoin()); 
        }
    }

    /// <summary>
    /// Création de la salle
    /// </summary>
    public void CreateMatchmakingGame() {
        StartMatchMaker();
        matchMaker.CreateMatch(
            // Creation de la salle de Host
            _playerAssiociated.matchMakingID,
            (uint)maxPlayers,
            true,
            "", "", "", 0, 0,
            OnMatchCreate);

        //backDelegate = StopHost;
        //_isMatchmaking = true;
        //DisplayIsConnecting();

        //SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
    }

    /// <summary>
    /// Joindre l'host. 
    /// </summary>
    public void OnClickJoin() {
        StartMatchMaker();
        matchMaker.ListMatches(0, 6, _playerAssiociated.matchMakingID, true, 0, 0, SimpleVoidDisplay); 
        // networkAddress = _playerAssiociated.matchMakingID;
        Debug.Log(_playerAssiociated.matchMakingID);
        StartClient();
        StartCoroutine(Info()); 

        // lobbyManager.backDelegate = lobbyManager.StopClientClbk;
        // lobbyManager.DisplayIsConnecting();

        // lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
    }

    public IEnumerator WaitToJoin() {
        yield return new WaitForSeconds(1f);
        OnClickJoin(); 
    }

    private IEnumerator Info() {
        while (true) {
            Debug.Log(client.serverIp);
            Debug.Log(client.isConnected);
            yield return new WaitForSeconds(1f); 
        }
    }

    public void SimpleVoidDisplay(bool success, string extendedInfo, List<MatchInfoSnapshot> match) {
        for (int i = 0; i < match.Count; i++) {
            Debug.Log(1);
            Debug.Log(match[0].networkId);
            // networkAddress = 0;
            JoinMatch(match[0].networkId);
            
        }
    }

    void JoinMatch(NetworkID networkID) {
        matchMaker.JoinMatch(networkID, "", "", "", 0, 0, OnMatchJoined);
        // backDelegate = lobbyManager.StopClientClbk;
        // _isMatchmaking = true;
        // lobbyManager.DisplayIsConnecting();
    }


}
