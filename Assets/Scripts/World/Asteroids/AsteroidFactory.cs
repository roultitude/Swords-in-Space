using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SwordsInSpace
{
    public class AsteroidFactory : NetworkBehaviour
    {

        [SerializeField]
        GameObject[] asteroids;

        // Start is called before the first frame update

        [SerializeField]
        public int worldSize = 50;

        [SerializeField]
        bool randomSeed = true;
        [SerializeField, SyncVar]
        public Vector2 seed = new Vector2(1f, 1f);

        public float[,] noiseGrid;



        [SerializeField]
        public float threshold = 0.01f;

        [SerializeField]
        public float density = 2f; //How often asteroids appear. Lower density gives bigger (to implement) but fewer asteroids.

        [SerializeField]
        public float distance = 64;


        [SerializeField]
        public RuleTile tile;

        [SerializeField]
        public int spawnCullingRadius;

        private Vector2 offset;

        private int hp;
        public override void OnStartServer()
        {
            base.OnStartServer();
            if (IsServer)
            {
                if (randomSeed)
                    seed = new Vector2(Random.Range(0, 9000), Random.Range(0, 9000));
                noiseGrid = new float[worldSize, worldSize];
                offset = new Vector2(-worldSize / 2 * distance, -worldSize / 2 * distance);
                MakeNoiseArray();
                MakeAsteroids();
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            noiseGrid = new float[worldSize, worldSize];
            offset = new Vector2(-worldSize / 2 * distance, -worldSize / 2 * distance);
            MakeNoiseArray();
        }



        private void MakeNoiseArray()
        {
            for (int i = 0; i < worldSize; i++)
            {
                for (int j = 0; j < worldSize; j++)
                {
                    noiseGrid[i, j] = Mathf.PerlinNoise((seed.x + i) * (1.0f / density) + 0.1f,
                        (seed.y + j) * (1.0f / density) + 0.1f);
                }
            }
        }


        private void MakeAsteroids()
        {
            bool[,] hasSeen = new bool[worldSize, worldSize];

            for (int i = 0; i < worldSize; i++)
            {
                for (int j = 0; j < worldSize; j++)
                {

                    if (AboveThreshold(i, j) && !hasSeen[i, j] && (i * i + j * j) > spawnCullingRadius * spawnCullingRadius)
                    {
                        GameObject toAdd = Instantiate(asteroids[0]
                            , new Vector3(i * distance + offset.x, j * distance + offset.y, 0)
                            //, new Vector3(i, j, 0)
                            //, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                            , Quaternion.Euler(0, 0, 0));
                        Tilemap currentMap = toAdd.GetComponentInChildren<Tilemap>();
                        if (currentMap)
                            Traverse(hasSeen, currentMap, i, j, 0, 0);

                        toAdd.GetComponentInChildren<Asteroid>().Setup(0f);

                        Spawn(toAdd);
                    }


                }

            }

            ProcessAsteroids();


        }

        [ObserversRpc(IncludeOwner = true, BufferLast = true)]
        private void ProcessAsteroids()
        {
            Asteroid[] asteroids = Object.FindObjectsOfType<Asteroid>();
            bool[,] hasSeen = new bool[worldSize, worldSize];
            foreach (Asteroid asteroid in asteroids)
            {
                Vector2 loc = new Vector2((asteroid.gameObject.transform.position.x - offset.x) / distance
                , (asteroid.gameObject.transform.position.y - offset.y) / distance);
                Tilemap currentMap = asteroid.GetComponentInChildren<Tilemap>();
                if (currentMap)
                {
                    hp = 0;
                    Traverse(hasSeen, currentMap, (int)loc.x, (int)loc.y, 0, 0);

                    if (IsServer)
                        asteroid.hp = hp;
                }




                asteroid.gameObject.transform.position = new Vector3(loc.x * distance + offset.x,
                loc.y * distance + offset.y, 0);

            }
            if (IsServer)
                AstarPath.active.Scan();

        }

        private bool AboveThreshold(int i, int j)
        {
            return noiseGrid[i, j] >= threshold;
        }

        private void Traverse(bool[,] hasSeen, Tilemap currentMap, int i, int j, int x, int y)
        {
            hasSeen[i, j] = true;
            hp++;
            currentMap.SetTile(new Vector3Int(x, y, 0), tile);

            if (i > 0 && !hasSeen[i - 1, j] && AboveThreshold(i - 1, j))
            {
                Traverse(hasSeen, currentMap, i - 1, j, x - 1, y);
            }
            if (i < worldSize - 1 && !hasSeen[i + 1, j] && AboveThreshold(i + 1, j))
            {
                Traverse(hasSeen, currentMap, i + 1, j, x + 1, y);
            }
            if (j < worldSize - 1 && !hasSeen[i, j + 1] && AboveThreshold(i, j + 1))
            {
                Traverse(hasSeen, currentMap, i, j + 1, x, y + 1);
            }
            if (j > 0 && !hasSeen[i, j - 1] && AboveThreshold(i, j - 1))
            {
                Traverse(hasSeen, currentMap, i, j - 1, x, y - 1);
            }


        }
    }
};
