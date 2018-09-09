using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tournament.Client {

    public class TournamentUIManager : MonoBehaviour {

        [SerializeField]
        private Camera tournamentCamera;
        private TournamentUIObject lastSelected;
        public float camSpeed = 1f;

        public void LateUpdate() {
            if (Input.GetKey(KeyCode.RightArrow)) {
                transform.Translate(new Vector3(camSpeed * 5 * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                transform.Translate(new Vector3(-camSpeed * 5 * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                transform.Translate(new Vector3(0, -camSpeed * 5 * Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.UpArrow)) {
                transform.Translate(new Vector3(0, camSpeed * 5 * Time.deltaTime, 0));
            }
        }

        #region MouseControlls
        void Update() {
            if (Input.GetMouseButtonDown(0) && getSelectedObject())
                lastSelected.onLeftClick();
            if (Input.GetMouseButtonDown(1) && getSelectedObject())
                lastSelected.onRightClick();
        }

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