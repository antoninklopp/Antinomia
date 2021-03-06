﻿
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

    /// <summary>
    /// Créer un objet sur la carte. 
    /// </summary>
    public class LineRendererObject : MonoBehaviour {

        bool isDragging = false;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            Dragging();
        }

        /// <summary>
        /// Deplacer la carte. 
        /// La carte suit la souris. 
        /// </summary>
        public void Dragging() {
            /*
             * Déplacement de la carte qui suit la souris. 
             */
            Vector3 MousePosition = Input.mousePosition;
            MousePosition.z = 15;
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);
            transform.position = mouseWorldPoint;
        }

        /// <summary>
        /// Trouver tous les colliders à une certaine distance de la LineRenderer. 
        /// </summary>
        /// <param name="distance">Distance maximale</param>
        /// <param name="center">Centre de la recherche</param>
        /// <returns></returns>
        public GameObject[] FindAllColliders(float distance, Vector2 center) {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, distance);
            Debug.Log(transform.position);
            Debug.Log(hitColliders.Length);
            GameObject[] allCollidersObject = new GameObject[hitColliders.Length];
            for (int i = 0; i < hitColliders.Length; i++) {
                allCollidersObject[i] = hitColliders[i].gameObject;
            }
            return allCollidersObject;
        }
    }

}
