using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI{
	public class ClientIconManager{
		public static Sprite loadIcon(int iconNumber){
			string imagePath = "PlayerIcons/" + "Icon.1_";
			imagePath += iconNumber < 10 ? "0"+ iconNumber.ToString() : iconNumber.ToString();
			return Resources.Load<Sprite> (imagePath);
		}
	}
}