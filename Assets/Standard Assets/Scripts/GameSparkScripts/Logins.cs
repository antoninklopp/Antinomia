using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Antinomia.GameSparksScripts {

    [Serializable]
    public class Logins {
        public string login;
        public string password;

        public Logins() {
        }

        public Logins(string login, string password) {
            this.login = login;
            this.password = password;
        }
    }

}
