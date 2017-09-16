﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class Action {
    /*
    * Classe répertoriant les actions possibles lors de l'instanciation d'un sort. 
    * Elle comprend l'action
    * et l'entier associé à l'action. 
    * 
    * entier yyxx
    * 
    * avec yy nombre de tours. 
    *  et  xx l'entier symbolisant le changement tel qu'un changement de puissance. 
    */

    // Le sort peut avoir un ou plusieurs effets
    public enum ActionEnum {
        NATURE_EAU, // La nature d'x entités devient Eau pendant y tours 
        NATURE_TERRE, // La nature d'x entités devient Terre pendant y tours
        NATURE_FEU, // La nature d'x entités devient Feu pendant y tours
        NATURE_AIR, // La nature d'x entités devient Air pendant y tours
        NATURE, // La nature d'x entités devient de la nature choisie lors de la condition pendant y tours. 
        /// <summary>
        /// Changer la nature d'une autre entité. 
        /// </summary>
        CHANGER_NATURE_AUTRE_ENTITE, 
        /// <summary>
        /// Piocher x cartes
        /// </summary>
        PIOCHER_CARTE, 
        /// <summary>
        /// Detruire y cartes
        /// </summary>
        DETRUIRE, 
        /// <summary>
        /// Changer la position d'une carte
        /// </summary>
        CHANGER_POSITION, 
        /// <summary>
        /// Pendant x tours, le terrain passe sous domination astrale
        /// </summary>
        TERRAIN_ASTRAL,
        /// <summary>
        /// Pendant x tours, le terrain passe sous domination maléfique
        /// </summary>
        TERRAIN_MALEFIQUE,
        /// <summary>
        /// La puissance de toutes les entités eau sur le champ de bataille augmente de x
        /// </summary>
        PUISSANCE_EAU_AUGMENTE,
        /// <summary>
        /// La puissance de toutes les entités terre sur le champ de bataille augmente de x
        /// </summary>
        PUISSANCE_TERRE_AUGMENTE,
        /// <summary>
        /// La puissance de toutes les entités air sur le champ de bataille augmente de x
        /// </summary>
        PUISSANCE_AIR_AUGMENTE,
        /// <summary>
        /// La puissance de toutes les entités feu sur le champ de bataille augmente de x
        /// </summary>
        PUISSANCE_FEU_AUGMENTE,
        /// <summary>
        /// Ici système particulier car on a besoin de deux entiers! un pour le nombre de tours et un pour l'augmentation de puissance
        /// Donc l'entier transmis sera de type xxyy avec xx l'augmentation de puissance (inférieur à 99) et yy le nombre de tours (inférieur à 99)
        /// </summary>
        PUISSANCE_AUGMENTE, 
        PUISSANCE_MULTIPLIE, 
        GAIN_AKA_UN_TOUR,
        /// <summary>
        /// Le joueur est obligé d'attaquer la carte en question.
        /// </summary>
        ATTAQUE_OBLIGATOIRE,
        /// <summary>
        /// Sacrifie la carte choisir dans la condition.
        /// </summary>
        SACRIFIER_CARTE, 
        /// <summary>
        /// Placer la carte dans le sanctuaire si possible. 
        /// </summary>
        PLACER_SANCTUAIRE, 
        NONE
    };

    public ActionEnum ActionAction;
    // Cet entier comprend toutes les informations. 
    public int intAction;

    public int properIntAction; 
    public int nombreDeTours; 

    // Cette liste d'effets serait pas mal à mettre dans un document XML s'il y a beaucoup beaucoup d'effets. 
    // Notamment pour savoir si l'effet a un timer ou sa target.
    // Liste des effets avec un timer, c'est-ç-dire qui ont un effets sur plusieurs tours. 
    List<ActionEnum> EffetsTimer = new List<ActionEnum> { ActionEnum.NATURE_EAU,
                                                          ActionEnum.NATURE_AIR,
                                                          ActionEnum.NATURE_FEU,
                                                          ActionEnum.NATURE_TERRE,
                                                          ActionEnum.TERRAIN_ASTRAL,
                                                          ActionEnum.TERRAIN_MALEFIQUE};

    // Si un effet est dans TargetAll, c'est que l'effet a un effet sur tout le terrain.
    List<ActionEnum> TargetAll = new List<ActionEnum> { ActionEnum.PIOCHER_CARTE,
                                        ActionEnum.TERRAIN_ASTRAL,
                                        ActionEnum.TERRAIN_MALEFIQUE,
                                        ActionEnum.PUISSANCE_AIR_AUGMENTE,
                                        ActionEnum.PUISSANCE_EAU_AUGMENTE,
                                        ActionEnum.PUISSANCE_FEU_AUGMENTE,
                                        ActionEnum.PUISSANCE_TERRE_AUGMENTE}; 

    public Action(ActionEnum _action, int _intAction) {
        ActionAction = _action;
        intAction = _intAction;
        understandInt(); 
    }

    public Action(){

    }


    public bool isEffetWithTimer() {
        /*
         * Renvoie true si l'effet a un "timer", c'est-à-dire si l'effet dure plusieurs tours
         * Renvoie false sinon. 
         */
        
         if (EffetsTimer.Contains(ActionAction)){
            return true; 
         }
        return false;
    }

    public bool isEffetTargetAll() {
        /*
         * Renvoie true si l'effet target tout le terrain
         * Renvoie false sinon. 
         */ 
         if (TargetAll.Contains(ActionAction)) {
            return true; 
        }
        return false;
    }

    public void understandInt() {
        if (intAction < 100) {
            properIntAction = intAction; 
        } else {
            properIntAction = intAction % 100;
            nombreDeTours = (int)(intAction / 100); 
        }
    }


}