
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement; 

namespace AntinomiaException {

    public class UnknownCardException : Exception {

        public UnknownCardException() : base() {

        }

        public UnknownCardException(string message) : base(message) {

        }

        /// <summary>
        /// Exception pour envoyer une information complète à la base de données pouvant être traitée. 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="globalInfo"></param>
        /// <param name="carte"></param>
        public UnknownCardException(string message, GetGlobalInfoGameSparks globalInfo,
            Carte carte) : this() {
            Debug.LogError(ToString() + carte.Name); 
            globalInfo.ReportError(this,
                message + "\n" + 
                carte.shortCode + "\n"
                ); 
        }

        public override string ToString() {
            return "Carte Inconnue"; 
        }
    }
}
