using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks; 
using GameSparks.Core; 

public class MatchMakingGameSparks : MonoBehaviour {
    /*
     * Matchmaking automatique en utilisant GameSparks. 
     * 
     */ 

     // On passe startMatch à true quand un match a été trouvé. 
    bool startMatch = false;
    // On crée aussi un boolean pour la réponse à la requête envoyée au serveur lors de l'update du statut dans la 
    // queue du matchmaking. 
    bool responseUpdate = false;
    int gameID = 0; 


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MultiplayerMatchRequest(long skill){
		/*
		 * Faire une requête de matchmaking. 
		 */ 
		new GameSparks.Api.Requests.MatchmakingRequest ()
			.SetMatchShortCode ("Game")
			.SetMatchGroup ("group1")
			.SetSkill (skill)
			.Send((response) => {
				if (!response.HasErrors){
                    //List<GSData> participants; 
                    //if (response.ScriptData.GetGSDataList("particpants") != null) {
                    //    // On récupère la liste des participants. 
                    //    participants = response.ScriptData.GetGSDataList("particpants");
                    //    // On récupère le nom du deuxième joueur.
                    //    // Comme ça, on le garde et on n'aura pas à l'envoyer à l'autre joueur, au début de la game. 
                    //    // TODO : Il faudra peut-être vérifier que les joueurs ont le statut online
                    //    if (PlayerPrefs.GetString("user") == participants[0].GetString("displayName")) {
                    //        // Si le premier joueur est celui qui est connecté sur le device, 
                    //        // on met le deuxième joueur dans le PlayerPrefs user2
                    //        PlayerPrefs.SetString("user2", participants[1].GetString("displayName")); 
                    //    } else {
                    //        PlayerPrefs.SetString("user2", participants[0].GetString("displayName")); 

                    //    }

                    //} else {
                    //    // Les participants ne sont pas encore in-game
                    //    return; 
                    //}
                    Debug.Log("Player en attente de game"); 
				} else {
					Debug.Log("On n'a pu ajouter le joueur au matchmaking"); 
				}
		}); 
	}

	public void UpdateMatchRequest(){
        /*
         * Avant l'update il faut avoir appelé une multiplayer request.
         */ 
        new GameSparks.Api.Requests.UpdateMessageRequest()
            .Send((response) => {
                    if (!response.HasErrors) {
                        List<GSData> participants;
                        if (response.ScriptData.GetGSDataList("particpants") != null) {
                            // On récupère la liste des participants. 
                            participants = response.ScriptData.GetGSDataList("particpants");
                            // On récupère le nom du deuxième joueur.
                            // Comme ça, on le garde et on n'aura pas à l'envoyer à l'autre joueur, au début de la game. 
                            // TODO : Il faudra peut-être vérifier que les joueurs ont le statut online
                            if (PlayerPrefs.GetString("user") == participants[0].GetString("displayName")) {
                                // Si le premier joueur est celui qui est connecté sur le device, 
                                // on met le deuxième joueur dans le PlayerPrefs user2
                                PlayerPrefs.SetString("user2", participants[1].GetString("displayName"));
                            }
                            else {
                                PlayerPrefs.SetString("user2", participants[0].GetString("displayName"));

                            }
                            startMatch = true;
                            gameID = response.ScriptData.GetInt("gameId").Value; 

                        }

                    }
                    else {
                        Debug.Log(response.JSONData); 
                        Debug.Log("Pas d'update reçue.");
                    }
          }); 
	}

    IEnumerator WaitForMatchUpdate() {
        UpdateMatchRequest(); 
        while (!responseUpdate) {
            yield return new WaitForSeconds(0.05f);
        }
    }

    public bool returnMatchUpdate() {
        /*
         * Retourner l'état de la position du joueur dans le matchmaking
         * True => matchmaking trouvé
         * False => matchmaking non trouvé. 
         */
        return startMatch; 
    }

    public int GameIDInfo() {
        /*
         * Lorsque le joueur est accepté dans un match, on récupère l'ID du match pour pouvoir le connecter "à la main"
         * depuis Unity. 
         */
        return gameID; 
    }

    public void updatePlayerGameCount() {
        /*
         * Pour garder une trace du nombre de games faites par le joueur sur le serveur, 
         * on update son nombre de games jouées sur le serveur. 
         * 
         */
        new GameSparks.Api.Requests.LogEventRequest()
           .SetEventKey("enterNewGame")
           .Send((response) => {
               if (!response.HasErrors) {
                   Debug.Log("Le nombre de games du joueur a été augmentée");
                   // Debug.Log("Nombre : " + response.ScriptData.GetString("gameNumber")); 
               } else {
                   Debug.Log("Le nombre de games du joueur n'a pas pu être augmentée"); 
               }
           }); 
    }
}
