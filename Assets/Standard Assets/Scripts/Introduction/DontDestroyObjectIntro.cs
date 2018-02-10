using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 


public class DontDestroyObjectIntro : MonoBehaviour {

    private void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        GetComponent<Canvas>().worldCamera = Camera.main; 
    }
}
