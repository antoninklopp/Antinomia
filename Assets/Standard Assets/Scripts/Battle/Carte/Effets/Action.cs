﻿
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Antinomia.Battle {

    /// <summary>
    /// Classe répertoriant les actions possibles lors de l'instanciation d'un sort. 
    /// Elle comprend l'action
    /// et l'entier associé à l'action.
    /// 
    /// entier zzyyxx
    /// 
    /// avec yy nombre de tours.
    /// avec zz la condition lors de la destruction ou autre
    /// 
    /// et xx l'entier symbolisant le changement tel qu'un changement de puissance.
    /// </summary>
    public class Action {

        /*
         * On définit yy de la manière suivante:
         * yy entre 0 et 99 nombre de tours où la carte est active. (On suppose que la carte ne peut pas durer plus de 99 tours).
         */

        // Le sort peut avoir un ou plusieurs effets
        public enum ActionEnum {
            /// <summary>
            /// La nature d'x entités devient Eau pendant y tours 
            /// </summary>
            NATURE_EAU,
            /// <summary>
            /// La nature d'x entités devient Terre pendant y tours
            /// </summary>
            NATURE_TERRE,
            /// <summary>
            /// La nature d'x entités devient Feu pendant y tours
            /// </summary>
            NATURE_FEU,
            /// <summary>
            /// La nature d'x entités devient Air pendant y tours
            /// </summary>
            NATURE_AIR,
            /// <summary>
            /// La nature d'x entités devient de la nature choisie lors de la condition pendant y tours. 
            /// </summary>
            NATURE,
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
            /// Detruire une carte directement sans possibilité de réponse de l'autre joueur. 
            /// </summary>
            DETRUIRE_DIRECT,
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
            /// <summary>
            /// La puissance agmente sans que l'autre joueur ne puisse répondre.
            /// </summary>
            PUISSANCE_AUGMENTE_DIRECT,
            /// <summary>
            /// La puissance de la carte est multipliee
            /// </summary>
            PUISSANCE_MULTIPLIE,
            /// <summary>
            /// La puissance d'une entité augmente pendant x tours. 
            /// Le string transmis est donc de yyxx. 
            /// </summary>
            PUISSANCE_AUGMENTE_X_TOURS,
            /// <summary>
            /// Le joueur gagne x AKA sur le tour. 
            /// </summary>
            GAIN_AKA_UN_TOUR,
            /// <summary>
            /// Le joueur est obligé d'attaquer la carte en question.
            /// </summary>
            ATTAQUE_OBLIGATOIRE,
            /// <summary>
            /// Sacrifie la carte choisie dans la condition (sacrifice = dans la main du joueur). 
            /// </summary>
            SACRIFIER_CARTE,
            /// <summary>
            /// Placer la carte dans le sanctuaire si possible. 
            /// </summary>
            PLACER_SANCTUAIRE,
            /// <summary>
            /// La carte peut attaquer directement l'adversaire. 
            /// </summary>
            ATTAQUE_DIRECTE,
            /// <summary>
            /// Cette carte ne peut pas attaquer. 
            /// </summary>
            ATTAQUE_IMPOSSIBLE,
            /// <summary>
            /// Le joueur doit révéler une carte (de sa main). 
            /// </summary>
            REVELER_CARTE,
            /// <summary>
            /// L'adversaire doit révéler une carte (de sa main). 
            /// </summary>
            REVELER_CARTE_ADVERSAIRE,
            /// <summary>
            /// Defausser une carte. 
            /// </summary>
            DEFAUSSER,
            /// <summary>
            /// Attaque d'une carte adverse
            /// </summary>
            ATTAQUE,
            /// <summary>
            /// Attaque du joueur adverse
            /// </summary>
            ATTAQUE_JOUEUR_ADVERSE,
            /// <summary>
            /// Carte forte face aux entités neutres.
            /// La puissance des entités neutre est donc divisée 
            /// par deux dans un combat face à cette entité
            /// 1 : AIR
            /// 2 : EAU
            /// 3 : FEU
            /// 4 : TERRE
            /// 5 : NEUTRE
            /// </summary>
            FORTE_ENTITE,
            /// <summary>
            /// La carte retourne dans le main de son propriétaire
            /// </summary>
            RETOUR_MAIN_PROPRIETAIRE,
            /// <summary>
            /// Ajoute x points d'AKA supplémentaire. 
            /// </summary>
            PROCURE_AKA_SUPPLEMENTAIRE,
            /// <summary>
            /// La carte ne peut plus changer sa position
            /// </summary>
            CHANGER_POSITION_IMPOSSIBLE,
            /// <summary>
            /// Si possible, la carte doit attaquer. 
            /// </summary>
            CARTE_DOIT_ATTAQUER,
            /// <summary>
            /// Cette action correspond à une condition qui a pour but de vérifier au préalable si le
            /// joueur peut payer le cout en AKA. 
            /// Payer un cout en AKA. 
            /// </summary>
            PAYER_AKA,
            /// <summary>
            /// Aucune action
            /// </summary>
            NONE
        };

        /// <summary>
        /// Action 
        /// </summary>
        public ActionEnum ActionAction;

        /// <summary>
        /// Int envoyé par la base de données, à "décortiquer". 
        /// </summary>
        public int intAction;

        /// <summary>
        /// Int propre à l'action, tel qu'un nombre de points de dégâts.
        /// </summary>
        public int properIntAction;

        /// <summary>
        /// Nombre de tours que dure l'action. 
        /// </summary>
        public int nombreDeTours;

        /// <summary> 
        /// Cette liste d'effets serait pas mal à mettre dans un document XML s'il y a beaucoup beaucoup d'effets. 
        /// Notamment pour savoir si l'effet a un timer ou sa target.
        /// Liste des effets avec un timer, c'est-ç-dire qui ont un effets sur plusieurs tours.
        /// </summary> 
        List<ActionEnum> EffetsTimer = new List<ActionEnum> { ActionEnum.NATURE_EAU,
                                                          ActionEnum.NATURE_AIR,
                                                          ActionEnum.NATURE_FEU,
                                                          ActionEnum.NATURE_TERRE,
                                                          ActionEnum.TERRAIN_ASTRAL,
                                                          ActionEnum.TERRAIN_MALEFIQUE};

        /// <summary>
        /// Si un effet est dans TargetAll, c'est que l'effet a un effet sur tout le terrain.
        /// </summary>
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

        public Action() {

        }

        /// <summary>
        /// L'action dure-t-elle plusieurs tours? 
        /// </summary>
        /// <returns>true, si l'action dure plus d'un tour, false sinon.</returns>
        public bool isEffetWithTimer() {
            /*
             * Renvoie true si l'effet a un "timer", c'est-à-dire si l'effet dure plusieurs tours
             * Renvoie false sinon. 
             */

            if (EffetsTimer.Contains(ActionAction)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// L'action a-t-elle un effet sur toutes les cartes? 
        /// </summary>
        /// <returns>true, si l'action a un effet sur toutes les cartes, false sinon.</returns>
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

        /// <summary>
        /// Decortiquer l'entier envoyé par la base de données.
        /// Afin de pouvoir le "comprendre". 
        /// </summary>
        public void understandInt() {
            if (intAction < 100) {
                properIntAction = intAction;
            }
            else {
                properIntAction = intAction % 100;
                nombreDeTours = (int)(intAction / 100);
            }
        }


    }

}
