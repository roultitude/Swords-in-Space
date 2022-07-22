using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class FiveFishesShooter : EnemyShooter
    {
        int phase = 1;
        private double initialShotSpeed;

        private void Awake()
        {
            initialShotSpeed = shotSpeed;
        }

        public override void Shoot()
        {
            if(phase < 4)
            {
                float offset = Random.Range(0, 72);
                SpawnLocalRotation(Quaternion.Euler(0, 0, offset));
                SpawnLocalRotation(Quaternion.Euler(0, 0, offset + 72));
                SpawnLocalRotation(Quaternion.Euler(0, 0, offset + 72 * 2));
                SpawnLocalRotation(Quaternion.Euler(0, 0, offset + 72 * 3));
                SpawnLocalRotation(Quaternion.Euler(0, 0, offset + 72 * 4));
                phase++;

            } else if(phase == 4)
            {
                StartCoroutine(BurstShot());
                phase = 1;
            }
            
        }

        public IEnumerator BurstShot()
        {
            ShootAtPlayer();
            yield return new WaitForSeconds(0.1f);
            ShootAtPlayer();
            yield return new WaitForSeconds(0.1f);
            ShootAtPlayer();
        }
    }
}

