using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpeedRunner {
    public class PlayerGroundCheck : MonoBehaviour {

        private List<Action> onGroundedHandlers = new List<Action>();
        private List<Action> onLeftGroundHandlers = new List<Action>();

        private bool isGrounded = false;
        private bool wasGroundedLastframe = false;

        private void Start() { StartCoroutine(lateUpdateCheck()); }
        private void FixedUpdate() { isGrounded = false; }

        private IEnumerator lateUpdateCheck() {
            while (true) {
                yield return new WaitForFixedUpdate();
                checkWasGrounded();
            }
        }

        private void checkWasGrounded() {
            if (isGrounded == false && wasGroundedLastframe)
                activateEvent(onLeftGroundHandlers);
            else if (isGrounded && wasGroundedLastframe == false)
                activateEvent(onGroundedHandlers);

            wasGroundedLastframe = isGrounded;
        }


        private void OnTriggerStay2D(Collider2D collider) {
            if (collider.gameObject.layer == 8)
                isGrounded = true;
        }



        private void activateEvent(List<Action> grounEvent) { grounEvent.ForEach((a) => a.Invoke()); }
        public void subscribeOnLeftGroundEvent(Action a) { onLeftGroundHandlers.Add(a); }
        public void subscribeOnGroundedEvent(Action a) { onGroundedHandlers.Add(a); }
        public bool getIsGrounded() { return isGrounded; }
    }
}