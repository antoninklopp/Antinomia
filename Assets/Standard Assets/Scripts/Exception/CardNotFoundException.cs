using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AntinomiaException {
    public class CardNotFoundException : Exception {

        public CardNotFoundException() {

        }

        public CardNotFoundException(string message) : base(message) {

        }

        /// <summary>
        /// Exception pour envoyer une information complète à la base de données pouvant être traitée. 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="globalInfo"></param>
        /// <param name="carte"></param>
        public CardNotFoundException(string message, GetGlobalInfoGameSparks globalInfo,
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
