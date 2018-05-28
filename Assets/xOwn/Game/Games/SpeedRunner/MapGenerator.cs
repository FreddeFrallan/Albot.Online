using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedRunner {

    public class MapGenerator : MonoBehaviour {

        [SerializeField]
        private MapBlocks blocks;
        private ObjectPool groundSegmentPool;

        private float currentMapEnd;


        void Start() {
            groundSegmentPool = new ObjectPool(GameConstants.MapGenerator.POOL_SIZE);
        }


        private void spawnNewMapSegment() {
            Vector2 spawnPos = getNextSpawnPos();
            GameObject temp = Instantiate(blocks.getNewBlock(), spawnPos, Quaternion.identity);
            groundSegmentPool.addNewObject(temp);

            currentMapEnd = spawnPos.x + temp.GetComponent<GroundBlock>().width;
        }


        private Vector2 getNextSpawnPos() { return new Vector2(currentMapEnd, 0); }
        public void newPlayerPos(float xPos) {
            if (xPos >= currentMapEnd - GameConstants.MapGenerator.SPAWN_DISTANCE)
                spawnNewMapSegment();
        }
    }



    public class ObjectPool {
        private int maxPoolObjects;
        private List<GameObject> spawnedObjects = new List<GameObject>();

        public ObjectPool(int poolSize) {
            maxPoolObjects = poolSize;
        }

        public void addNewObject(GameObject obj) {
            spawnedObjects.Add(obj);
            if (spawnedObjects.Count > maxPoolObjects) {
                GameObject.Destroy(spawnedObjects[0]);
                spawnedObjects.RemoveAt(0);
            }
        }
    }
}