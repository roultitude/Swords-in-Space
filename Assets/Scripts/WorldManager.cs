using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class WorldManager : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField]
        EnemySpawner spawner;

        private bool levelComplete = false;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CheckIfComplete()
        {
            if (spawner.spawningComplete && !levelComplete && FindObjectOfType(typeof(EnemyAI)) == null)
            {
                levelComplete = true;
            }
        }


    }
};