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
        public bool useDebugLevel = false;

        public GameObject[] worlds;
        public GameObject debugWorld;
        public override void OnStartServer()
        {
            base.OnStartServer();
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


            GameObject toAdd = Instantiate(worlds[GameManager.instance.currentLevel < worlds.Length ? GameManager.instance.currentLevel : 0]);
            Spawn(toAdd);
        }
    }
};
