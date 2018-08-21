using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect4 {

    public class Connect4WinFlicker : MonoBehaviour {

        public MeshRenderer theMesh;
        private float minOpacity = 0.2f, maxOpacity = 1, fadeSpeed = 1;
        private float currentOpacity = 0.2f;
        private bool fadingUp = true;

        // Use this for initialization
        void Start() {
            setOpacity(minOpacity);
            StartCoroutine(fadeColor());
        }

        private IEnumerator fadeColor() {
            while (true) {
                yield return new WaitForEndOfFrame();

                if (fadingUp) {
                    currentOpacity += Time.deltaTime * fadeSpeed;
                    if (currentOpacity >= maxOpacity)
                        fadingUp = false;
                    currentOpacity = Mathf.Clamp(currentOpacity, minOpacity, maxOpacity);
                }
                else {
                    currentOpacity -= Time.deltaTime * fadeSpeed;
                    if (currentOpacity <= minOpacity)
                        fadingUp = true;
                    currentOpacity = Mathf.Clamp(currentOpacity, minOpacity, maxOpacity);

                }

                setOpacity(currentOpacity);
            }
        }

        private void setOpacity(float o) {
            Color c = theMesh.material.color;
            c.a = o;
            theMesh.material.color = c;
        }
    }
}