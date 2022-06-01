using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
namespace SwordsInSpace
{
    public class EnemySpawner : NetworkBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]
        Timer SpawnCD;

        [SerializeField]
        int minDensity;

        [SerializeField]
        int maxDensity;

        [System.Serializable]
        public struct SpawnInfo
        {
            public GameObject MobType;
            public int weight;
        }

        [SerializeField]
        SpawnInfo[] spawninfos;

        int totalWeight;

        [SerializeField]
        Vector2 worldSize = new Vector2(1000, 1000);

        void Start()
        {
            totalWeight = 0;
            foreach (SpawnInfo info in spawninfos)
            {
                totalWeight += info.weight;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Spawn()
        {
            int currentWeight = totalWeight;
            GameObject toSpawn = spawninfos[0].MobType;
            foreach (SpawnInfo info in spawninfos)
            {
                currentWeight -= info.weight;
                if (currentWeight <= 0)
                {
                    toSpawn = info.MobType;
                    break;
                }
            }

            int size = Random.Range(minDensity, maxDensity);
            for (int i = 0; i < size; i++)
            {
                GameObject toAdd = Instantiate(toSpawn, getRandomPosition(), transform.rotation);
                Spawn(toAdd);

            }

        }

        private Vector3 getRandomPosition()
        {
            return new Vector3(Random.Range(-worldSize.x / 4, worldSize.x / 4), Random.Range(-worldSize.y / 4, worldSize.y / 4), 0);
        }

        public void StopSpawn()
        {

        }
    }
};
