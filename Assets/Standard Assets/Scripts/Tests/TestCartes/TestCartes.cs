using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO; 

public class TestCartes : MonoBehaviour {

    GetPlayerInfoGameSparks playerInfo;

    private List<GameObject> Cards;

    public GameObject CartePrefab;

    /// <summary>
    /// Dossier contenant tous les fichiers qui doivent être en output. 
    /// </summary>
    string folder = "Assets/Resources/TestCartes";

	// Use this for initialization
	void Start () {
        playerInfo = GetComponent<GetPlayerInfoGameSparks>();
        StartCoroutine(GetCards());
    }

    IEnumerator GetCards() {
        yield return playerInfo.WaitForPlayerCards(CartePrefab); 
        Cards = playerInfo.GetAllCards(CartePrefab); 
#if UNITY_EDITOR
        Test();
#endif
    }

    // Update is called once per frame
    void Update () {
		
	}

    // Runner tous les tests.
    void Test() {
        // On detruit d'abord tous les fichiers dans le dossier. 
        DeleteFilesTestFolder();
        foreach (GameObject Carte in Cards) {
            Carte compCarte = Carte.GetComponent<Carte>();
            StreamWriter writer = new StreamWriter(folder + "/" + compCarte.Name + ".txt", true);
            List<Effet> allEffets = compCarte.AllEffets;
            string effetToString = compCarte.AllEffetsStringToDisplay;
            int i = 1; 
            foreach (Effet e in allEffets) {
                TestUneCarte(e, compCarte, writer, effetToString, i);
                i++; 
            }
            writer.Close(); 
        }
    }

    /// <summary>
    /// Test d'une carte. 
    /// Avec sortie des résultats sur le fichier file. 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="compCarte"></param>
    /// <param name="file"></param>
    /// <param name="effetToDisplay"></param>
    /// <param name="numeroEffet"></param>
    void TestUneCarte(Effet e, Carte compCarte, StreamWriter file, string effetToDisplay, int numeroEffet) {
        // Iteration sur toutes les phases
        Debug.Log("On cree une carte"); 
        file.WriteLine("Effet : " + effetToDisplay);
        file.WriteLine("Numero de l'effet : " + numeroEffet);
        foreach (Player.Phases p in Enum.GetValues(typeof(Player.Phases))) {
            // Iteration sur si la carte vient d'être posée
            for (int i = 0; i < 2; i++) {
                // Si la carte vient d'être détruite
                for (int j = 0; j < 2; j++) {
                    // On vient de passer un nouveau Tour. 
                    for (int k = 0; k < 2; k++) {
                        // On crée des entités avec des états différents
                        foreach (Entite.State es in Enum.GetValues(typeof(Entite.State))) {
                            // On crée des entités avec des natures differentes. s
                            foreach (Entite.Nature en in Enum.GetValues(typeof(Entite.Nature))) {
                                // Changement de domination. 
                                for (int l = 0; l < 2; l++) {
                                    GameObject CarteCible = Instantiate(CartePrefab);
                                    CarteCible.AddComponent<Entite>(); 
                                    CarteCible.GetComponent<Entite>().EntiteState = es;
                                    CarteCible.GetComponent<Entite>().EntiteNature = en;
                                    compCarte.GererConditions(e.AllConditionsEffet, _currentPhase: p, debut: i == 1, estMort: j == 1,
                                        nouveauTour: (k == 1), Cible: CarteCible, changementDomination : (l==1));
                                    file.WriteLine(
                                        "La carte vient d'être posée : " + (i == 1).ToString() + " \n" +
                                        "La carte vient de mourir : " + (j == 1).ToString() + " \n" +
                                        "On vient de passer à un nouveau Tour : " + (k == 1).ToString() + " \n" +
                                        "Caracteristiques de l'objet attaque : " + "\n" +
                                        "Etat " + es.ToString() + "\n" +
                                        "Nature " + en.ToString() + "\n" +
                                        "Changement de domination " + (l == 1).ToString() + "\n"
                                        );
                                    Destroy(CarteCible);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Detruire tous les fichiers dans le dossier de Test. 
    /// </summary>
    void DeleteFilesTestFolder() {
        foreach (string f in Directory.GetFiles(folder)) {
            File.Delete(f); 
        }
    }
}
