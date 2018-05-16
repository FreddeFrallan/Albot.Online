using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI{

	public class MenuDropdown : MonoBehaviour {

		public float upDist = 10;
		public bool rollUpMode{set; get;}
		private Vector2 downPos, upPos;
		public MenuBar theMenu;


		private bool isDown = true;
		private bool isUp = false;
		private int moveDir = 1; // 1 -> UP  -1 -> down
		private float moveSpeed = 50;
		private bool hasMouseFocus = true;

		void Start(){
			downPos = transform.position;
			upPos = downPos;
			upPos.y += upDist;
		}

		void Update(){
			if (hasMouseFocus && isDown)
				return;
			if (!hasMouseFocus && isUp)
				return;

			Vector2 tempPos = transform.position;
			moveDir = hasMouseFocus ? -1 : 1;
			tempPos.y += moveSpeed * moveDir * Time.deltaTime;
			transform.position = tempPos;

			if (hasMouseFocus) {
				isUp = false;
				checkIfDown ();
			} else {
				isDown = false;
				theMenu.rollUpDropdowns ();
				checkIfUp ();
			}
		}



		public void OnMouseOver(){
			hasMouseFocus = true;
		}

		public void OnMouseExit(){
			if(rollUpMode)
				hasMouseFocus = false;
		}
			
		private bool checkIfUp(){
			if (transform.position.y >= upPos.y) {
				transform.position = upPos;
				isUp = true;
				return true;
			}
			return false;
		}
			
		private bool checkIfDown(){
			if (transform.position.y <= downPos.y) {
				transform.position = downPos;
				isDown = true;
				return true;
			}
			return false;
		}
	}
}