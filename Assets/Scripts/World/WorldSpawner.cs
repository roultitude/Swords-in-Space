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
        public bool useDebugLevel = false;

        public GameObject[] worlds;
        public GameObject debugWorld;
        void Start()
        {
            if (!IsServer) return;
            LoadWorld();
        }

        public void LoadWorld()
        {
            if (useDebugLevel)
            {
                GameObject toAddDebug = Instantiate(debugWorld);
                Spawn(toAddDebug);
                return;
            }


            GameObject toAdd = Instantiate(worlds[level < worlds.Length ? level : 0]);
            Spawn(toAdd);
        }
    }
};
