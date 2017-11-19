using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class GameManagerLoginUser : MonoBehaviour {

	private GameObject InputFieldLogin; 
	private GameObject InputFieldPassword; 
	private GameObject Result; 

	// Est-ce que l'utilisateur veut que l'appareil se souvienne de ses logins.
#pragma warning disable CS0414 // Le champ 'GameManagerLoginUser.rememberLogins' est assigné, mais sa valeur n'est jamais utilisée
	private bool rememberLogins = true;
#pragma warning restore CS0414 // Le champ 'GameManagerLoginUser.rememberLogins' est assigné, mais sa valeur n'est jamais utilisée
	// Est-ce qu'il y a déjà des logins sur l'appareil. 
#pragma warning disable CS0414 // Le champ 'GameManagerLoginUser.loginsOnDevice' est assigné, mais sa valeur n'est jamais utilisée
	private bool loginsOnDevice = false; 
#pragma warning restore CS0414 // Le champ 'GameManagerLoginUser.loginsOnDevice' est assigné, mais sa valeur n'est jamais utilisée

	// Use this for initialization
	void Start () {
		Result = GameObject.Find ("Result"); 
		InputFieldLogin = GameObject.Find ("Login");
		InputFieldPassword = GameObject.Find ("Password"); 

		SaveLoginsOnDevice _save = new SaveLoginsOnDevice (); 
		if (_save.isOnePlayerAuthenticated()) {
			List<string> _logins = _save.getLogin (); 
			StartCoroutine (OnClickResultRoutine (_logins [0], _logins [1])); 
			loginsOnDevice = true; 
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClickResult(){
        Result.transform.GetChild(0).gameObject.GetComponent<Text>().text = "TRY TO CONNECT"; 
        string login = InputFieldLogin.GetComponent<InputField> ().text; 
		string password = InputFieldPassword.GetComponent<InputField> ().text; 
		StartCoroutine (OnClickResultRoutine (login, password)); 
	}

    public void CreateAccount() {
        SceneManager.LoadScene("Registration");
    }

    public IEnumerator OnClickResultRoutine(string login, string password){
		//DataBaseLogin baseLogin = gameObject.GetComponent<DataBaseLogin> (); 
//		if (baseLogin.LoginUser (login, password) != 0) {
//			Result.transform.GetChild (0).gameObject.GetComponent<Text> ().text = baseLogin.LoginUser (login, password).ToString (); 
//		} else {
//			Result.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "ERREUR"; 
//		}

		RegisterPlayer register = gameObject.GetComponent<RegisterPlayer>(); 
		//bool canAccess = register.AuthorizePlayerFunction (login, password); 
		yield return new WaitForSeconds (0.1f);

#if UNITY_EDITOR
        // Dans l'editor on fait de device le compte test. 
        yield return register.AuthorizePlayerRoutine("device", "device");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
#else
		yield return register.AuthorizePlayerRoutine (login, password); 
		bool canAccess = register.getAuthorize (); 
		Debug.Log (canAccess); 
		if (canAccess) {
            //GetPlayerInfoGameSparks playerInfo = gameObject.GetComponent<GetPlayerInfoGameSparks> (); 
            yield return new WaitForSeconds (0.5f); 
			PlayerPrefs.SetString ("user", login); 
			Result.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "CONNECTED"; 
			if (!loginsOnDevice && rememberLogins) {
				SaveLoginsOnDevice _save = new SaveLoginsOnDevice (); 
				_save.SaveLoginInfos (login, password);
            }
            SceneManager.LoadScene("MainMenu");
        } else {
			Result.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "IMPOSSIBLE"; 
		}  
#endif
    }
}
