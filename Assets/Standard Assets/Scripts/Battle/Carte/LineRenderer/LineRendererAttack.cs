﻿
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.Battle {

    /// <summary>
    /// A mettre sur la carte. 
    /// </summary>
    public class LineRendererAttack : MonoBehaviour {

        public LineRenderer lineRenderer;

        public GameObject LineRendererPrefab;

        /// <summary>
        /// Ligne courante. 
        /// </summary>
        public GameObject CurrentInstantiatedLine;

        protected bool enCours = true;

        protected GameObject FinalTarget;

        private Vector2 lastPosition;

        // Use this for initialization
        void Start() {
            // LineRendererSetup();
            LineRendererPrefab = Resources.Load("Prefabs/AttackLine", typeof(GameObject)) as GameObject;
        }

        // Update is called once per frame
        void Update() {
            if (lineRenderer != null) {
                LineRendererUpdate();
            }
        }

        void LineRendererSetup() {
            Debug.Log("On fait le setup");
            lineRenderer.SetPosition(0, lineRenderer.transform.position);
            lineRenderer.SetPosition(1, transform.position);

            lineRenderer.sortingOrder = 3;
        }

        void LineRendererUpdate() {
            // Debug.Log(lineRenderer); 
            int count = 100;
            lineRenderer.positionCount = count + 1;
            lineRenderer.SetPosition(0, transform.position);
            for (int i = 1; i <= count; i++) {
                // On set tous les points entre. Pour faire un forme.
                lineRenderer.SetPosition(i, new Vector2(transform.position.x * (count - i) / count + lineRenderer.transform.parent.position.x * i / count,
                                                        transform.position.y * (count - i) / count + lineRenderer.transform.parent.position.y * i / count));
            }
        }

        public void OnBeginDragAttack() {
            LineRendererPrefab = Resources.Load("Prefabs/AttackLine", typeof(GameObject)) as GameObject;
            Debug.Log("LineRendererPrefab");
            CurrentInstantiatedLine = Instantiate(LineRendererPrefab);
            CurrentInstantiatedLine.SetActive(true);
            lineRenderer = CurrentInstantiatedLine.transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
            CurrentInstantiatedLine.transform.position = transform.position;
            enCours = true;
            // LineRendererSetup(); 

        }

        protected virtual void OnMouseUp() {
            if (CurrentInstantiatedLine != null) {
                Vector3 MousePosition = Input.mousePosition;
                MousePosition.z = 15;
                Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(MousePosition);
                Debug.Log("Position d'arrêt : " + mouseWorldPoint.ToString());
                lastPosition = new Vector2(mouseWorldPoint.x, mouseWorldPoint.y);
                FindGameObjectOverlap();
                enCours = false;
                Debug.Log(CurrentInstantiatedLine);
                // InstantiatedLine.SetActive(false);
                LineRendererPrefab.SetActive(false);
                Debug.Log("On a détruit la ligne");
            }
        }

        public bool EstEnCours() {
            return enCours;
        }

        /// <summary>
        /// Trouver la carte la plus proche 
        /// </summary>
        public void FindGameObjectOverlap() {
            float distance = 0.5f;
            while (true) {
                GameObject[] allObjectsOverlap = CurrentInstantiatedLine.GetComponent<LineRendererObject>().FindAllColliders(distance,
                    lastPosition);
                for (int i = 0; i < allObjectsOverlap.Length; i++) {
                    Debug.Log("Objet Overlap" + allObjectsOverlap[i].name);
                    Debug.Log("La dernière position " + lastPosition);
                    Debug.Log("position de l'objet " + allObjectsOverlap[i].transform.position);
                }
                if (distance < 0 || distance > 1) {
                    FinalTarget = null;
                    break;
                }
                if (allObjectsOverlap.Length > 2) {
                    distance -= 0.1f;
                }
                else if (allObjectsOverlap.Length == 1 || (allObjectsOverlap.Length == 2 &&
                  (allObjectsOverlap[1] == gameObject))) {
                    FinalTarget = allObjectsOverlap[0];
                    break;
                }
                else if (allObjectsOverlap[0] == gameObject || allObjectsOverlap.Length == 2) {
                    FinalTarget = allObjectsOverlap[1];
                    break;
                }
                else {
                    distance += 0.1f;
                }
            }
        }

        public GameObject GetFinalTarget() {
            return FinalTarget;
        }
    }

}
