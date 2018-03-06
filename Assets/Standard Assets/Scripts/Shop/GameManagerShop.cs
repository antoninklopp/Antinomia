
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GameManagerShop : MonoBehaviour {
	/*
	 * Le gameManager de la scene de la boutique, ou il sera possible d'acheter des cartes et de les ouvrir? 
	 * 
	 */ 
	GameObject paquetSimple;
	GameObject MoneyUI; 

	InGamePurchase purchase; 
	int numberPaquets; 
	int money; 

	// Use this for initialization
	void Start () {
		paquetSimple = GameObject.Find ("Paquet_simple"); 
		MoneyUI = GameObject.Find ("Monnaie"); 
		purchase = GetComponent<InGamePurchase> (); 
		StartCoroutine (SetNumberOfPaquetsSimples ()); 
		StartCoroutine(SetMoney ()); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator SetNumberOfPaquetsSimples(){
		yield return purchase.WaitForNumberOfGoods("paquet_simple"); 
		numberPaquets = purchase.GetNumberPackCards (); 
		paquetSimple.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "Nombre : " + numberPaquets.ToString (); 
		Debug.Log (numberPaquets); 
	}

	IEnumerator SetMoney(){
		yield return purchase.WaitForPlayerMoney (); 
		money = purchase.GetMoney (); 
		MoneyUI.GetComponent<Text> ().text = "Monnaie : " + money.ToString (); 
	}

	public void BuyAPaquet(){
		StartCoroutine (AchatPaquet (1)); 
	}

	IEnumerator AchatPaquet(int number){
		/*
		 * Acheter des paquets
		 * 
		 */ 
		purchase.BuyPaquetSimple (number); 
		yield return new WaitForSeconds (0.1f); 
		StartCoroutine (SetMoney ()); 
		StartCoroutine (SetNumberOfPaquetsSimples ());
	}
}
