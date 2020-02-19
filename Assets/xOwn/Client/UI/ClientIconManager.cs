using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI{
	public class ClientIconManager{

        private static int amountOfImages = 52;
        private static int iconAssignCounter = 1;

		public static Sprite loadIcon(int iconNumber){
			string imagePath = "PlayerIcons/" + "Resurs " + iconNumber;
			return Resources.Load<Sprite> (imagePath);
		}

        public static int getRandomIconNumber() {
            return Random.Range(1, amountOfImages);
        }

        public static int giveNewPlayerIconNumber() {
            iconAssignCounter++;
            if (iconAssignCounter > amountOfImages)
                iconAssignCounter = 1;
            return iconAssignCounter;
        }
	}
}