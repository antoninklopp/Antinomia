using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation des cartes avec des shaders et des changements de position. 
/// </summary>
public class CarteAnimation : MonoBehaviour {

    /// <summary>
    /// Animation de l'apparition de la carte
    /// </summary>
    public void AnimationDebut() {
        StartCoroutine(AnimationEntreeRoutine(20)); 
    }

	private IEnumerator AnimationEntreeRoutine(int time) {
        Material NewMaterial = Instantiate(Resources.Load("Material/DisolveMaterial") as Material) as Material;
        Debug.Log(NewMaterial);
        GetComponent<SpriteRenderer>().material = NewMaterial;

        // Appear
        int i = 0;
        while (i < time) {
            NewMaterial.SetFloat("_Level", 1 - (float)i / time);
            yield return new WaitForSeconds(0.05f);
            i++;
        }
    }

    /// <summary>
    /// Animation de la disparition de la carte
    /// </summary>
    public void AnimationFin() {
        StartCoroutine(AnimationFinRoutine(20)); 
    }

    private IEnumerator AnimationFinRoutine(int time) {
        GameObject Particle = Instantiate(Resources.Load("Particles/ParticleDeath") as GameObject);
        Particle.transform.position = transform.position;

        // Disolve
        Material NewMaterial = Instantiate(Resources.Load("Material/DisolveMaterial") as Material) as Material;
        Debug.Log(NewMaterial);
        GetComponent<SpriteRenderer>().material = NewMaterial;

        int i = 0;
        while (i < time) {
            NewMaterial.SetFloat("_Level", (float)i / time);
            yield return new WaitForSeconds(0.05f);
            i++;
        }
        Destroy(Particle);
    }
}
