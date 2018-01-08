﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using System;

public class GetGlobalInfoGameSparks : MonoBehaviour {

    /// <summary>
    /// Note de version. 
    /// </summary>
    VersionNote _VersionNote;

    /// <summary>
    /// Commentaire OK dit si le commentaire a bien été envoyé. 
    /// -1 si le commentaire n'a pas réussi à être envoyé
    /// 0 si le commentaire n'a pas encore été soumis
    /// 1 si l'envoie a réussi. 
    /// </summary>
    int commentaireOK = 0;

    bool finish = false; 

    /// <summary>
    /// Récupérer la note de version (et le numéro de la version) 
    /// depuis la bdd GameSparks. 
    /// </summary>
    /// <param name="start">true si on est au début du jeu (si le joueur a déjà lancé le jeu depuis
    /// la nouvelle version, on ne lui montrera pas la note de version), false sinon (le joueur veut
    /// explicitement voir la note de version).</param>
    public void LoadVersionNote(bool start=true) {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("getLastVersionNote")
            .Send((response) => {

                if (!response.HasErrors) {
                    _VersionNote = new VersionNote(
                        response.ScriptData.GetString("VersionNumber"),
                        response.ScriptData.GetString("VersionNote"),
                        response.ScriptData.GetString("UpdateNecessary")
                    ); 
                    finish = true; 
                }
            }); 
    }

    /// <summary>
    /// Attente des notes de version. 
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    public IEnumerator WaitForVersionNote(bool start=true) {
        LoadVersionNote(start);
        while(!finish) {
            yield return new WaitForSeconds(0.05f); 
        }
        finish = false; 
    }

    /// <summary>
    /// Récupérer la note de version
    /// </summary>
    /// <returns>note de la version</returns>
    public VersionNote GetVersionNote() {
        return _VersionNote; 
    }

    /// <summary>
    /// Récupérer le numéro de la version. 
    /// </summary>
    /// <returns>Numéro de la dernière version.</returns>
    public string getVersionNumber() {
        return VersionNumber; 
    }

    /// <summary>
    /// Si un joueur de la beta veut faire un commentaire. 
    /// </summary>
    /// <param name="comment">Le texte du commentaire.</param>
    private void MakeABetaComment(string comment) {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("addACommentBeta")
            .SetEventAttribute("name", PlayerPrefs.GetString("user"))
            .SetEventAttribute("comment", comment)
            .Send((response) => {
                if (!response.HasErrors) {
                    Debug.Log("Le commentaire a bien été ajouté");
                    commentaireOK = 1; 
                } else {
                    Debug.Log("Probleme lors de l'ajout du commentaire");
                    commentaireOK = -1; 
                }
            }); 
    }

    public IEnumerator WaitForComment(string comment) {
        MakeABetaComment(comment); 
        while(commentaireOK == 0) {
            yield return new WaitForSeconds(0.1f); 
        }
    }

    public int getCommentaireOK() {
        return commentaireOK;
    }
}
