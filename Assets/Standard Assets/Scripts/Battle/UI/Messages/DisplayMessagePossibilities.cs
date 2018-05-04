
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LanguageModule;
using UnityEngine.UI; 

public class DisplayMessagePossibilities : MonoBehaviourAntinomia {

    private GameObject EnsembleMessage;

    private GameObject CloseButton;

    private GameObject MontrerMessageLocal;

    private GameObject MontrerMessageNotLocal; 

	// Use this for initialization
	public override void Start () {
        base.Start(); 
        EnsembleMessage = GameObject.Find("EnsembleMessage");
        CloseButton = GameObject.Find("CloseMessage");
        MontrerMessageLocal = GameObject.Find("MontrerMessageLocal");
        MontrerMessageLocal.SetActive(false);
        MontrerMessageNotLocal = GameObject.Find("MontrerMessageNotLocal");
        MontrerMessageNotLocal.SetActive(false); 
        CloseMessages(); 
	}

    /// <summary>
    /// Montrer les messages possibles. 
    /// </summary>
    public void ShowMessages() {
        // Si c'est la première fois il faut créer les messages.
        EnsembleMessage.SetActive(true);
        CloseButton.SetActive(true); 
        if (EnsembleMessage.GetComponent<RectTransform>().childCount == 0) {
            for (int i = 1; i <= 6; i++) {
                // On instantie les 6 messages. 
                GameObject NewMessage = Instantiate(Resources.Load("Prefabs/MessagePrefab") as GameObject);
                NewMessage.transform.SetParent(EnsembleMessage.transform, false);
                Debug.Log("message" + i.ToString()); 
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
        FindLocalPlayer().GetComponent<Player>().SendMessageToPlayer(code);
        CloseMessages(); 
    }

    /// <summary>
    /// Fermer l'espace des messages.
    /// </summary>
    public void CloseMessages() {
        EnsembleMessage.SetActive(false);
        CloseButton.SetActive(false); 
    }

    /// <summary>
    /// Montrer un message envoyé par ce joueur ou par un autre.
    /// </summary>
    public void ShowMessageSent(bool LocalPlayer, int code) {
        StartCoroutine(ShowMessageSentRoutine(LocalPlayer, code)); 
    }

    /// <summary>
    /// On montre le message deux secondes au joueur.
    /// </summary>
    /// <param name="LocalPlayer"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    private IEnumerator ShowMessageSentRoutine(bool LocalPlayer, int code) {
        GameObject Display; 
        if (LocalPlayer) {
            Display = MontrerMessageLocal; 
        } else {
            Display = MontrerMessageNotLocal; 
        }

        Display.SetActive(true);
        Display.GetComponent<RectTransform>().Find("Text").gameObject.GetComponent<Text>().text = 
            LanguageData.GetString("message" + code.ToString(), "message"); 
        // On laisse le message affiché 2 secondes. 
        yield return new WaitForSeconds(2f);

        Display.SetActive(false); 
    }
}
