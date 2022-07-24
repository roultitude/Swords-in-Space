using FishNet.Object;
using UnityEngine;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using System.Collections;

namespace SwordsInSpace
{
    public class Spawner : NetworkBehaviour
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

        [SerializeField]
        int UnitsFromAsteroid = 3;

        [System.Serializable]
        public struct EnemySpawnInfo
        {
            public GameObject MobType;
            public int weight;
            public Vector2 localPackSizeRange;
        }

        [SerializeField]
        EnemySpawnInfo[] spawninfos;

        [SerializeField]
        GameObject[] bossPrefabs;

        [SerializeField]
        EnemySpawnInfo[] InactiveEnemyInfos;

        public int InactiveMobPacks = 5;

        public GameObject[] Powerups;
        public float PowerupSpawnChance;

        List<GameObject> currentEnemies;

        public int bossesKilled;
        int totalWeight;

        [SerializeField]
        Vector2 worldSize = new Vector2(1000, 1000);

        [SerializeField]
        Timer countdownTimer;

        [SyncVar]
        public double timeTillBoss;

        [SyncVar]
        public bool spawningComplete = false;

        public bool IsAllBossesKilled() => bossesKilled == bossPrefabs.Length;

        private int MaxGetRandomPositionAttempts = 5;
        public override void OnStartServer()
        {
            base.OnStartServer();
            totalWeight = 0;
            foreach (EnemySpawnInfo info in spawninfos)
            {
                totalWeight += info.weight;
            }
            bossesKilled = 0;
            StartCoroutine(OnStartSpawn());
        }


        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsServer)
                return;
            foreach (Timer time in GetComponents<Timer>())
            {
                time.Stop();
            }
        }

        public void SpawnMob()
        {
            if (spawninfos.Length == 0)
                return;

            int currentWeight = Random.Range(0, totalWeight + 1);
            EnemySpawnInfo selectedInfo = spawninfos[0];
            foreach (EnemySpawnInfo info in spawninfos)
            {
                currentWeight -= info.weight;
                if (currentWeight <= 0)
                {
                    selectedInfo = info;
                    break;
                }
            }
            Vector3 loc = getRandomPosition();
            int size = Random.Range(minPackDensity, maxPackDensity);
            for (int i = 0; i < size * (int)Random.Range(selectedInfo.localPackSizeRange.x, selectedInfo.localPackSizeRange.y + 1); i++)
            {
                Vector2 randomLocInSpawnRadius = getValidLocInSpawnRadius(loc);
                GameObject toAdd = Instantiate(selectedInfo.MobType, new Vector3(randomLocInSpawnRadius.x, randomLocInSpawnRadius.y,
                    loc.z), transform.rotation);
                Spawn(toAdd);

            }

        }



        private IEnumerator OnStartSpawn()
        {
            if (InactiveEnemyInfos.Length == 0)
                yield break;

            int inactiveEnemyWeight = 0;
            foreach (EnemySpawnInfo info in InactiveEnemyInfos)
            {
                inactiveEnemyWeight += info.weight;
            }

            //Spawn Sleeping Mobs
            for (int mobNo = 0; mobNo < InactiveMobPacks; mobNo++)
            {
                int currentWeight = Random.Range(0, inactiveEnemyWeight + 1);
                EnemySpawnInfo selectedInfo = InactiveEnemyInfos[0];
                foreach (EnemySpawnInfo info in InactiveEnemyInfos)
                {
                    currentWeight -= info.weight;
                    if (currentWeight <= 0)
                    {
                        selectedInfo = info;
                        break;
                    }
                }

                Vector3 loc = getRandomPosition(MaxGetRandomPositionAttempts, new Vector2(worldSize.x / 2, worldSize.y / 2), minRange: new Vector2(50, 50));
                int size = Random.Range(minPackDensity, maxPackDensity);

                for (int i = 0; i < size * (int)Random.Range(selectedInfo.localPackSizeRange.x, selectedInfo.localPackSizeRange.y + 1); i++)
                {
                    Vector2 randomLocInSpawnRadius = getValidLocInSpawnRadius(loc);
                    GameObject toAdd = Instantiate(selectedInfo.MobType, new Vector3(randomLocInSpawnRadius.x, randomLocInSpawnRadius.y,
                        loc.z), transform.rotation);

                    Spawn(toAdd);
                    toAdd.GetComponent<EnemyAI>().SetInactive();

                }

                if (Random.Range(0f, 1f) < PowerupSpawnChance)
                {
                    GameObject toSpawn = Instantiate(Powerups[Random.Range(0, Powerups.Length)], loc, Quaternion.Euler(0, 0, 0));
                    Spawn(toSpawn);
                }

                yield return new WaitForSeconds(0.05f);
            }



        }

        private Vector2 getValidLocInSpawnRadius(Vector3 loc)
        {

            bool foundValid = false;
            int radiusExpanded = 0;
            int increaseSpawnRadiusAttempts = 3;
            Vector2 currentPackSpawnRadius = packSpawnRadius;
            Vector2 randomLocInSpawnRadius = new Vector2(loc.x, loc.y); ;
            while (!foundValid)
            {
                for (int attemptsInVicinity = 0; attemptsInVicinity < 10; attemptsInVicinity++)
                {
                    randomLocInSpawnRadius = new Vector2(loc.x + Random.Range(-currentPackSpawnRadius.x / 2, currentPackSpawnRadius.x / 2),
                    loc.y + Random.Range(-currentPackSpawnRadius.y / 2, currentPackSpawnRadius.y / 2));

                    if (isValidSpawnPosition(randomLocInSpawnRadius))
                    {
                        foundValid = true;
                        return randomLocInSpawnRadius;
                    }

                }
                radiusExpanded += 1;

                currentPackSpawnRadius *= 2;

                if (radiusExpanded > increaseSpawnRadiusAttempts)
                    break;


            }

            return randomLocInSpawnRadius;

        }

        private Vector3 getRandomPosition()
        {

            return getRandomPosition(MaxGetRandomPositionAttempts, new Vector2(worldSize.x / 3, worldSize.y / 3));
        }

        private Vector3 getRandomPosition(int attempts, Vector2 SpawnArea, Vector2 minRange = default)
        {

            Vector3 pos;
            if (minRange == default)
                pos = new Vector3(Random.Range(-SpawnArea.x, SpawnArea.x), Random.Range(-SpawnArea.y, SpawnArea.y), 0);
            else
            {
                pos = new Vector3(Random.Range(minRange.x, SpawnArea.x), Random.Range(minRange.y, SpawnArea.y), 0);
                if (Random.Range(0, 2) == 1)
                {
                    pos.x = -pos.x;
                }

                if (Random.Range(0, 2) == 1)
                {
                    pos.y = -pos.y;

                }

            }

            if (!isValidSpawnPosition(pos) && attempts > 0)
            {
                return getRandomPosition(attempts--, SpawnArea);
            }
            return pos;

        }
        private bool isValidSpawnPosition(Vector2 pos)
        {
            return Physics2D.OverlapCircle(new Vector2(pos.x, pos.y), UnitsFromAsteroid) == null;
        }

        public void SpawnBosses()
        {
            foreach (GameObject boss in bossPrefabs)
            {
                Vector3 loc = getRandomPosition();
                Vector2 randomLocInSpawnRadius = getValidLocInSpawnRadius(loc);
                GameObject toAdd = Instantiate(boss, new Vector3(randomLocInSpawnRadius.x, randomLocInSpawnRadius.y,
                    loc.z), transform.rotation);
                toAdd.GetComponent<EnemyAI>().SetBoss(true);
                Spawn(toAdd);
                GameManager.instance.MessageRPC("Boss " + boss.name + " has awoken!");
            }
        }

        public void StopSpawn()
        {
            SpawnCD.Stop();
            SpawnBosses();
            spawningComplete = true;
        }

        private void Update()
        {
            if (!IsServer) return;
            timeTillBoss = countdownTimer.waitTime - countdownTimer.currentTime;
        }
    }
};
