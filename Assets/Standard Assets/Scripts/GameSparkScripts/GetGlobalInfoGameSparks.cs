using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using System;

public class GetGlobalInfoGameSparks : MonoBehaviour {

    /// <summary>
    /// Note de version. 
    /// </summary>
    string VersionNote = "";

    /// <summary>
    /// Numéro de la version. 
    /// </summary>
    string VersionNumber; 

    /// <summary>
    /// Récupérer la note de version (et le numéro de la version) 
    /// depuis la bdd GameSparks. 
    /// </summary>
    /// <param name="start">true si on est au début du jeu (si le joueur a déjà lancé le jeu depuis
    /// la nouvelle version, on ne lui montrera pas la note de version), false sinon (le joueur veut
    /// explicitement voir la note de version).</param>
    public void GetVersionNote(bool start=true) {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("getLastVersionNote")
            .Send((response) => {

                if (!response.HasErrors) {
                    VersionNumber = response.ScriptData.GetString("VersionNumber"); 
                    // Si le joueur a déjà vu cette note truede version.
                    if (PlayerPrefs.HasKey(VersionNumber) && !start){
                        VersionNote = "None"; 
                    } else {
                        VersionNote = response.ScriptData.GetString("VersionNote");
                        PlayerPrefs.SetString(VersionNumber, "true"); 
                    }
                }
            }); 
    }

    public IEnumerator WaitForVersionNotes(bool start=true) {
        GetVersionNote(start);
        while(VersionNote == "") {
            yield return new WaitForSeconds(0.05f); 
        }
    }

    /// <summary>
    /// Récupérer la note de version
    /// </summary>
    /// <returns>note de la version</returns>
    public string getVersionNote() {
        return VersionNote; 
    }

    /// <summary>
    /// Récupérer le numéro de la version. 
    /// </summary>
    /// <returns>Numéro de la dernière version.</returns>
    public string getVersionNumber() {
        return VersionNumber; 
    }
}
