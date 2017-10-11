using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO; 

/// <summary>
/// Permet de faire des tests et d'écrire les résultats dans des fichiers
/// </summary>
public class WriteOutputFile {

    public void WriteFileTestFunction(string functionName, string result) {
        StreamWriter writer = new StreamWriter("Assets/Resources/" + functionName + ".txt");
        writer.WriteLine(result + "\n"); 
    }

    public void WriteFileTestFunction(string functionName, int result) {
        StreamWriter writer = new StreamWriter("Assets/Resources/" + functionName + ".txt");
        writer.WriteLine(result.ToString() + "\n");
    }
}
