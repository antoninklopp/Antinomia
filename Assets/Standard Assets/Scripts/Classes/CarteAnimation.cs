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
    public void AnimationDebut(float alpha=0.5f) {
        StartCoroutine(AnimationEntreeRoutine(50, alpha));
    }

	private IEnumerator AnimationEntreeRoutine(int time, float alpha=0.5f) {
        Material NewMaterial = Instantiate(Resources.Load("Material/DisolveMaterial") as Material) as Material;
        GetComponent<SpriteRenderer>().material = NewMaterial;
        NewMaterial.SetFloat("_Alpha", alpha); 

        // Appear
        int i = 0;
        while (i < time) {
            NewMaterial.SetFloat("_Level", 1 - (float)i / time);
            yield return new WaitForSeconds(0.05f);
            i++;
        }
    }

    public void AnimationDeposeCarte(float alpha = 1f) {
        StartCoroutine(AnimationDeposeCarteRoutine(50, alpha)); 
    }

    private IEnumerator AnimationDeposeCarteRoutine(int time, float alpha=1f) {
        // Disolve
        Material NewMaterial = Instantiate(Resources.Load("Material/DisolveMaterial") as Material) as Material;
        Debug.Log(NewMaterial);
        GetComponent<SpriteRenderer>().material = NewMaterial;
        NewMaterial.SetFloat("_Alpha", alpha);

        int i = 0;
        while (i < time) {
            NewMaterial.SetFloat("_Level", (float)i / time);
            yield return new WaitForSeconds(0.05f);
            i++;
        }
    }

    /// <summary>
    /// Animation de la disparition de la carte
    /// </summary>
    public void AnimationFin(float alpha=0.5f) {
        StartCoroutine(AnimationFinRoutine(50, alpha)); 
    }

    private IEnumerator AnimationFinRoutine(int time, float alpha=0.5f) {
        GameObject Particle = Instantiate(Resources.Load("Particles/ParticleDeath") as GameObject);
        Particle.transform.position = transform.position;

        // Disolve
        Material NewMaterial = Instantiate(Resources.Load("Material/DisolveMaterial") as Material) as Material;
        Debug.Log(NewMaterial);
        GetComponent<SpriteRenderer>().material = NewMaterial;
        NewMaterial.SetFloat("_Alpha", alpha);

        int i = 0;
        while (i < time) {
            NewMaterial.SetFloat("_Level", (float)i / time);
            yield return new WaitForSeconds(0.05f);
            i++;
        }
        Destroy(Particle);
    }
}
