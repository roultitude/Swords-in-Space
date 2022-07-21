using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Scened;
using FishNet.Connection;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace SwordsInSpace
{
    [ExecuteInEditMode]
    public class EditorFireSpawner : MonoBehaviour
    {
        public Tilemap floorTileMap;


        private bool[,] spots;

        public bool spawnFire = false;

#if UNITY_EDITOR
        public void Update()
        {
            if (spawnFire)
            {
                Activate();

                spawnFire = false;
            }

        }

        public void Activate()
        {
            floorTileMap.CompressBounds();
            var bounds = floorTileMap.cellBounds;
            spots = new bool[bounds.size.x, bounds.size.y];
            GameObject[,] fires = new GameObject[bounds.size.x, bounds.size.y];
            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    var px = bounds.xMin + x;
                    var py = bounds.yMin + y;
                    spots[x, y] = floorTileMap.HasTile(new Vector3Int(px, py, 0));
                    if (spots[x, y])
                    {
                        fires[x, y] = instantiateFire();
                    }

                }
            }

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    if (fires[x, y] == null)
                        continue;

                    fires[x, y].transform.localPosition = new Vector3(x, y, 0);
                    if (x > 0 && fires[x - 1, y] != null)
                    {
                        fires[x, y].GetComponent<Fire>().Left = fires[x - 1, y].GetComponent<Fire>();
                    }

                    if (y > 0 && fires[x, y - 1] != null)
                    {
                        fires[x, y].GetComponent<Fire>().Down = fires[x, y - 1].GetComponent<Fire>();
                    }
                    if (x < bounds.size.x - 2 && fires[x + 1, y] != null)
                    {
                        fires[x, y].GetComponent<Fire>().Right = fires[x + 1, y].GetComponent<Fire>();
                    }
                    if (y < bounds.size.y - 2 && fires[x, y + 1] != null)
                    {
                        fires[x, y].GetComponent<Fire>().Up = fires[x, y + 1].GetComponent<Fire>();
                    }

                }
            }


            Debug.Log("?");

        }

        private bool isValidFireLoc(Vector2 loc)
        {
            if (loc.x >= 0 && loc.y >= 0 && loc.x < spots.GetLength(0) && loc.y < spots.GetLength(1))
                return spots[(int)loc.x, (int)loc.y];
            return false; //Out of range
        }

        public GameObject instantiateFire()
        {
            return (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Fire/Fire.prefab", typeof(GameObject)));
        }

#endif
    }

};
