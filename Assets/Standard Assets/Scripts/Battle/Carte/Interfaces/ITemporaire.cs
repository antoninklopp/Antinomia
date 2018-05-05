
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

    /// <summary>
    /// Interface d'une classe temporaire. 
    /// </summary>
    public interface ITemporaire {

        /// <summary>
        /// Voir si la carte est temporaire. 
        /// </summary>
        /// <returns>True si la carte est temporaire, false sinon. </returns>
        bool EstTemporaire();

    }

}
