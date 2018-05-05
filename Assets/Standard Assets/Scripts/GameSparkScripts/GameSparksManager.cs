
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.GameSparksScripts {

    public class GameSparksManager : MonoBehaviour {

        private static GameSparksManager instance = null;

        // Void to don't destroy on load. 
        void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
        }
    }

}
