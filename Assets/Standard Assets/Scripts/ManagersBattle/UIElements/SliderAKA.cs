using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

/// <summary>
/// Slider de gestion des effets de l'AKA. 
/// </summary>
public class SliderAKA : MonoBehaviour {

    private GameObject EffetParticules;

    public void Start() {

        // On réveille l'effet de particules; 
        if (EffetParticules == null) {
            EffetParticules = transform.Find("Particle").gameObject;
        }

        EffetParticules.SetActive(false); 
    }

    /// <summary>
    /// Changer la valeur de l'AKA courant avec une animation. 
    /// </summary>
    /// <param name="newTurn">Si nouveau tour, on montre la barre qui se charge de 0. </param>
    public void ChangeCurrentAKA(int newAKA, int maxAKA, bool newTurn=false) {
        if (maxAKA == 0) {
            GetComponent<Slider>().value = 1;
            return; 
        }

        float previousValue = GetComponent<Slider>().value; // Est-ce la bonne manière de récupérer l'ancienne valeur? 
        float nextValue = (float)newAKA / maxAKA; 
        StartCoroutine(ChangeCurrentAKARoutine(previousValue, nextValue, newTurn)); 
    }

    private IEnumerator ChangeCurrentAKARoutine(float previousValue, float nextValue, bool newTurn=false) {
        // Si c'est un nouveau tour et que la valeur de l'AKA est de 0, aucune utilité
        // à montrer un effet de chargement. 
        if (newTurn && nextValue == 0) {
            yield break; 
        }

        // Sinon on recharge la barre en entier.
        if (newTurn) {
            previousValue = 0; 
        }

        EffetParticules.SetActive(true);


        // On fait durer l'animation 1 seconde. 
        for (int i = 0; i < 20; i++) {
            GetComponent<Slider>().value = previousValue * (1 - i/20f) + nextValue * (i/20f); 
            yield return new WaitForSeconds(0.05f); 
        }
        
        EffetParticules.SetActive(false);
    }


}
