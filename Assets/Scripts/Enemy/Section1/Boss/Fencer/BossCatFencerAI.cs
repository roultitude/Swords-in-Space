using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class BossCatFencerAI : EnemyAI, RageInterface
    {
        public bool debugEnrage = false;
        SpriteRenderer sprite;

        private void Awake()
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
        private void Update()
        {
            sprite.flipY = transform.rotation.eulerAngles.z < 180;
            if (debugEnrage)
            {
                debugEnrage = false;
                StartRagePhase();
            }
        }
        public void StartRagePhase()
        {
            GetComponentInChildren<BossCatFencerShooter>().StartRagePhase();
            StartCoroutine(BulkUp());
            StartCoroutine(RageStateRepositioning());
        }

        public IEnumerator RageStateRepositioning()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);
                StartCoroutine(BossTeleport());
            }
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