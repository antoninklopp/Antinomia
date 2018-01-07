using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class RetourMenu : MonoBehaviour {

    public void RetourMenuBouton() {
        SceneManager.LoadScene("MainMenu"); 
    }

}
