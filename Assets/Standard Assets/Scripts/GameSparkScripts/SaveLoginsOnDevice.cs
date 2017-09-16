using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO; 

public class SaveLoginsOnDevice {
	/*
	 * Sauvegarder le login et le mot de passe de l'utilisateur, 
	 * afin qu'il n'ait pas à s'authentifier à chaque fois.
	 * 
	 * On doit demander à l'utilisateur s'il veut qu'on se souvienne de son mot de passe et de son login ou pas. 
	 */ 

	public void SaveLoginInfos(string user, string password){
		/*
		 * Sauvegarder les infos de login. 
		 */ 
		BinaryFormatter bf = new BinaryFormatter (); 
		FileStream file = File.Create (Application.persistentDataPath + "/logins.dat"); 

		Logins _login = new Logins (); 
		_login.login = user; 
		_login.password = password; 

		bf.Serialize (file, _login); 
		file.Close (); 
	}

	public bool isOnePlayerAuthenticated(){
		/*
		 * Savoir si un joueur est authetifié sur l'appareil. 
		 */ 
		if (File.Exists (Application.persistentDataPath + "/logins.dat")) {
			return true; 
		} else {
			return false; 
		}
	}

	public List<string> getLogin(){
		/*
		 * Récuperer le login de l'utilisateur et le mot de passe sur l'appareil
		 */ 
		List<string> userAndPassword = new List<string> (); 
		if (File.Exists (Application.persistentDataPath + "/logins.dat")) {
			BinaryFormatter bf = new BinaryFormatter (); 
			FileStream file = File.Open (Application.persistentDataPath + "/logins.dat", FileMode.Open); 
			Logins _login = (Logins)bf.Deserialize (file); 
			file.Close (); 

			userAndPassword.Add (_login.login); 
			userAndPassword.Add (_login.password); 
		}
		return userAndPassword; 
	}

	public void DisconnectPlayer(){
		/*
		 * Déconnecter un joueur. 
		 */ 
		File.Delete(Application.persistentDataPath + "/logins.dat"); 
	}
}

[Serializable]
class Logins
{
	public string login; 
	public string password; 
}
