using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedRunner {
    public abstract class CooldownObject : MonoBehaviour {
        protected float cooldownTime;
        protected bool isInCooldown;

        public void init(float cooldown, bool isInCooldown = false) {
            this.cooldownTime = cooldown;
            this.isInCooldown = isInCooldown;
        }

        private IEnumerator cooldown() {
            yield return new WaitForSeconds(cooldownTime);
            isInCooldown = false;
        }

        public void activateCooldown() {
            isInCooldown = true;
            StartCoroutine(cooldown());
        }
        public bool canActivate() { return !isInCooldown; }
    }
}