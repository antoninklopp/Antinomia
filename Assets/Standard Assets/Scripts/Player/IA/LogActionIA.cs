using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 

/// <summary>
/// Log des actions de l'IA
/// </summary>
public class LogActionIA {

    int nombreFichier = 0;
    StreamWriter writer;
    string path = "Assets/Resources/IALog/";
    string filename = "logIA"; 
    
	public void InitializeWriter () {
        if (PlayerPrefs.HasKey("NombreFichierLogIA")) {
            nombreFichier = PlayerPrefs.GetInt("NombreFichierLogIA");
            PlayerPrefs.SetInt("NombreFichierLogIA", nombreFichier + 1); 
        } else {
            PlayerPrefs.SetInt("NombreFichierLogIA", 1); 
        }

        writer = new StreamWriter(path + filename + nombreFichier.ToString() + ".txt");
        writer.WriteLine("Log de la partie " + nombreFichier.ToString()); 
	}
	


    public void addLog(string newLogLine) {
        writer.WriteLine(newLogLine); 
    }

}
