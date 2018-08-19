using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI{
	public class ClientIconManager{

        private static int amountOfImages = 8;

		public static Sprite loadIcon(int iconNumber){
			string imagePath = "PlayerIcons/" + "Ball" + iconNumber;
			return Resources.Load<Sprite> (imagePath);
		}

        public static int getRandomIconNumber() {
            return Random.Range(1, amountOfImages);
        }
	}
}