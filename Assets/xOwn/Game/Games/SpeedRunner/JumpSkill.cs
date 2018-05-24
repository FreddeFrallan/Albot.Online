using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedRunner { 

    public class JumpSkill : CooldownObject {

        [SerializeField]
        private PlayerGroundCheck groundCheck;
        private float jumpSpeed;
        private int jumpCounter;
        private int maxJumps;

        private void Start() {
            base.init(GameConstants.PlayerStartValues.startJustCooldown, false);
            jumpSpeed = GameConstants.PlayerStartValues.startJumpSpeed;
            maxJumps = GameConstants.PlayerStartValues.startAmountJumps;
            jumpCounter = maxJumps;

            groundCheck.subscribeOnGroundedEvent(onGrounded);
        }

        private void onGrounded() {jumpCounter = maxJumps;}

        public void jump(ref Vector2 pos) {
            if (canJump()) { 
                activateCooldown();
                pos.y = jumpSpeed;
                jumpCounter--;
            }
        }

        public bool canJump() { return (isInCooldown == false && jumpCounter > 0 && groundCheck.getIsGrounded()); }
    }
}