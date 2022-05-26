using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
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
        [SerializeField]
        public Vector2 seed = new Vector2(1f, 1f);

        public float[,] noiseGrid;



        [SerializeField]
        public float threshold = 0.01f;

        [SerializeField]
        public float density = 2f; //How often asteroids appear. Lower density gives bigger (to implement) but fewer asteroids.

        [SerializeField]
        public float distance = 64;

        [SerializeField]
        public float chance = 0.2f;

        private Vector2 offset;

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



        private void MakeNoiseArray()
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    noiseGrid[i, j] = Mathf.PerlinNoise((seed.x + i) * (1.0f / density) + 0.1f,
                        (seed.y + j) * (1.0f / density) + 0.1f);
                }
            }
        }


        private void MakeAsteroids()
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (noiseGrid[i, j] >= threshold)
                    {
                        Debug.Log(noiseGrid[i, j]);
                        GameObject toAdd = Instantiate(asteroids[0]
                            , new Vector3(i * distance + offset.x, j * distance + offset.y, 0)
                            , Quaternion.Euler(0, 0, Random.Range(0, 360)));
                        if (Random.Range(0, 1 / chance) == 0)
                            toAdd.GetComponent<Asteroid>().Setup(Random.Range(0f, 1f));
                        else
                            toAdd.GetComponent<Asteroid>().Setup(0f);

                        Spawn(toAdd);
                    }
                }
            }
        }




    }
};
