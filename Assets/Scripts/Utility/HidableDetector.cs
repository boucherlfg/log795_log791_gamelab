using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLab.Utility
{
    public class HidableDetector : MonoBehaviour
    {
        [SerializeField] private List<Transform> tracingOrigin = new();
        [SerializeField] private int checkIntervalMs = 1;

        [Header("Debug")]
        [SerializeField] private List<Hidable> hidedElements = new();


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(nameof(StartInfiniteInterval));
        }

        private void DetectHidable()
        {
            if (Camera.main == null)
            {
                return;
            }

            List<Hidable> currentHidable = new();

            // GET ALL HIDABLE HIDING THE ELEMENT
            foreach (var origin in tracingOrigin)
            {
                Vector3 direction = (Camera.main.transform.position - origin.position);
                RaycastHit[] hits = Physics.RaycastAll(origin.position, direction.normalized, direction.magnitude);
                if (hits.Length > 0)
                {
                    Debug.DrawRay(origin.position, direction, Color.green);
                }
                else
                {
                    Debug.DrawRay(origin.position, direction, Color.blue);
                }
                foreach (var hit in hits)
                {
                    Hidable hidable = hit.transform.GetComponentInChildren<Hidable>();
                    if (hidable is null)
                    {
                        continue;
                    }

                    if (!currentHidable.Contains(hidable))
                    {
                        currentHidable.Add(hidable);
                    }
                }
            }


            // PRE CREATE THE LISTS
            List<Hidable> elementsToHide = new();
            List<Hidable> elementsToShow = new();

            // ASSIGN THE ELEMENTS IN THE LISTS
            SortElements(currentHidable, elementsToHide, elementsToShow);

            // TREAT THE LISTS
            TreatLists(elementsToHide, elementsToShow);
        }

        private void TreatLists(List<Hidable> elementsToHide, List<Hidable> elementsToShow)
        {
            hidedElements.Clear();

            // HIDE ALL NEW ELEMENTS
            foreach (var element in elementsToHide)
            {
                element.Hide();
                hidedElements.Add(element);
            }

            // ASSIGN ELEMENTS TO HIDE TO THE LIST
            hidedElements.AddRange(elementsToHide);

            // SHOW ALL ELEMENTS NOT IN THE PATH ANYMORE
            foreach (var element in elementsToShow)
            {
                element.Show();
            }
        }

        private void SortElements(List<Hidable> listToSort, List<Hidable> elementsToHide, List<Hidable> elementsToShow)
        {
            foreach (var element in hidedElements)
            {
                if (!listToSort.Contains(element))
                {
                    elementsToShow.Add(element);
                }
            }

            elementsToHide.AddRange(listToSort);
        }

        IEnumerator StartInfiniteInterval()
        {
            WaitForSeconds delay = new WaitForSeconds((float)checkIntervalMs / 1000.0f);
            while (true)
            {
                DetectHidable();
                yield return delay;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            foreach (var element in hidedElements)
            {
                element.Show();
            }
        }
    }
}
