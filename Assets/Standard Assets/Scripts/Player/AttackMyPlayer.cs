using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMyPlayer : MonoBehaviour {
	/*
	 * Script si on veut attaquer l'autre joueur. 
	 * 
	 */ 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown(){
		/*
		 * Lors d'un clique sur la "personnification" du joueur. 
		 * 
		 * Cette attaque sous-entend qu'il n'y a aucune entité sur le board ou le sanctuaire du joueur. 
		 * 
		 */ 
		GameObject.Find ("GameManager").SendMessage ("AttackOtherPlayer", gameObject.transform.parent.gameObject); 

	}
}
