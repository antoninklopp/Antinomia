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

    /// <summary>
    /// L'ID du match.
    /// </summary>
    System.UInt64 _currentMatchID;

    bool OtherPlayerReady;

    public LobbyPlayer _playerAssiociated;

    /// <summary>
    /// Methode Start
    /// </summary>
    public void Start() {
        matchMaking = GetComponent<MatchMakingGameSparks>(); 
    }

    /// <summary>
    /// Rechercher un match via GameSparks. 
    /// </summary>
    public void ChercherMatch() {
        // Il faudra remplacer par le skill ici. 
        matchMaking.MultiplayerMatchRequest(10);
        StartCoroutine(LaunchMatch());
        MontrerRechercher(); 
    }
    
    /// <summary>
    /// Lorsque le match se crée. 
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="matchInfo"></param>
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

    /// <summary>
    /// une game a été trouvé on crée le lobby. 
    /// </summary>
    /// <param name="_player"></param>
    public void CreateGame(LobbyPlayer _player) {
        _playerAssiociated = _player;
        // S'il a l'ID 1, c'est lui crée la Game
        if (_playerAssiociated.peerID == 1) {
            CreateMatchmakingGame(); 
        } else {
            StartCoroutine(WaitToJoin()); 
        }
        MontrerMatchTrouve(); 
    }

    /// <summary>
    /// Création de la salle
    /// </summary>
    public void CreateMatchmakingGame() {
        StartMatchMaker();
        matchMaker.CreateMatch(
            // Creation de la salle de Host
            _playerAssiociated.matchMakingID,
            2,
            true,
            "", "", "", 0, 0,
            OnMatchCreate);

        StartCoroutine(WaitForConnectionBothPlayers(1));
    }

    /// <summary>
    /// Joindre l'host. 
    /// </summary>
    public void OnClickJoin() {
        StartMatchMaker();
        StartCoroutine(ListMatchesRoutine()); 
        // networkAddress = _playerAssiociated.matchMakingID;
        Debug.Log(_playerAssiociated.matchMakingID);
        // StartCoroutine(Info());

        // lobbyManager.backDelegate = lobbyManager.StopClientClbk;
        // lobbyManager.DisplayIsConnecting();

        // lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);

        StartCoroutine(WaitForConnectionBothPlayers(2)); 
    }

    public IEnumerator ListMatchesRoutine() {
        yield return new WaitForSeconds(2f); 
        matchMaker.ListMatches(0, 6, _playerAssiociated.matchMakingID, true, 0, 0, SimpleVoidDisplay);
        while (client.isConnected == false) {
            yield return new WaitForSeconds(0.5f);
            // matchMaker.ListMatches(0, 6, _playerAssiociated.matchMakingID, true, 0, 0, SimpleVoidDisplay);
        }
    }

    /// <summary>
    /// Attendre que le player Join la game.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Callback appelé par la recherche des matchs. 
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="match"></param>
    public void SimpleVoidDisplay(bool success, string extendedInfo, List<MatchInfoSnapshot> match) {
        Debug.Log("salut"); 
        for (int i = 0; i < match.Count; i++) {
            Debug.Log(1);
            Debug.Log(match[0].networkId);
            // networkAddress = 0;
            JoinMatch(match[0].networkId);
            // FindLobbyPlayer(2).GetComponent<LobbyPlayer>().SetPlayerReady();
        }
    }

    /// <summary>
    /// Rejoindre un match. 
    /// </summary>
    /// <param name="networkID"></param>
    void JoinMatch(NetworkID networkID) {
        matchMaker.JoinMatch(networkID, "", "", "", 0, 0, OnMatchJoined);
        // backDelegate = lobbyManager.StopClientClbk;
        // _isMatchmaking = true;
        // lobbyManager.DisplayIsConnecting();
    }

    /// <summary>
    /// INUTILE
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    GameObject FindLobbyPlayer(int ID) {
        LobbyPlayer[] Player = FindObjectsOfType<LobbyPlayer>();
        Debug.Log(Player[0].peerID);
        Debug.Log(Player[1].peerID); 
        if (Player[0].peerID == ID) {
            return Player[0].gameObject; 
        } else {
            return Player[1].gameObject; 
        }
    }

    /// <summary>
    /// Attente de la connection des deux joueurs. 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public IEnumerator WaitForConnectionBothPlayers(int ID) {
        Debug.Log("On essaie"); 
        LobbyPlayer[] Player = FindObjectsOfType<LobbyPlayer>();
        while (Player.Length != 2) {
            yield return new WaitForSeconds(1f);
            Player = FindObjectsOfType<LobbyPlayer>();
        }
        Debug.Log("On essaie 2 "); 
        if (ID == 2) {
            yield return new WaitForSeconds(2f); 
        }
        // FindLobbyPlayer(ID).GetComponent<LobbyPlayer>().SetPlayerReady();
        // FindLobbyPlayer(ID).GetComponent<LobbyPlayer>().readyToBegin = true;
        for (int i = 0; i < 2; i++) {
            Player[i].SetPlayerReady(); 
        }

        
        Debug.Log("Player is ready"); 
    }

    /// <summary>
    /// Montrer au joueur que la recherche est en cours. 
    /// </summary>
    void MontrerRechercher() {
        GameObject.Find("MontrerText").GetComponent<Text>().text = 
            "Recherche de match en cours... "; 
    }

    /// <summary>
    /// Montrer au joueur qu'un match a été trouvé. 
    /// </summary>
    void MontrerMatchTrouve() {
        GameObject.Find("MontrerText").GetComponent<Text>().text =
            "Match trouvé! Connexion... ";
        StartCoroutine(HideMatchTrouve()); 
    }

    public IEnumerator HideMatchTrouve() {
        yield return new WaitForSeconds(3f);
        GameObject.Find("MontrerText").SetActive(false);
        
    }

    //public override void OnClientSceneChanged(NetworkConnection conn) {
    //    base.OnClientSceneChanged(conn);
    //}

}
