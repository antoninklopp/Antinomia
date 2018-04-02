using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

/// <summary>
/// 6 possibilités de messages
/// </summary>
public class Message : MonoBehaviour {

    /// <summary>
    /// Chaque message a un code, moins lourd à envoyer par le réseau. 
    /// </summary>
    public int code; 

    public void OnMouseDown() {
        transform.parent.gameObject.GetComponent<DisplayMessagePossibilities>().SendMessageToPlayer(code); 
    }

    /// <summary>
    /// Aggicher le message sur le Text UI. 
    /// </summary>
    /// <param name="m"></param>
    public void SetMessage(string m) {
        GetComponent<Text>().text = m; 
    }
}
