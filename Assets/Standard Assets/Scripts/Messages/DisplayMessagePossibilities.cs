using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LanguageModule;
using UnityEngine.UI; 

public class DisplayMessagePossibilities : MonoBehaviourAntinomia {

    private GameObject EnsembleMessage; 

	// Use this for initialization
	void Start () {
        EnsembleMessage = GameObject.Find("EnsembleMessage"); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown() {
        // Si c'est la première fois il faut créer les messages.
        EnsembleMessage.SetActive(true); 
        if (EnsembleMessage.GetComponent<RectTransform>().childCount == 0) {
            for (int i = 1; i <= 6; i++) {
                // On instantie les 6 messages. 
                GameObject NewMessage = Instantiate(Resources.Load("Prefabs/MessagePrefab") as GameObject);
                NewMessage.transform.SetParent(EnsembleMessage.transform, false);
                NewMessage.GetComponent<Message>().SetMessage(LanguageData.GetString("message" + i.ToString(), "message"));
                NewMessage.GetComponent<Message>().code = i; 
            }
        }
    }

    /// <summary>
    /// Envoyer un message à l'autre joueur.
    /// </summary>
    /// <param name="code"></param>
    public void SendMessageToPlayer(int code) {
        FindLocalPlayer().GetComponent<Player>().SendMessageToPlayer();
        CloseMessages(); 
    }

    /// <summary>
    /// Fermer l'espace des messages.
    /// </summary>
    public void CloseMessages() {
        EnsembleMessage.SetActive(false); 
    }

    /// <summary>
    /// Montrer un message envoyé par ce joueur ou par un autre.
    /// </summary>
    public void ShowMessageSent(bool LocalPlayer, int code) {

    }
}
