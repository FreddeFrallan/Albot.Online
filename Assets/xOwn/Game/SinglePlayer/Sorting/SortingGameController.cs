using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SortingGame {

    public class SortingGameController : MonoBehaviour {

        [SerializeField]
        private GameObject visualNumberPrefab;
        [SerializeField]
        private Transform sortingParent;

        private float gapBetweenNumbers = 1;
        private float cameraPadding = 3;

        private SortingVisualNumber[] currentNumbers;
        private float[] sortArray;
        

        public SortingVisualNumber[] startNewSession(int amountOfNumbers, float min, float max, bool integers = true) {
            clearOldNumbers();
            generateNewArray(amountOfNumbers, min, max, integers);
            generateNewVisualNumbers();
            setCamera();
            return currentNumbers;
        }

        private void generateNewArray(int amount, float min, float max, bool integers) {
            sortArray = new float[amount];
            for (int i = 0; i < amount; i++)
                sortArray[i] = integers ? Random.RandomRange((int)min, (int)max) : Random.RandomRange(min, max);
        }

        private void generateNewVisualNumbers() {
            int n = sortArray.Length;
            float fullArrayWidth = n * SortingVisualNumber.WIDTH + (n - 1) * gapBetweenNumbers;
            float firstPos = (-fullArrayWidth + SortingVisualNumber.WIDTH) /2 ;
            float delta = SortingVisualNumber.WIDTH + gapBetweenNumbers;

            currentNumbers = new SortingVisualNumber[n];
            for(int i = 0; i < n; i++) {
                currentNumbers[i] = Instantiate(visualNumberPrefab, sortingParent).GetComponent<SortingVisualNumber>();
                currentNumbers[i].init(new Vector3(firstPos + delta * i, 0, 0), sortArray[i]);
            }
        }

        private void setCamera() {
            float arrayEndWidth = -currentNumbers[0].transform.position.x + SortingVisualNumber.WIDTH/2;
            float fullCamWidth = arrayEndWidth + 2 * cameraPadding;
            float neededOrtSize = fullCamWidth * Screen.height / Screen.width;
            Camera.main.orthographicSize = neededOrtSize;
        }

        private void clearOldNumbers() {
            if (currentNumbers == null)
                return;
            foreach (SortingVisualNumber n in currentNumbers)
                Destroy(n.gameObject);
        }

        public bool isInRange(int index) { return index >= 0 && index < sortArray.Length; }
    }

}