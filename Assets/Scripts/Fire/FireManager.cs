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

        private int fireid = 0;

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
        private int test = 1000;

        public void Update()
        {
            if (DebugStartFire)
            {
                DebugStartFire = false;

            }

            if (IsServer && test > 0)
            {
                test--;
                if (test == 2)
                    trySpawnFire();
            }

        }


        public void trySpawnFire()
        {

            int attempts = 20;

            for (int i = 0; i < attempts; i++)
            {
                Vector2 spawnLoc = new Vector2(Random.Range(0, spots.GetLength(0))
                , Random.Range(0, spots.GetLength(1)));

                Debug.Log(spawnLoc);
                if (trySpawnFire(spawnLoc))
                {
                    Debug.Log("spawned fire");
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
            fireid++;
            toSpawn.name = "fire" + fireid.ToString("D3");
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
                    StopCoroutine("doFireTick");
                    StartCoroutine("doFireTick");

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

        public IEnumerator doFireTick()
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
                        yield return new WaitForSeconds((float)tickDur + 1f);
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

                if (possibledir.Count != 0)
                {
                    activeFire.Push(loc);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void fireExtinguished(Vector2 loc)
        {
            loc += offset;

            fireLoc[(int)loc.x, (int)loc.y] = false;
            if (activeFire.Contains(loc))
            {

                Stack<Vector2> copy = new Stack<Vector2>();
                while (activeFire.Count > 0)
                {
                    Vector2 temp = activeFire.Pop();
                    if (temp != loc)
                    {
                        copy.Push(temp);
                    }
                }

                activeFire = copy;

            }

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
