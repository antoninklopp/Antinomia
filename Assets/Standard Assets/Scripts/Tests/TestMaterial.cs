using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMaterial : MonoBehaviour {

    public Material m;

    public bool FinCarte;

    public GameObject ParticleFin; 


	// Use this for initialization
	void Start () {
        Debug.Log("ici"); 
        StartCoroutine(DisolveCard()); 
	}

    private void Update() {
        if (FinCarte) {
            StartCoroutine(AnimerParticuleFin());
            FinCarte = false;
        }
    }

    private IEnumerator DisolveCard() {
        Material NewMaterial = Instantiate(Resources.Load("Material/DisolveMaterial") as Material) as Material;
        Debug.Log(NewMaterial); 
        GetComponent<SpriteRenderer>().material = NewMaterial;

        int i = 0; 
        while (i < 50) {
            NewMaterial.SetFloat("_Level", 1 - (float)i/50f);
            yield return new WaitForSeconds(0.05f);
            i++; 
        }
    }

    private IEnumerator AnimerParticuleFin() {
        GameObject Particle = Instantiate(ParticleFin);
        Particle.transform.position = transform.position; 


        // Disolve
        Material NewMaterial = Instantiate(Resources.Load("Material/DisolveMaterial") as Material) as Material;
        Debug.Log(NewMaterial);
        GetComponent<SpriteRenderer>().material = NewMaterial;

        int i = 0;
        while (i < 20) {
            NewMaterial.SetFloat("_Level",(float)i / 20f);
            yield return new WaitForSeconds(0.05f);
            i++;
        }
        Destroy(Particle);

        yield return new WaitForSeconds(3f); 

        StartCoroutine(DisolveCard()); 
    }


}
