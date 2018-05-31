using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedRunner {
    public class Player : MonoBehaviour {

        [SerializeField]
        private MapGenerator theMapGenerator;
        [SerializeField]
        private Rigidbody2D theRigid;
        [SerializeField]
        private JumpSkill jumpManager;

        private float runVelocity;
        private bool performJump = false;

        private void Start() {
            initStartValues();
            StartCoroutine(gameUpdate());
            theRigid.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }
        private void initStartValues() {
            runVelocity = GameConstants.PlayerStartValues.startRunSpeed;
        }


        private void velocityUpdate() {
            Vector2 tempVelocity = theRigid.velocity;
            setRunVelocity(ref tempVelocity);
            theRigid.velocity = tempVelocity;
        }

        //Human Controlls
        private void Update() {
            Vector2 tempVelocity = theRigid.velocity;

            if (Input.GetKeyDown(KeyCode.Space) || performJump) {
                jumpManager.jump(ref tempVelocity);
                performJump = false;
            }

            theRigid.velocity = tempVelocity;
        }

        private void setRunVelocity(ref Vector2 velocity) { velocity.x = runVelocity; }

        private IEnumerator gameUpdate() {
            while (true) {
                yield return new WaitForFixedUpdate();
                theMapGenerator.newPlayerPos(transform.position.x);
                velocityUpdate();
            }
        }

        public void activateJump() {
            performJump = true;
        }
    }
}