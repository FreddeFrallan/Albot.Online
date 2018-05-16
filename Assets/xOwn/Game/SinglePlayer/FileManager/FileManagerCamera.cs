using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileManager{

	public class FileManagerCamera : MonoBehaviour {

		public Camera theCamera;
		private float maxOrtSize, minOrtSize, maxYPos, minXPos;
		private float midCameraPos = 45.6f;
		private float currentAspectRatio;

		private float zoomFactor = 1f, minZoomFactor;
		private float zoomSpeed = 1f;


		// Use this for initialization
		void Start () {
			
		}
				
		// Update is called once per frame
		void Update () {
			updateScreen();
			setZoom ();
			zoomFactor -= Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
			zoomFactor = Mathf.Clamp (zoomFactor, minZoomFactor, 1);
			transform.position = new Vector3 (midCameraPos, maxYPos, -5);
		}

		private void setZoom(){
			minZoomFactor = minOrtSize / maxOrtSize;
			theCamera.orthographicSize = Mathf.Clamp(maxOrtSize*zoomFactor, minOrtSize, maxOrtSize);
		}

		private void updateScreen(){
			currentAspectRatio = (float)Screen.width / (float)Screen.height;
			maxOrtSize = 25 / currentAspectRatio;
			minOrtSize = 6 / currentAspectRatio;


			float maxDeltaZoom = 1 - minZoomFactor;
			float percentageOfMaxZoom = (zoomFactor - minZoomFactor) / maxDeltaZoom;
			//print (percentageOfMaxZoom);

			float tempScaledYPos = -22 / currentAspectRatio;

			maxYPos = percentageOfMaxZoom * tempScaledYPos;

			//print ("Aspect: " + currentAspectRatio + "  ortSize: " + theCamera.orthographicSize + "  Zoom: " + zoomFactor);
		}
			
	}

}