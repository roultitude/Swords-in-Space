using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class RengarAI : EnemyAI, RageInterface
    {
        public Collider2D enemyCollider;

        public bool debugEnrage = false;
        public float contactDamage;

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
            GetComponent<RengarMover>().StartRagePhase();
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Ship obj = collision.gameObject.GetComponent<Ship>();

            if (obj != null)
            {
                enemyCollider.isTrigger = true;
                Ship.currentShip.TakeDamage(contactDamage);
            }

        }
    }
};