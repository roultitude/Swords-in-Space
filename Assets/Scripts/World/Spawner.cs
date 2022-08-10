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
            public int cost;
            public Vector2 localPackSizeRange;
        }

        [SerializeField]
        EnemySpawnInfo[] spawninfos;

        public Vector2 SpawnMobProximityToPlayers;
        public float startCost;
        public float costGrowthMultiplier;
        private float currentGrowthMultiplier = 1;

        [SerializeField]
        GameObject[] bossPrefabs;

        [SerializeField]
        EnemySpawnInfo[] InactiveEnemyInfos;

        public int InactiveMobPacks = 5;
        public Vector2 InactiveMobCostRange = new Vector2(3, 6);

        public GameObject[] Powerups;
        public float PowerupSpawnChance;

        public GameObject QuestObjectivePowerup;
        public int NumQuestObjectivePowerupToSpawn;
        public int questObjectiveCollected = 0;

        List<GameObject> currentEnemies;

        public int bossesKilled;
        int totalWeight;

        [SerializeField]
        Vector2 worldSize = new Vector2(1000, 1000);

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
                totalWeight += info.cost;
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

        public void OnMobPackTimerComplete()
        {
            if (spawninfos.Length == 0)
                return;

            currentGrowthMultiplier *= costGrowthMultiplier;

            Vector2 toSpawnLoc = new Vector2(Random.Range(SpawnMobProximityToPlayers.x, SpawnMobProximityToPlayers.y), Random.Range(SpawnMobProximityToPlayers.x, SpawnMobProximityToPlayers.y));

            if (Random.Range(0, 2) == 0)
                toSpawnLoc.x = -toSpawnLoc.x;

            if (Random.Range(0, 2) == 0)
                toSpawnLoc.y = -toSpawnLoc.y;


            Vector2 mobPackLoc = new Vector2(Ship.currentShip.gameObject.transform.position.x, Ship.currentShip.gameObject.transform.position.y) + toSpawnLoc;
            SpawnMobPack(spawninfos, startCost * currentGrowthMultiplier, mobPackLoc);

        }

        public void SpawnMobPack(EnemySpawnInfo[] spawnInfo, float currentCost, Vector2 toSpawnLoc, bool IsActive = true)
        {
            /*
             * Cost Based System deducts currentCost until the cost is negative, each mob will have equal chance to spawn.
             * Spawning a higher cost mob will just mean that the pack will be smaller.
             */

            while (currentCost > 0)
            {
                EnemySpawnInfo currentEnemyInfo = spawnInfo[Random.Range(0, spawnInfo.Length)];
                currentCost -= currentEnemyInfo.cost;

                int size = Random.Range(minPackDensity, maxPackDensity);
                for (int i = 0; i < size * (int)Random.Range(currentEnemyInfo.localPackSizeRange.x, currentEnemyInfo.localPackSizeRange.y + 1); i++)
                {
                    Vector2 randomLocInSpawnRadius = getValidLocInSpawnRadius(toSpawnLoc);
                    GameObject toAdd = Instantiate(currentEnemyInfo.MobType, new Vector3(randomLocInSpawnRadius.x, randomLocInSpawnRadius.y,
                        0), transform.rotation);

                    if (!IsActive)
                        toAdd.GetComponent<EnemyAI>().SetInactive();

                    Spawn(toAdd);

                }

            }


        }


        private IEnumerator OnStartSpawn()
        {
            if (InactiveEnemyInfos.Length == 0)
                yield break;

            yield return new WaitForSeconds(0.05f);

            SpawnQuestObjectives();

            //Spawn Sleeping Mobs
            for (int mobNo = 0; mobNo < InactiveMobPacks; mobNo++)
            {

                Vector3 loc = getRandomPosition(MaxGetRandomPositionAttempts, new Vector2(worldSize.x / 2, worldSize.y / 2), minRange: new Vector2(100, 100));
                int size = Random.Range(minPackDensity, maxPackDensity);

                SpawnMobPack(InactiveEnemyInfos, Random.Range(InactiveMobCostRange.x, InactiveMobCostRange.y), loc, false);

                if (Random.Range(0f, 1f) < PowerupSpawnChance)
                {
                    GameObject toSpawn = Instantiate(Powerups[Random.Range(0, Powerups.Length)], loc, Quaternion.Euler(0, 0, 0));
                    Spawn(toSpawn);
                }

                yield return new WaitForSeconds(0.05f);
            }



        }

        private void SpawnQuestObjectives()
        {
            if (NumQuestObjectivePowerupToSpawn == 0)
            {
                OnCompleteCollectObjectives();
                return;
            }

            for (int questObjectiveSpawned = 0; questObjectiveSpawned < NumQuestObjectivePowerupToSpawn; questObjectiveSpawned++)
            {
                Vector3 loc = getRandomPosition(MaxGetRandomPositionAttempts, new Vector2(worldSize.x / 2, worldSize.y / 2), minRange: new Vector2(100, 100));
                GameObject toSpawn = Instantiate(QuestObjectivePowerup, loc, Quaternion.Euler(0, 0, 0));
                Spawn(toSpawn);
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

        public void collectObjective()
        {
            questObjectiveCollected += 1;
            if (questObjectiveCollected >= NumQuestObjectivePowerupToSpawn)
                OnCompleteCollectObjectives();




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

        public void OnCompleteCollectObjectives()
        {
            SpawnCD.Stop();
            spawningComplete = true;
            SpawnBosses();
        }

    }
};
