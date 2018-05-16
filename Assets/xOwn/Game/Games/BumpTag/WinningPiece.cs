using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningPiece : MonoBehaviour {

	float fadeSpeed = 0.5f;
	float min = 0f, max = 0.8f;
	bool goingDown = false;

	Renderer meshR;

	void Start(){
		meshR = GetComponent<Renderer> ();
		Color c = meshR.material.color;
		c.a = 0;
		meshR.material.color = c;
	}


	// Update is called once per frame
	void FixedUpdate () {
		Color c = meshR.material.color;

		c.a += goingDown ? -fadeSpeed * Time.fixedDeltaTime : fadeSpeed * Time.fixedDeltaTime;
		c.a = Mathf.Clamp (c.a, min, max);

		if (c.a == min && goingDown)
			goingDown = false;
		if (c.a == max && goingDown == false)
			goingDown = true;

		meshR.material.color = c;
	}
}
