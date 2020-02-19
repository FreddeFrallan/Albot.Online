using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSchema2048 : MonoBehaviour {

    public List<Color> colors = new List<Color>();
    public static List<Color> brickColors = new List<Color>();

	// Use this for initialization
	void Awake () {
        brickColors = colors;
	}

}
