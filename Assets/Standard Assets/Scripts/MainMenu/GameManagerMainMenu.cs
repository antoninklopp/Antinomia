using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 
#if UNITY_EDITOR
using UnityEditor; 
using UnityEditor.Callbacks;
#endif

public class GameManagerMainMenu : MonoBehaviour {
	/*
	 * GameManager du MainMenu
	 * 
	 */ 

	public bool choixDeck = false; 
	GameObject BuildVersion; 

	// Use this for initialization
	void Start () {
		BuildVersion = GameObject.Find ("BuildVersion");
#if UNITY_EDITOR
        BuildVersion.GetComponent<Text>().text = "Debug Version : device" + "\nBuild version : " + Application.version;
#else
        BuildVersion.GetComponent<Text> ().text = "Connected as : " + 
            PlayerPrefs.GetString("user") + "\nBuild version : " + Application.version; 
#endif
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GoToManageCards(){
		/*
		 * Changement de scene pour le management des cartes
		 */ 

		SceneManager.LoadScene ("ManageCards"); 
	}

	public void GoToLobby(){
		/*
		 * Changement de scène pour aller au lobby
		 */ 
		if (choixDeck) {
			SceneManager.LoadScene ("ChoixDeck");
		} else {
			PlayerPrefs.SetInt ("ChoixDeck", 0); 
			SceneManager.LoadScene ("SimpleLobby"); 
		}
	}

	public void GoToShop(){
		/*
		 * Changement de scène pour le magasin!
		 */ 
		SceneManager.LoadScene ("Shop"); 
	}

	public void Disconnect(){
		/*
		 * Deconnexion du joueur. 
		 */ 
		SaveLoginsOnDevice save = new SaveLoginsOnDevice (); 
		save.DisconnectPlayer (); 
		Application.Quit (); 
	}
}
