using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Scened;
using FishNet.Connection;
using UnityEngine.Tilemaps;


namespace SwordsInSpace
{
    public class FireManager : NetworkBehaviour
    {
        public Tilemap shipGrid;
        public GameObject fire;

        public InteractableIdManager manager;
        private bool[,] spots;

        private bool[,] fireLoc;

        private Stack<Vector2> activeFire;

        public Vector2 tileSize = new Vector2(32, 32);

        public bool isOnFire = false;

        public float multipleFireSpreadChance = 0.1f;

        public double tickChance = 0.1f;
        public double tickDur = 0.8f;

        public bool DebugStartFire = false;

        private Vector2 offset;

        public void Awake()
        {
            shipGrid.CompressBounds();
            var bounds = shipGrid.cellBounds;
            activeFire = new Stack<Vector2>();
            spots = new bool[bounds.size.x, bounds.size.y];
            fireLoc = new bool[bounds.size.x, bounds.size.y];

            offset = new Vector2(bounds.size.x / 2f + 0.5f, bounds.size.y / 2f - 0.5f);

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    var px = bounds.xMin + x;
                    var py = bounds.yMin + y;
                    spots[x, y] = shipGrid.HasTile(new Vector3Int(px, py, 0));

                }
            }
        }

        public void Update()
        {
            if (DebugStartFire)
            {
                DebugStartFire = false;
                trySpawnFire();
                Debug.Log("debug toggle");
            }
        }


        public void trySpawnFire()
        {

            int attempts = 20;

            for (int i = 0; i < attempts; i++)
            {
                Vector2 spawnLoc = new Vector2(Random.Range(0, spots.GetLength(0))
                , Random.Range(0, spots.GetLength(1)));
                if (trySpawnFire(spawnLoc))
                {
                    Debug.Log("spawned");
                    return;
                }
            }
        }
        public bool trySpawnFire(Vector2 loc)
        {
            if (!isValidFireLoc(loc))
            {
                return false;
            }

            fireLoc[(int)loc.x, (int)loc.y] = true;
            activeFire.Push(loc);

            GameObject toSpawn = Instantiate(fire, Ship.currentShip.shipInterior.transform);

            toSpawn.transform.localPosition = new Vector3(loc.x - offset.x, loc.y - offset.y, 0);

            Spawn(toSpawn);

            manager.RefreshData();
            CheckIsOnFire();

            return true;
        }

        public IEnumerator FireTimer()
        {
            while (isOnFire)
            {
                if (Random.Range(0f, 1f) < tickChance)
                {
                    Debug.Log("tick");
                    doFireTick();
                }
                yield return new WaitForSeconds((float)tickDur);
            }

            ResetFire();
            yield break;
        }

        private void ResetFire()
        {
            activeFire = new Stack<Vector2>();
            fireLoc = new bool[spots.GetLength(0), spots.GetLength(1)];

        }
        private bool isValidFireLoc(Vector2 loc)
        {
            if (loc.x >= 0 && loc.y >= 0 && loc.x < spots.GetLength(0) && loc.y < spots.GetLength(1))
                return spots[(int)loc.x, (int)loc.y] && !fireLoc[(int)loc.x, (int)loc.y];
            return false; //Out of range
        }

        public void doFireTick()
        {

            Stack<Vector2> copyActiveFire = new Stack<Vector2>();
            while (activeFire.Count != 0)
            {
                copyActiveFire.Push(activeFire.Pop());
            }

            while (copyActiveFire.Count != 0)
            {
                Vector2 loc = copyActiveFire.Pop();
                List<Vector2> possibledir = new List<Vector2>(); // n=4
                possibledir.Add(new Vector2(0, 1));
                possibledir.Add(new Vector2(0, -1));
                possibledir.Add(new Vector2(1, 0));
                possibledir.Add(new Vector2(-1, 0));

                while (possibledir.Count > 0)
                {
                    int randindex = Random.Range(0, possibledir.Count);
                    Vector2 dir = possibledir[randindex];
                    possibledir.RemoveAt(randindex);

                    if (trySpawnFire(loc + dir))
                    {

                        if (Random.Range(0f, 1f) < multipleFireSpreadChance)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void fireExtinguished(Vector2 loc)
        {
            loc += offset;

            fireLoc[(int)loc.x, (int)loc.y] = false;
            CheckIsOnFire();
        }



        private void CheckIsOnFire()
        {
            bool prevVal = isOnFire;
            isOnFire = false;

            foreach (bool i in fireLoc)
            {
                if (i)
                    isOnFire = true;
            }

            if (prevVal == false && isOnFire)
            {
                Debug.Log("Starting track fire");
                StopCoroutine("FireTimer");
                StartCoroutine("FireTimer");
            }

        }











    }
};
