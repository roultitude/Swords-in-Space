using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class SpiderShooter : EnemyShooter
    {
        public double[] bulletSpeeds;
        public override void Shoot()
        {
            foreach (double bulletSpeed in bulletSpeeds)
            {
                shotSpeed = bulletSpeed;
                ShootAtPlayer();
            }

        }
    }
};