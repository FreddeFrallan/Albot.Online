using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedRunner {
    public class MapBlocks : MonoBehaviour {
        [SerializeField]
        private List<GameObject> spawnableBlocks = new List<GameObject>();

        public GameObject getNewBlock() {
            return spawnableBlocks[Random.Range(0, spawnableBlocks.Count)];
        }
    }
}