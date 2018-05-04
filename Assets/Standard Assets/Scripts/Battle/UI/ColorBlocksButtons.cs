
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ColorBlocksButtons {

    /// <summary>
    /// Recuperer le ColorBlock d'un bouton de phase actif. 
    /// </summary>
    /// <returns></returns>
    public static ColorBlock GetColorCurrentButton() {
        return new ColorBlock {
            normalColor = new Color(0, 1, 0, 1f),
            highlightedColor = new Color(1, 0, 0, 0.5f),
            disabledColor = new Color(0, 1, 0, 1f),
            pressedColor = new Color(1, 0, 0, 0.5f),
            colorMultiplier = 1
        };
    }

    /// <summary>
    /// Recuperer le ColorBlock du bouton de la phase active ensuite. 
    /// </summary>
    /// <returns></returns>
    public static ColorBlock GetColorNextButton() {
        return new ColorBlock {
            normalColor = new Color(0, 1, 0, 0.2f),
            highlightedColor = new Color(0, 1, 0, 1f),
            disabledColor = new Color(0, 1, 0, 0.2f),
            pressedColor = new Color(0, 1, 0, 1f),
            colorMultiplier = 1
        };
    }

    /// <summary>
    /// Color Block d'un bouton qui est désactivé. 
    /// </summary>
    /// <returns></returns>
    public static ColorBlock GetColorDisabledButton() {
        return new ColorBlock {
            normalColor = new Color(1, 1, 1, 0.5f),
            // On indique au joueur qu'il ne peut pas cliquer sur bouton en mettant une couleur 
            // de highlight rouge
            highlightedColor = new Color(1, 0, 0, 0.5f),
            disabledColor = new Color(1, 1, 1, 0.5f),
            colorMultiplier = 1
        };
    }

    /// <summary>
    /// ColorBlock du bouton quand ce n'est ni la bonne phase, 
    /// ni le tour du joueur. 
    /// </summary>
    /// <returns></returns>
    public static ColorBlock GetColorNoTurnNoPhase() {
        return new ColorBlock {
            normalColor = new Color(1, 1, 1, 0.2f),
            // On indique au joueur qu'il ne peut pas cliquer sur bouton en mettant une couleur 
            // de highlight rouge
            highlightedColor = new Color(1, 0, 0, 0.5f),
            disabledColor = new Color(1, 1, 1, 0.2f),
            colorMultiplier = 1
        };
    }

    /// <summary>
    /// ColorBlock du bouton quand c'est la bonne phase mais 
    /// pas le tour du joueur. 
    /// </summary>
    /// <returns></returns>
    public static ColorBlock GetColorNoTurnRightPhase() {
        return new ColorBlock {
            normalColor = new Color(0, 1, 0, 0.2f),
            // On indique au joueur qu'il ne peut pas cliquer sur bouton en mettant une couleur 
            // de highlight rouge
            highlightedColor = new Color(1, 0, 0, 0.5f),
            disabledColor = new Color(0, 1, 0, 0.2f),
            colorMultiplier = 1
        };
    }

}
