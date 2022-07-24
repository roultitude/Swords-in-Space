using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SwordsInSpace
{
    public class CucumberShooter : EnemyShooter
    {
        public int sideBulletPairs = 1;
        public float shotOffset;
        public Animator anim;

        public override void Shoot()
        {
            anim.CrossFade("Attack1", 0);
            for (int i = 0; i < sideBulletPairs + 1; i++)
            {
                if (i == 0)
                {
                    ShootAtPlayer();
                }
                else
                {
                    ShootAtPlayer(offset: Quaternion.Euler(0, 0, shotOffset * i));
                    ShootAtPlayer(offset: Quaternion.Euler(0, 0, -shotOffset * i));
                }

            }

        }

    }

};