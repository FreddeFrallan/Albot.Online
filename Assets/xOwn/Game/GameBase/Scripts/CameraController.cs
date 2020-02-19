using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraController : MonoBehaviour {

	private float moveSpeed = 30;
	private float mouseMoveSpeed = 15f;
	private float scrollSpeed = 25;
	private float lookSpeed = 150;
	private Vector2 mouseDelta;
	public Rigidbody rig;

	private Vector2 mouseXClamp = new Vector2(80, -10);
	private bool cursorIsLocked = true;
	private Vector3 startPos;

	// Update is called once per frame
	void Update () {
		rig.velocity = Vector3.zero;
		calcMouseDelta ();
		arrowMoves ();
		mouseScroll ();
		mouseMoves ();
		mouseLook ();
		constraints ();
		InternalLockUpdate ();
	}

	private void constraints(){
		float angle = transform.localEulerAngles.x;
		Vector3 targetAngle = transform.localEulerAngles;


		if (angle > 300 && angle < 360 + mouseXClamp.y) {
			targetAngle.x = 360 + mouseXClamp.y;
		} else if (angle < 300 && angle > mouseXClamp.x) {
			targetAngle.x = mouseXClamp.x;
		}

		transform.localEulerAngles = targetAngle;
	}


	private void mouseLook(){
		if (Input.GetMouseButton (1) == false)
			return;

		Vector3 extraRot = Vector3.zero;
		extraRot.y = mouseDelta.x * lookSpeed;
		extraRot.x = -mouseDelta.y * lookSpeed;

		transform.localEulerAngles += extraRot * Time.deltaTime;

		Vector3 forwardPoint = transform.position + transform.forward * 10;
		transform.LookAt (forwardPoint);
	}

	private void calcMouseDelta(){
		mouseDelta = new Vector2 (CrossPlatformInputManager.GetAxis ("Mouse X"), CrossPlatformInputManager.GetAxis ("Mouse Y")); 
	}

	private void mouseMoves(){
		if (Input.GetMouseButton (2) == false)
			return;
		
		Vector3 moveDir = Vector3.zero;

		moveDir -= transform.up * mouseDelta.y * mouseMoveSpeed;
		moveDir -= transform.right * mouseDelta.x * mouseMoveSpeed;

	//	transform.Translate (moveDir * Time.deltaTime, Space.World);
		rig.velocity += moveDir;
	}


	private void mouseScroll(){
		Vector3 moveDir = Vector3.zero;
		moveDir += transform.forward * scrollSpeed * Input.mouseScrollDelta.y;
		transform.Translate (moveDir * Time.deltaTime, Space.World);
	}

	private void arrowMoves(){
		Vector3 moveDir = Vector3.zero;

		Vector3 forwardPoint = transform.position + transform.forward * 10;
		forwardPoint.y = transform.position.y;
		Vector3 forwardDir = forwardPoint - transform.position;
		forwardDir.Normalize ();

		Vector3 rightDir = Vector3.Cross (transform.up, forwardDir);
		rightDir.Normalize ();

		int x = 0;
		int y = 0;
		if (Input.GetKey (KeyCode.UpArrow)) y++;
		if (Input.GetKey (KeyCode.DownArrow)) y--;
		if (Input.GetKey (KeyCode.LeftArrow)) x--;
		if (Input.GetKey (KeyCode.RightArrow)) x++;

		moveDir += forwardDir * moveSpeed * y;
		moveDir += rightDir * moveSpeed * x;

	//	transform.Translate (moveDir * Time.deltaTime, Space.World);
		rig.velocity += moveDir;
	}
		
	private void InternalLockUpdate()
	{
		if (Input.GetMouseButton (1) || Input.GetMouseButton (2)) { // Activates only if we are using a mouse command
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			cursorIsLocked = true;
		} else
			cursorIsLocked = false;

		if (cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else if (!cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
