
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

public class Controller : MonoBehaviour{
	/*
	 * Partie controller de l'architecture MCV
	 * 
	 * Cette partie devrai être transférée sur un serveur. 
	 * 
	 */ 

	public string host, user, password; 
	public bool pooling = true; 

	private string database = "antinomia"; 

	private string connectionString; 
	private MySqlConnection connection = null; 

	void Awake(){
		//connectionString = "Server=" + host + ";Database=" + database + ";User=" + user + ";Password=" + password + ";Pooling=";
		//if (pooling) {
		//	connectionString += "true;"; 
		//} else {
		//	connectionString += "false;";
		//}
		//try{
		//	connection = new MySqlConnection(connectionString); 
		//	connection.Open(); 
		//	print("MySqlState " + connection.State.ToString()); 

		//} catch (Exception e){
		//	print (e); 
		//}
	}

	void OnApplicationQuit(){
		if (connection != null) {
			if (connection.State.ToString() != "Closed") {
				connection.Close ();
				print ("MySql connection closed"); 
			}
			connection.Dispose (); 
		}
	}

	void CloseConnection(){
		if (connection != null) {
			if (connection.State.ToString() != "Closed") {
				connection.Close ();
				print ("MySql connection closed"); 
			}
			connection.Dispose (); 
		}
	}

	public string GetConnectionState(){
		return connection.State.ToString();
	}

	bool OpenConnection(){
		// Ouverture de la connection avec la base de données. 
		connectionString = "Server=" + host + ";Database=" + database + ";User=" + user + ";Password=" + password + ";Pooling=true;";
		try{
			connection = new MySqlConnection(connectionString); 
			connection.Open(); 
			print("MySqlState " + connection.State.ToString()); 
			return true; 
		} catch (Exception e){
			print (e); 
			return false; 
		}

	}

	/*
	 * 
	 * Insérer un nouveau joueur dans la base de données. 
	 * 
	 * On y entre un triplé : login, password, playerID
	 * 
	 */ 
	public void InsertNewPlayer(string login,  string password){

		int newID = CreateNewPlayerID ();
		string query = "INSERT INTO users (login, password, playerID) VALUES('" + login +
			"', '" + password + "', '" + newID.ToString() + "')"; 

		// Open connection
		if (this.OpenConnection ()) {
			MySqlCommand cmd = new MySqlCommand (query, connection); 

			// Execute command
			cmd.ExecuteNonQuery();

			this.CloseConnection (); 
		}
	}

	public List<string> GetAllLogins(){
		/*
		 * Afin que deux joueurs n'aient pas les mêmes logins.
		 */ 

		string query = "SELECT * FROM users"; 

		// Create a list to store the result. 
		List<string> list = new List<string>(); 

		// open Connection
		if (this.OpenConnection ()) {
			MySqlCommand cmd = new MySqlCommand (query, connection); 
			//Create a data reader and execute the command
			MySqlDataReader dataReader = cmd.ExecuteReader (); 

			//Read the data and store them in the list
			while (dataReader.Read ()) {
				list .Add (dataReader ["login"] + ""); 
			}

			dataReader.Close (); 

			this.CloseConnection ();

			return list; 
		} else {
			return list; 
		}
	}

	public int CreateNewPlayerID(){
		/*
		 * Retourne l'ID suivant, lors de l'insertion d'un nouveau joueur dans la base de données. 
		 */ 
		string query = "SELECT * FROM users"; 

		int playerID = 0; 
		// Create a list to store the result. 
		List<int> list = new List<int>(); 

		// open Connection
		if (this.OpenConnection ()) {
			MySqlCommand cmd = new MySqlCommand (query, connection); 
			//Create a data reader and execute the command
			MySqlDataReader dataReader = cmd.ExecuteReader (); 

			//Read the data and store them in the list
			while (dataReader.Read ()) {
				list .Add (int.Parse(dataReader ["playerID"] + "")); 
			}

			dataReader.Close (); 

			this.CloseConnection ();

			playerID = list [list.Count - 1] + 1; 
		} else {
			throw new Exception ("ERROR : Probleme dans la base de données"); 
			//return 0; 
		}

		return playerID; 

	}

	public bool CheckIfNameInDataBase(string login){
		/*
		 * Regarder si le nom est dans une base de données. 
		 * 
		 * Retour: vrai si le nom est déjà dans la base de données, 
		 * faux sinon. 
		 */  
		List<string> allLogins = GetAllLogins (); 
		foreach (string player in allLogins){
			if (player == login) {
				return true; 
			}
		}
		return false; 
	}

	public int Login(string login, string password){
		/*
		 * Ici on vérifie si le joueur est dans la base de données. 
		 * 
		 * Si le joueur existe on retourne son playerID, qui permettre d'accéder à ses cartes, 
		 * sinon on retourne 0. 
		 * 
		 */ 

		string query = "SELECT * FROM users WHERE login='" + login + "' AND password='" + password + "'" ; 
		print (query); 
		// Create a list to store the result. 
		List<string>[] list = new List<string>[2]; 

		list[0] = new List<string>(); 
		list[1] = new List<string>(); 

		// open Connection
		if (this.OpenConnection ()) {
			MySqlCommand cmd = new MySqlCommand (query, connection); 
			//Create a data reader and execute the command
			MySqlDataReader dataReader = cmd.ExecuteReader (); 

			//Read the data and store them in the list
			while (dataReader.Read ()) {
				print (dataReader ["login"] + ""); 
				print (list [0]); 
				list [0].Add (dataReader ["login"] + ""); 
				list [1].Add (dataReader ["playerID"] + ""); 
			}

			dataReader.Close (); 

			this.CloseConnection ();

		} else {
			return 0; 
		}

		if (list[0].Count == 1) {
			return int.Parse (list [1][0]);
		} else {
			throw new Exception ("ERROR : PlayerID introuvanble"); 
			//return 0;
		}

	} 

}
