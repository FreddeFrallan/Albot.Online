using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SpeedRunner {
    public class CameraMovement : MonoBehaviour {

        private float minimumHeight = -2;
        private float extraXPos = 5;
        [SerializeField]
        private List<Transform> followObjects = new List<Transform>();
        [SerializeField]
        private Camera theCam;
        [SerializeField]
        private Transform cameraTransform;


        private void Update() {
            Vector3 tempPos = cameraTransform.position;
            setXPos(ref tempPos);
            setYPos(ref tempPos);
            cameraTransform.position = tempPos;
        }


        private void setXPos(ref Vector3 cameraPos) {
            Vector2 topPlayerPos = getFrontObject();
            cameraPos.x = topPlayerPos.x + extraXPos;
        }

        private void setYPos(ref Vector3 cameraPos) {
            Vector2 topPlayerPos = getTopObject();
            cameraPos.y = Mathf.Max(minimumHeight + theCam.orthographicSize, topPlayerPos.y);
        }

        private Vector2 getTopObject() { return followObjects.OrderByDescending((t) => t.position.y).ToList()[0].position; }
        private Vector2 getFrontObject() { return followObjects.OrderByDescending((t) => t.position.x).ToList()[0].position; }
    }
}