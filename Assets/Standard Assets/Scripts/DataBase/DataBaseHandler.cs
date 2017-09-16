using System.Collections;
using System.Collections.Generic;
using System; 
using System.Data; 
using MySql.Data; 
using MySql.Data.MySqlClient; 
using System.Text; 
using System.Security.Cryptography; 
using UnityEngine;

public class DataBaseHandler : MonoBehaviour {
	/*
	 * TODO:
	 * Reregarder le tuto et regarder comment faire un back up intelligent des données. 
	 * 
	 * Il faudra sûrement synchroniser la base de données avec un fichier xml sur l'appareil, afin de
	 * ne pas avoir à se connecter à la base de données tout le temps. 
	 * 
	 */ 
	public string host, user, password; 
	public bool pooling = true; 

	private string database = "antinomia"; 

	private string connectionString; 
	private MySqlConnection connection = null; 
	//private MySqlCommand cmd = null; 
	//private MySqlDataReader rdr = null; 

#pragma warning disable CS0169 // Le champ 'DataBaseHandler.md5hash' n'est jamais utilisé
	private MD5 md5hash; 
#pragma warning restore CS0169 // Le champ 'DataBaseHandler.md5hash' n'est jamais utilisé

	void Awake(){
		DontDestroyOnLoad (this.gameObject); 
		connectionString = "Server=" + host + ";Database=" + database + ";User=" + user + ";Password=" + password + ";Pooling=";
		if (pooling) {
			connectionString += "true;"; 
		} else {
			connectionString += "false;";
		}
		try{
			connection = new MySqlConnection(connectionString); 
			connection.Open(); 
			print("MySqlState " + connection.State.ToString()); 
		
		} catch (Exception e){
			print (e); 
		}
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
	 * Insérer une nouvelle carte dans la base de données. 
	 * 
	 * La carte n'a que deux informations: l'ID (identifiant entier naturel unique par carte), 
	 * et le nombre de cartes de ce type possédé par l'utilisateur. 
	 * 
	 * On doit donc avoir un triplé (IDJoueur, IDCarte, nombre). 
	 * 
	 */ 
	public void InsertCarte(int IDJoueur,  int IDCarte, int nombre){
		string query = "INSERT INTO cartes (IDJoueur, IDCarte, nombre) VALUES('" + IDJoueur.ToString () +
		               "', '" + IDCarte.ToString () + "', '" + nombre.ToString () + "')"; 

		// Open connection
		if (this.OpenConnection ()) {
			MySqlCommand cmd = new MySqlCommand (query, connection); 

			// Execute command
			cmd.ExecuteNonQuery();

			this.CloseConnection (); 
		}
	}

	/*
	 * 
	 * Changer une valeur dans la base de données des cartes
	 * Ici rajouter une carte. 
	 * 
	 * Pas de void de delete pour l'instant!
	 * 
	 */ 
	public void UpdateCarte(int IDJoueur, int IDCarte, int oldNombre, int newNombre){
		string query = "UPDATE cartes SET IDJoueur='" + IDJoueur.ToString () + "', IDCarte='" +
		               IDCarte.ToString () + "', nombre='" + newNombre.ToString () + "' WHERE nombre='" + oldNombre.ToString () + "'"; 

		if (this.OpenConnection ()) {
			MySqlCommand cmd = new MySqlCommand (query, connection); 

			cmd.CommandText = query;

			cmd.Connection = connection; 

			cmd.ExecuteNonQuery (); 

			this.CloseConnection (); 
		}
	}

	public List<int>[] Select(){


		string query = "SELECT * FROM cartes"; 

		// Create a list to store the result. 
		List<int>[] list = new List<int>[4]; 
		list [0] = new List<int> (); 
		list [1] = new List<int> (); 
		list [2] = new List<int> ();
		list [3] = new List<int> ();

		// open Connection
		if (this.OpenConnection ()) {
			MySqlCommand cmd = new MySqlCommand (query, connection); 
			//Create a data reader and execute the command
			MySqlDataReader dataReader = cmd.ExecuteReader (); 

			//Read the data and store them in the list
			while (dataReader.Read ()) {

				list [0].Add (int.Parse (dataReader ["id"] + "")); 
				list [1].Add (int.Parse (dataReader ["IDJoueur"] + "")); 
				list [2].Add (int.Parse (dataReader ["IDCarte"] + "")); 
				list [3].Add (int.Parse (dataReader ["nombre"] + "")); 

			}

			dataReader.Close (); 

			this.CloseConnection ();

			return list; 
		} else {
			return list; 
		}
	}

	public List<int>[] SelectCarteFromJoueur(int playerID){

		string query = "SELECT * FROM cartes WHERE IDJoueur='" + playerID.ToString() + "'"; 

		// Create a list to store the result. 
		List<int>[] list = new List<int>[4]; 
		list [0] = new List<int> (); 
		list [1] = new List<int> (); 
		list [2] = new List<int> ();
		list [3] = new List<int> ();

		// open Connection
		if (this.OpenConnection ()) {
			MySqlCommand cmd = new MySqlCommand (query, connection); 
			//Create a data reader and execute the command
			MySqlDataReader dataReader = cmd.ExecuteReader (); 

			//Read the data and store them in the list
			while (dataReader.Read ()) {

				list [0].Add (int.Parse (dataReader ["id"] + "")); 
				list [1].Add (int.Parse (dataReader ["IDJoueur"] + "")); 
				list [2].Add (int.Parse (dataReader ["IDCarte"] + "")); 
				list [3].Add (int.Parse (dataReader ["nombre"] + "")); 

			}

			dataReader.Close (); 

			this.CloseConnection ();

			return list; 
		} else {
			return list; 
		}


	}
}
