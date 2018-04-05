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
    /// <param name="previousAKA"></param>
    /// <param name="newAKA"></param>
    /// <param name="maxAKA"></param>
    public void ChangeCurrentAKA(int newAKA, int maxAKA) {
        float previousValue = GetComponent<Slider>().value; // Est-ce la bonne manière de récupérer l'ancienne valeur? 
        float nextValue = (float)newAKA / maxAKA; 
        StartCoroutine(ChangeCurrentAKARoutine(previousValue, nextValue)); 
    }

    private IEnumerator ChangeCurrentAKARoutine(float previousValue, float nextValue) {
        EffetParticules.SetActive(true); 


        // On fait durer l'animation 1 seconde. 
        for (int i = 0; i < 20; i++) {
            GetComponent<Slider>().value = previousValue * (1 - i/20f) + nextValue * (i/20f); 
            yield return new WaitForSeconds(0.05f); 
        }
        
        EffetParticules.SetActive(false);
    }


}
