using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryManager{

	public class HeapManagerCamera : MonoBehaviour {

		public Camera theCamera;
		public Transform memArea, bottomnMemArea;

		private int fullHeapSize;
		private float maxZoomSize, currentZoomFactor = 1;
		private const float minZoomSize = 6, scrollSpeed = 1, targetScaleFactor = 8;
		public float cameraMoveSpeed = 10;

		private bool hasScreenChanges = false;
		private int lastScreenWidth, lastScreenHeight;
		private Vector2 moveBorders = new Vector2();


		public void init(int size){
			theCamera = GetComponent<Camera> ();
			fullHeapSize = size;
			currentZoomFactor = 1;
			setPos (size / 2);

			hasScreenChanges = true;
			setLatestScreenSize ();
			StartCoroutine (setStartScale ());
		}


		// Update is called once per frame
		void Update () {
			manageControlls ();
			checkForScreenDeltas ();

			if (hasScreenChanges) {
				setMoveBorders ();
				hasScreenChanges = false;
				calcMaxZoomOut ();
				theCamera.orthographicSize = calcCurrentZoom();
				setCurrentYScale ();
				setNewMoveSpeed ();
			}
		}




		#region controlls & Checks
		private void manageControlls(){
			if (Input.mouseScrollDelta.y != 0)
				zoom (Input.mouseScrollDelta.y);

			Vector3 tempPos = transform.position;
			if (Input.GetKey (KeyCode.LeftArrow))
				tempPos.x -= cameraMoveSpeed * Time.deltaTime;
			if (Input.GetKey (KeyCode.RightArrow))
				tempPos.x += cameraMoveSpeed * Time.deltaTime;

			tempPos.x = Mathf.Clamp (tempPos.x, moveBorders.x, moveBorders.y);
			transform.position = tempPos;
		}


		private void zoom(float scroll){
			currentZoomFactor -= scroll * scrollSpeed * Time.deltaTime;
			currentZoomFactor = Mathf.Clamp (currentZoomFactor, 0, 1);
			hasScreenChanges = true;
		}

		private void checkForScreenDeltas(){
			if (lastScreenHeight != Screen.height || lastScreenWidth != Screen.width) {
				hasScreenChanges = true;
				setLatestScreenSize ();
			}
		}

		private void setNewMoveSpeed(){
			if (currentZoomFactor <= 0)
				cameraMoveSpeed = 10;
			else
				cameraMoveSpeed = fullHeapSize * currentZoomFactor;
		}


		private void setMoveBorders(){
			float allowedMidDelta = (fullHeapSize / 2) * (1 - currentZoomFactor);
			moveBorders = new Vector2 ((fullHeapSize / 2) - allowedMidDelta, (fullHeapSize / 2) + allowedMidDelta);
		}

		#endregion




		#region Zoom
		//	float magicZoomNumber = 40;
		//	float magicZoomNumber2 = 100;
		private float calcCurrentZoom(){
			float deltaZoom = maxZoomSize - minZoomSize;
			return (deltaZoom * currentZoomFactor) + minZoomSize;
		}

		private void calcMaxZoomOut(){
			float aspectRatio = (float)Screen.width / (float)Screen.height;
			float extraHeapSize = fullHeapSize/15f;

			maxZoomSize = ((4 * (fullHeapSize + extraHeapSize) /10)/aspectRatio);
			maxZoomSize = Mathf.Max (maxZoomSize, minZoomSize);
		}

		private void setCurrentYScale(){
			float scaleSpeed = 1;
			float iterations = 100;
			float targetPosY = Screen.height / targetScaleFactor;

			for (float i = 0; i < iterations; i++) {
				Vector3 screenPos = theCamera.WorldToScreenPoint (bottomnMemArea.position);

				float increment = scaleSpeed - (i / iterations);
				float factor = screenPos.y > targetPosY ? 1 : -1;

				memArea.localScale += new Vector3 (0, increment * factor, 0);
			}
		}

		private void initYScale(){
			float targetPosY = Screen.height / targetScaleFactor;
			float delta;
			do {
				float currentYPos = theCamera.WorldToScreenPoint (bottomnMemArea.position).y;
				delta = (Mathf.Abs (currentYPos - targetPosY));
				setCurrentYScale ();
			} while(delta > 1);
		}


		private IEnumerator setStartScale(){
			yield return new WaitForEndOfFrame();
			initYScale ();
		}
		#endregion


		private void setPos(int pos){
			transform.position = new Vector3 (pos, 0, -10);
		}

		private void setLatestScreenSize(){
			lastScreenWidth = Screen.width;
			lastScreenHeight = Screen.height;
		}
	}
}