using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SpeedRunner {

    public class MapGenerator : MonoBehaviour {
        public static MapGenerator singleton;

        [SerializeField]
        private MapBlocks blocks;
        private ObjectPool groundSegmentPool;

        private float currentMapEnd = 20;


        void Start() {
            singleton = this;
            groundSegmentPool = new ObjectPool(GameConstants.MapGenerator.POOL_SIZE);
        }


        private void spawnNewMapSegment() {
            Vector2 spawnPos = getNextSpawnPos();
            GroundBlock temp = Instantiate(blocks.getNewBlock(), spawnPos, Quaternion.identity).GetComponent<GroundBlock>();
            groundSegmentPool.addNewObject(temp);

            currentMapEnd = spawnPos.x + temp.width;
        }


        private Vector2 getNextSpawnPos() { return new Vector2(currentMapEnd, 0); }
        public void newPlayerPos(float xPos) {
            if (xPos >= currentMapEnd - GameConstants.MapGenerator.SPAWN_DISTANCE)
                spawnNewMapSegment();
        }

        public List<GroundBlock> getCurrentBlocks() {return groundSegmentPool.getCurrentObjects();}
    }



    public class ObjectPool{
        private int maxPoolObjects;
        private List<GroundBlock> spawnedObjects = new List<GroundBlock>();

        public ObjectPool(int poolSize) {
            maxPoolObjects = poolSize;
        }

        public void addNewObject(GroundBlock obj) {
            spawnedObjects.Add(obj);
            if (spawnedObjects.Count > maxPoolObjects) {
                GameObject.Destroy(spawnedObjects[0].gameObject);
                spawnedObjects.RemoveAt(0);
            }
        }

        public List<GroundBlock> getCurrentObjects() { return spawnedObjects; }
    }
}