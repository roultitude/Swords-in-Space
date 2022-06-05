using FishNet.Object;
using UnityEngine;
namespace SwordsInSpace
{
    public class EnemySpawner : NetworkBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]
        Timer SpawnCD;

        [SerializeField]
        int minPackDensity;

        [SerializeField]
        int maxPackDensity;

        [SerializeField]
        Vector2 packSpawnRadius;

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
            Vector3 loc = getRandomPosition();
            int size = Random.Range(minPackDensity, maxPackDensity);
            for (int i = 0; i < size; i++)
            {
                GameObject toAdd = Instantiate(toSpawn, new Vector3(loc.x + Random.Range(-packSpawnRadius.x / 2, packSpawnRadius.x / 2),
                    loc.y + Random.Range(-packSpawnRadius.y / 2, packSpawnRadius.y / 2),
                    loc.z), transform.rotation);
                Spawn(toAdd);

            }

        }

        private Vector3 getRandomPosition()
        {
            return new Vector3(Random.Range(-worldSize.x / 3, worldSize.x / 3), Random.Range(-worldSize.y / 3, worldSize.y / 3), 0);
        }

        public void StopSpawn()
        {
            SpawnCD.Stop();
        }
    }
};
