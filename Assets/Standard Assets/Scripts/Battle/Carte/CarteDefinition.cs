
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

    public abstract class CarteDefinition {

        public virtual bool IsEntite() {
            return false;
        }

        public virtual bool IsSort() {
            return false;
        }

        public virtual bool IsAssistance() {
            return false;
        }
    }

}
