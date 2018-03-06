
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using System; 
using System.Data; 
using MySql.Data; 
using MySql.Data.MySqlClient; 
using System.Text; 
using System.Security.Cryptography; 
using UnityEngine;

public class DataBaseLogin : MonoBehaviour {
	/*
	 * Partie Modèle de l'architecture MCV, c'est ici qu'on demande et qu'on récupère les données!
	 * 
	 */ 

	Controller _myController;

	void OnStart(){

		_myController = gameObject.GetComponent<Controller> (); 

	} 

	/*
	 * 
	 * Insérer un nouveau joueur dans la base de données. 
	 * 
	 * On y entre un triplé : login, password, playerID
	 * 
	 */ 
	public void InsertNewPlayerUser(string login,  string password){
		
		_myController.InsertNewPlayer (login, password);

	}

	public int CreateNewPlayerIDUser(){
		/*
		 * Retourne l'ID suivant, lors de l'insertion d'un nouveau joueur dans la base de données. 
		 */ 
	
		return _myController.CreateNewPlayerID (); 

	}

	public bool CheckIfNameInDataBaseUser(string login){
		/*
		 * Regarder si le nom est dans une base de données. 
		 * 
		 * Retour: vrai si le nom est déjà dans la base de données, 
		 * faux sinon. 
		 */  

		return _myController.CheckIfNameInDataBase (login); 

	}

	public int LoginUser(string login, string password){
		/*
		 * Ici on vérifie si le joueur est dans la base de données. 
		 * 
		 * Si le joueur existe on retourne son playerID, qui permettre d'accéder à ses cartes, 
		 * sinon on retourne 0. 
		 * 
		 */ 

		return _myController.Login (login, password); 

	} 
}
