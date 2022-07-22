using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class SpiderShooter : EnemyShooter
    {
        public float[] bulletSpeeds;
        public override void Shoot()
        {
            foreach (float bulletSpeed in bulletSpeeds)
            {
                shotSpeed = bulletSpeed;
                ShootAtPlayer();
            }

        }
    }
};