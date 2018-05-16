using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace FileManager{

	public class FileManagerBitmapSection: MonoBehaviour {
			
		public RectTransform currentTransform;
		public LayoutGroup matchingGroup;
		public GameObject bitmapPrefab;
		public Text sectionText;

		private List<FileManagerUIObj> spawnedBitmaps = new List<FileManagerUIObj> ();


		public void init(string logoText, List<BitmapInfo> mappedSections){
			destroyOldBitmaps ();
			sectionText.text = logoText;

			foreach (BitmapInfo b in mappedSections) {
				FileManagerUIObj temp = Instantiate (bitmapPrefab, transform).GetComponent<FileManagerUIObj> ();
				temp.init (b, bitmapToColor (b));
				spawnedBitmaps.Add (temp);
			}
		}

		private void destroyOldBitmaps(){
			for (int i = spawnedBitmaps.Count - 1; i >= 0; i--)
				Destroy (spawnedBitmaps [i].gameObject);
			spawnedBitmaps.Clear ();
		}


		void Update () {
			Vector2 temp = currentTransform.sizeDelta;
			temp.y = matchingGroup.preferredHeight;
			currentTransform.sizeDelta = temp;
		}



		private Color bitmapToColor(BitmapInfo info){
			if (info.type == BitmapType.Inode)
				return Color.red;
			else
				return Color.green;
		}
			
	}


	public class BitmapInfo{
		public int id, targetBlockID;
		public BitmapType type;
	}

	public enum BitmapType{
		Inode,
		Data
	}
}