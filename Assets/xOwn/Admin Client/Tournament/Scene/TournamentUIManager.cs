using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tournament.Client {

    public class TournamentUIManager : MonoBehaviour {

        [SerializeField]
        private Camera tournamentCamera;
        private TournamentUIObject lastSelected;



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