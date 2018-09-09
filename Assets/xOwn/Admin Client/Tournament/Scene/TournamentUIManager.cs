using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tournament.Client {

    public class TournamentUIManager : MonoBehaviour {

        [SerializeField]
        private Camera tournamentCamera;
        private TournamentUIObject lastSelected;
        public float camSpeed = 1f;

        void Update() {
            keyBoardControlls();
            if (Input.GetMouseButtonDown(0) && getSelectedObject())
                lastSelected.onLeftClick();
            if (Input.GetMouseButtonDown(1) && getSelectedObject())
                lastSelected.onRightClick();
        }

        private void keyBoardControlls() {
            if (Input.GetKey(KeyCode.RightArrow))
                moveCamera(1, 0, camSpeed);
            if (Input.GetKey(KeyCode.LeftArrow))
                moveCamera(-1, 0, camSpeed);
            if (Input.GetKey(KeyCode.DownArrow))
                moveCamera(0, -1, camSpeed);
            if (Input.GetKey(KeyCode.UpArrow))
                moveCamera(0, 1, camSpeed);
        }

        private void moveCamera(float x, float y, float speed) {
            transform.Translate(new Vector3(x, y, 0) * camSpeed * Time.deltaTime);
        }

        #region MouseControlls
        private bool getSelectedObject() {
            RaycastHit hit;
            Ray ray = tournamentCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                TournamentUIObject temp = hit.collider.gameObject.GetComponent<TournamentUIObject>();
                if(temp != null) {
                    lastSelected = temp;
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}