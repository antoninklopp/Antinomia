﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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