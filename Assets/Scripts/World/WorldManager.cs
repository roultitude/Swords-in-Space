using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;


namespace SwordsInSpace
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager currentWorld;

        public EnemySpawner spawner;

        public Texture backgroundTextureBack, backgroundTextureMid, backgroundTextureFront;

        public bool levelComplete = false;

        private bool levelFinished = false;

        public Vector2 WorldSize = new Vector2(1000, 1000);

        void Start()
        {
            if (currentWorld)
            {
                Debug.Log("World exists, replacing with new world.");
                Destroy(currentWorld);
            }
            currentWorld = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (!InstanceFinder.IsServer) return;
            if (levelComplete && !levelFinished)
            {
                WorldSpawner.level++;
                GameManager.instance.OnLevelComplete();
                levelFinished = true;
            }
        }

        public IEnumerator CheckIfComplete()
        {
            yield return 0;
            if (spawner.spawningComplete && !levelComplete && spawner.IsAllBossesKilled())
            {
                levelComplete = true;
            }
        }


    }
};