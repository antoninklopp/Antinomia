
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Console {

    public class ReportBug {

        private Queue<string> allBugs = new Queue<string>();

        /// <summary>
        /// True si des bugs ont été report 
        /// </summary>
        private bool bugsReported = false;

        public ReportBug() {
            allBugs.Enqueue("BUG REPORT");
        }

        public ReportBug(string initializeString) : this() {
            allBugs.Enqueue(initializeString);
        }

        /// <summary>
        /// Report un nouveau bug. 
        /// </summary>
        /// <param name="bug"></param>
        public void addBug(string bug) {
            allBugs.Enqueue(bug);
            bugsReported = true;
        }

        /// <summary>
        /// Recuperer tous les report de bugs. 
        /// </summary>
        /// <returns></returns>
        public string getAllReport() {
            string retour = "";
            if (!bugsReported) {
                return null;
            }
            int i = allBugs.Count;
            while (i > 0) {
                i--;
                try {
                    retour += allBugs.Dequeue() + "\n";
                }
                catch (InvalidOperationException e) {
                    Debug.LogWarning("Impossible de dequeue ici " + e.ToString());
                }
            }
            retour += "heure " + DateTime.Now.ToString();
            return retour;
        }
    }

}
