using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

/// <summary>
/// Utile lors de la première scène du jeu.
/// </summary>
public class Introduction : MonoBehaviour {

	GameObject Antinomia; 

	// Use this for initialization
	void Start () {
		Antinomia = GameObject.Find ("Antinomia"); 
		// StartCoroutine (FadeName ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator FadeName(){
		/*
		 * Petit effet au lancement du jeu. 
		 */ 

		Color colorBase = Color.white; 
		colorBase.a = 1; 
		print ("ok"); 
		for (float i = 0f; i < 1f; i += 0.01f) {
			Antinomia.GetComponent<Text> ().color = Color.Lerp (Antinomia.GetComponent<Text> ().color, colorBase, i);
			yield return new WaitForSeconds (0.05f); 
		}

		colorBase.a = 0; 
		for (float i = 0f; i < 1f; i += 0.01f) {
			Antinomia.GetComponent<Text> ().color = Color.Lerp (Antinomia.GetComponent<Text> ().color, colorBase, i); 
			yield return new WaitForSeconds (0.05f); 
		}

		SceneManager.LoadScene ("Log"); 
	}
}
