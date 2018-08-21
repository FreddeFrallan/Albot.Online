using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientUI {

    public class ClientUIScrollLobbyManager : MonoBehaviour {

        public float itemExtraHeight = 10;
        public float noChildHeight = 10;
        public float scrollSpeed = 0.5f;
        public RectTransform theRect;
        public Scrollbar theScroll;

	    // Update is called once per frame
	    void Update () {
            scaleContent();
            scrollWheelControlls();
        }


        private void scrollWheelControlls() {
            theScroll.value += Input.mouseScrollDelta.y * scrollSpeed;
            Mathf.Clamp(theScroll.value, 0, 1);
        }

        private void scaleContent() {
            if (transform.childCount > 0) {
                GameObject lastChild = getLastChild();
                float yTemp = lastChild.GetComponent<RectTransform>().anchoredPosition.y;
                setHeight(-yTemp + itemExtraHeight);
            }
            else
                setHeight(noChildHeight);
        }

        private GameObject getLastChild() {return transform.GetChild(transform.childCount - 1).gameObject;}
        private void setHeight(float h) {
            Rect r = theRect.rect;
            theRect.sizeDelta = new Vector2(r.width, h);
        }
    }
}
