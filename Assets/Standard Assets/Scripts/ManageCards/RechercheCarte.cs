using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

/// <summary>
/// Attaché à l'inputText de recherche des cartes. 
/// </summary>
public class RechercheCarte : MonoBehaviour {

    /// <summary>
    /// Lorsqu'un utilisateur tape un nouveau charactere,
    /// on update les cartes. 
    /// </summary>
    public void NouveauCharactere() {
        // On ne fait pas cette recherche sur les portables, car on ne voit rien
        // quand il y a un inputField. 
#if !(UNITY_ANDROID || UNITY_IOS)
        string textSearch = GetComponent<InputField>().text;
        GameObject.Find("GameManager").GetComponent<GameManagerManageCards>().RechercheCarte(textSearch);
#endif
    }

    /// <summary>
    /// On fait la même chose à la fin de l'edition. 
    /// </summary>
    public void FinEdition() {
        string textSearch = GetComponent<InputField>().text;
        GameObject.Find("GameManager").GetComponent<GameManagerManageCards>().RechercheCarte(textSearch);
    }
}
