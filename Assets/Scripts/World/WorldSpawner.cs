using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;

namespace SwordsInSpace
{
    public class WorldSpawner : NetworkBehaviour
    {
        // Start is called before the first frame update

        public static int level = 0;

        public GameObject[] worlds;
        void Start()
        {
            LoadWorld();
        }

        public void LoadWorld()
        {
            GameObject toAdd = Instantiate(worlds[level < worlds.Length ? level : 0]);
            Spawn(toAdd);
        }
    }
};
