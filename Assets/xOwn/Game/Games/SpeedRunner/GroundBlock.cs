using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SpeedRunner {

    public class GroundBlock : MonoBehaviour {

        public readonly float width = 20f;
        public readonly float height = 5.5f;

        private List<Vector2> coordList = new List<Vector2>();

        float xStepValue = 1 / (float)GameConstants.MapProtocol.X_PRECISION;
        float yStepValue = 1 / (float)GameConstants.MapProtocol.Y_PRECISION;

        private void Start() {
            foreach (BoxCollider2D c in GetComponentsInChildren<Collider2D>())
                addBoxCollider(c);
        }

        private void addLocalCoordinate(float x, float y) {
            bool insideGridBounds = y >= 0 && x >= 0;
            if (insideGridBounds == false)
                return;

            coordList.Add(new Vector2(x, y));
        }

        private void addBoxCollider(BoxCollider2D coll) {
            Vector2 localCollPos = coll.bounds.center - transform.position;

            for (float y = -coll.bounds.extents.y; y < coll.bounds.extents.y; y += yStepValue)
                for (float x = -coll.bounds.extents.x; x < coll.bounds.extents.x; x += xStepValue)
                    addLocalCoordinate(localCollPos.x + x, localCollPos.y + y);
        }

        public List<Vector2> getCoordList() { return coordList; }
    }

}