
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class RetourMenu : MonoBehaviour {

    public void RetourMenuBouton() {
        SceneManager.LoadScene("MainMenu"); 
    }

}
