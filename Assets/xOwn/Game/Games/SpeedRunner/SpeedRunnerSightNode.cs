using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedRunner {

    public class SpeedRunnerSightNode : MonoBehaviour {

        private Camera theCam;

        public void init(Camera theCam) {
            this.theCam = theCam;
        }

        public string raycastNode() {
            RaycastHit hit;
            print("Node " +theCam.WorldToScreenPoint(transform.position));
            print("Mouse: " + Input.mousePosition);
            Ray ray = theCam.ScreenPointToRay(theCam.WorldToScreenPoint(transform.position));

            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
                return "H";
            }
            return "O";
        }

    }
}