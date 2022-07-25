using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class FencerAI : EnemyAI, RageInterface
    {
        public bool debugEnrage = false;

        private void Update()
        {
            if (debugEnrage)
            {
                debugEnrage = false;
                StartRagePhase();
            }
        }
        public void StartRagePhase()
        {
            GetComponentInChildren<FencerShooter>().StartRagePhase();
            StartCoroutine(BulkUp());
        }

        public IEnumerator BulkUp()
        {
            int pumps = 3;
            for (int i = 0; i < pumps; i++)
            {
                gameObject.transform.localScale *= 1.25f;
                currentHp += 10;
                maxHp += 10;
                yield return new WaitForSeconds(0.8f);
            }
        }



    }
};