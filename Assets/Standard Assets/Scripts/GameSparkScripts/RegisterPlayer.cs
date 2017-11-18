using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography; 

public class RegisterPlayer : MonoBehaviour{

	//public Text displayNameInput, userNameInput, passwordInput; 
	bool authorize = false;
	bool finish = false;

    [HideInInspector]
    public bool errorRegistrationName; 

    /// <summary>
    /// Créer un nouveau compte joueur dans la base de donnée
    /// </summary>
    /// <param name="displayName">Le nom d'utilisateur</param>
    /// <param name="userName">Le mot de passe utiilisateur</param>
    /// <param name="password">L'ancien nom du joueur (peut-être pour un changement de compte)</param>
	public void RegisterPlayerSetup(string displayName, string userName, string password){
		// register a new Player
		print ("Registering Player..."); 
		new GameSparks.Api.Requests.RegistrationRequest()
			.SetDisplayName(displayName)
			.SetUserName(userName)
			.SetPassword(password)
			.Send(( response ) => {
				if(!response.HasErrors){
					print("Player Registered \n User Name: " + response.DisplayName); 
				} else {
                    errorRegistrationName = true; 
					print("Error registering Player... \n " + response.Errors.JSON.ToString()); 
				}
			}); 
	}

	public void AuthorizePlayer(string userName, string password){
		// Authentification of a player
		print ("Authorizing Player..."); 
		new GameSparks.Api.Requests.AuthenticationRequest ()
			.SetUserName (userName)
			.SetPassword (password)
			.Send((response) => {
				if (!response.HasErrors){
					print("Player Authenticated ... \n User Name: " + response.DisplayName); 
					authorize = true; 
					if (!PlayerPrefs.HasKey("user")){
						PlayerPrefs.SetString("user", userName); 
					} 
					if (!PlayerPrefs.HasKey("password")){

					}
				} else {
					print("Error Authenticating Player ... \n " + response.Errors.JSON.ToString());
					authorize = false; 
				}
				finish = true; 
			}); 
	}

	public IEnumerator AuthorizePlayerRoutine(string userName, string password){
		AuthorizePlayer (userName, password);
		while (!finish) {
			yield return new WaitForSeconds (0.2f); 
		}
		finish = false; 
	}

    public IEnumerator RegisterPlayerRoutine(string userName, string password) {
        RegisterPlayerSetup(userName, password, userName);
        while (!finish) {
            yield return new WaitForSeconds(0.2f);
        }
        finish = false;
    }

	public bool getAuthorize(){
		return authorize; 
	}
//
//	public IEnumerator AuthorizePlayerRoutine2(string userName, string password){
//		/*
//		 * Attendre le résultat de la première coroutine
//		 */ 
//		yield return AuthorizePlayerRoutine (userName, password); 
//	}

//	public bool AuthorizePlayerFunction(string userName, string password){
//		StartCoroutine (AuthorizePlayerRoutine (userName, password)); 
//		Debug.Log (authorize); 
//		if (finish) {
//			return authorize;
//		} else {
//			Debug.Log ("merde"); 
//			return authorize;
//		}
//	}

	public void AuthenticateDevice(string displayName){
		// Authentification of the device
		print("Authenticating Device..."); 
		new GameSparks.Api.Requests.DeviceAuthenticationRequest ()
			.SetDisplayName (displayName)
			.Send (( response) => {
			
				if (!response.HasErrors){
					print("Device Authenticated..."); 
				} else {
					print("Error Anthenticating Device..."); 
				}
		}); 

	}
}
