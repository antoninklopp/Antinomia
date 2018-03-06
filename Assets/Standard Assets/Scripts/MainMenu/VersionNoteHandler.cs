
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

/// <summary>
/// Objet qui gère les notes de versions dans le Main Menu. 
/// If faut qu'il lui soit associé un composant PlayerInfo. 
/// </summary>
public class VersionNoteHandler : MonoBehaviour {

    private GetGlobalInfoGameSparks playerInfo; 

	// Use this for initialization
	void Start () {

        playerInfo = GetComponent<GetGlobalInfoGameSparks>();
        StartCoroutine(GetVersion()); 
	}
	
	private IEnumerator GetVersion() {
        yield return playerInfo.WaitForVersionNote();
        VersionNote lastVersionNote = playerInfo.GetVersionNote();

        if (PlayerPrefs.HasKey("VersionNumber" + lastVersionNote.VersionNumber)) {
            // Alors le joueur a déjà vu la note de version
            gameObject.SetActive(false);
        } else {
            GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            transform.Find("VersionNumber").gameObject.GetComponent<Text>().text = "Version Number : " +
                lastVersionNote.VersionNumber;
            transform.Find("VersionInformation").gameObject.GetComponent<Text>().text = "Version Information : " +
                lastVersionNote.VersionInformation;

            PlayerPrefs.SetString("VersionNumber" + lastVersionNote.VersionNumber, "OK"); 
        }

        if (lastVersionNote.UpdateNecessary) {
            // TODO : Bloquer le jeu, forcer à faire l'update
        } 
    }

    /// <summary>
    /// Fermer la note de version depuis le bouton 
    /// </summary>
    public void CloseVersionNote() {
        gameObject.SetActive(false); 
    }
}
