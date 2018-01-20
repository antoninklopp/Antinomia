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
    }
}
