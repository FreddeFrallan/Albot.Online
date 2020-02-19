using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SortingGame { 

    public class SortingVisualNumber : MonoBehaviour {

        public const float WIDTH = 10;

        [SerializeField]
        private TextMeshPro text;
        [SerializeField]
        private MeshRenderer theMesh;
        public float value;

        public void init(Vector3 localPos, float value) {
            transform.localPosition = localPos;
            assignNewValue(value);
        }

        public void assignNewValue(float value) {
            this.value = value;
            text.text = value.ToString();
        }

        public void setColor(Color c) {theMesh.material.color = c;}
    }

}