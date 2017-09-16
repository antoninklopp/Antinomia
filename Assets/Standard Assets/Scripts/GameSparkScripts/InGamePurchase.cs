using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using GameSparks.Core; 

public class InGamePurchase : MonoBehaviour {

	public int numberPackCards = 2; 
	public int PlayerMoney; 

	bool finish = false;

	public void FindNumberOfGoodsSparks(string type){
		/*
		 * Trouver le nombre de certains items que possède le joueur. 
		 */ 
		new GameSparks.Api.Requests.AccountDetailsRequest ()
			.Send ((response) => {
				if (!response.HasErrors){
					Debug.Log("Account Details Request Found ... "); 
					string PlayerName = response.DisplayName; 
					numberPackCards = (int) response.VirtualGoods.GetNumber(type); 
					Debug.Log(numberPackCards); 
					finish = true; 
				} else {
					throw new Exception("Les informations n'ont pas pu être récupérées"); 
				}
		}); 
	}

	public int GetNumberPackCards(){ 
		return numberPackCards; 
	}

	public IEnumerator WaitForNumberOfGoods(string type){
		FindNumberOfGoodsSparks (type); 
		while (!finish) {
			yield return new WaitForSeconds (0.05f); 	
			Debug.Log ("ah!"); 
		}
		// on met finish à false pour pouvoir recommencer une autre tâche plus tard. 
		finish = false; 
	}

	public void GetMoneyPlayer(){
		new GameSparks.Api.Requests.AccountDetailsRequest ()
			.Send ((response) => {
				if (!response.HasErrors){
					Debug.Log("Account Details Request Found ... "); 
					string PlayerName = response.DisplayName; 
					PlayerMoney = (int) response.Currencies.GetInt("Monnaie").Value; 
					Debug.Log("Money" + PlayerMoney.ToString()); 
					finish = true; 
				} else {
					throw new Exception("Les informations n'ont pas pu être récupérées"); 
				}
			}); 
	}

	public IEnumerator WaitForPlayerMoney(){
		GetMoneyPlayer (); 
		while (!finish) {
			yield return new WaitForSeconds (0.05f); 	
			Debug.Log ("ah!"); 
		}
		finish = false; 
	}

	public int GetMoney(){
		/*
		 * Récupérer la variable de la monnaie du player. 
		 */ 
		return PlayerMoney; 
	}

	public void BuyPaquetSimple(int number){
		/*
		 * Acheter un paquet de cartes (avec l'argent des abonnés)
		 */ 
		new GameSparks.Api.Requests.BuyVirtualGoodsRequest ()
			.SetCurrencyShortCode ("Monnaie")
			.SetQuantity (number)
			.SetShortCode ("paquet_simple")
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("Paquet acheté"); 
				} else {
					throw new Exception("Le paquet n'a pas pu être acheté"); 
				}
		}); 
	}
}
