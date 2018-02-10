using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GoNextSceneAfterIntro : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Load()); 
	}

    private IEnumerator Load() {
        AsyncOperation SceneLoad = SceneManager.LoadSceneAsync("LoginDataBase");
        Debug.Log("OK"); 
        SceneLoad.allowSceneActivation = false;
        yield return new WaitForSeconds(3f);
        SceneLoad.allowSceneActivation = true;
        Debug.Log("OK");
        while (!SceneLoad.isDone) {
            Debug.Log("On attend"); 
            yield return new WaitForSeconds(0.1f); 
        }
    }
}
