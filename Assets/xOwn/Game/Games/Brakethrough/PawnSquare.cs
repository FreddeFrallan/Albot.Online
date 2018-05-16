using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakthrough{

	public class PawnSquare : MonoBehaviour {

		public SpriteRenderer theRenderer;


		public void updateSprite(Sprite newSprite){
			if (newSprite == null)
				theRenderer.enabled = false;
			else {
				theRenderer.sprite = newSprite;
				theRenderer.enabled = true;
			}
		}
	}
}