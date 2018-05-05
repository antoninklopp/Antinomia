
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Antinomia.Battle {

    /// <summary>
    /// Effets d'une carte
    /// </summary>
    public class Effet {
        /*
         * L'effet est une association d'une liste de conditions et d'une liste d'actions. 
         * 
         * Décrypter le string en arrivée de la base de données gamesparks. 
         * 
         * Effet1 : Effet2 : Effet3....
         * avec Effet1 => Condition1 ; Condition2 ; Condition3 ; ... ! Action1 ; Action2 ; Action3 ; ... . 
         * 
         */

        /// <summary>
        /// Description de l'effet. 
        /// </summary>
        protected string effetString;

        /// <summary>
        /// Toutes les conditions de l'effet.
        /// </summary>
        public List<Condition> AllConditionsEffet = new List<Condition>();

        /// <summary>
        /// Toutes les actions de l'effet. 
        /// </summary>
        public List<Action> AllActionsEffet = new List<Action>();

        /// <summary>
        /// Si l'effet est déclarable, on le propose au joueur lorsque c'est le bon timing,
        /// sinon on le joue automatiquement. 
        /// </summary>
        public bool EstDeclarable;

        public string EffetString {
            get {
                return effetString;
            }

            set {
                effetString = value;
            }
        }

        public Effet() {

        }

        public Effet(List<Condition> AllCondiditonsEffet, List<Action> AllActionsEffet) {
            this.AllConditionsEffet = AllCondiditonsEffet;
            this.AllActionsEffet = AllActionsEffet;
        }

        public override string ToString() {
            return effetString;
        }

        /// <summary>
        /// Pour un sort il peut être nécessaire de choisir plusieurs cartes. 
        /// Il faut donc regarder si lors d'un sort, il n'est pas nécessaire de devoir jouer plusieurs cartes.
        /// </summary>
        public int CartesNecessairesSort() {
            foreach (Condition c in AllConditionsEffet) {
                Debug.Log(c.ConditionCondition);
                Debug.Log((int)c.ConditionCondition);
                if (Array.IndexOf(new int[] { 0, 1, 2, 3, 7, 8, 9 }, (int)c.ConditionCondition) != -1) {
                    return c.properIntCondition;
                }
            }
            return 1;
        }

        /// <summary>
        /// Renvoie si l'effet est déclarable. 
        /// </summary>
        /// <returns></returns>
        public bool IsDeclarable() {
            return EstDeclarable;
        }


    }

}
