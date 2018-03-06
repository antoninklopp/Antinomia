
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AntinomiaException {
    public class UnusualBehaviourException : Exception {

        public UnusualBehaviourException() : base() {

        }

        public UnusualBehaviourException(string message) : base(message) {

        }

        public UnusualBehaviourException(string message, GetGlobalInfoGameSparks globalInfo,
            Carte carte) : this() {
            Debug.LogError(ToString() + carte.Name);
            globalInfo.ReportError(this,
                message + "\n" +
                carte.shortCode + "\n"
                );
        }

        public override string ToString() {
            return "Unusual behaviour Exception"; 
        }

    }

}
