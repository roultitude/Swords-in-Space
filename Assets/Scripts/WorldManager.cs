using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;


namespace SwordsInSpace
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager currentWorld;
        
        // Start is called before the first frame update

        [SerializeField]
        EnemySpawner spawner;

        public bool levelComplete = false;

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
            if (levelComplete)
            {
                GameManager.instance.OnLevelComplete();
                levelComplete = false;
            }
        }

        public IEnumerator CheckIfComplete()
        {
            yield return 0;
            if (spawner.spawningComplete && !levelComplete && FindObjectOfType(typeof(EnemyAI)) == null)
            {
                levelComplete = true;
            }
        }


    }
};