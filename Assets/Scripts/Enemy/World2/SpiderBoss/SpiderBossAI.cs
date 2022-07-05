using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class SpiderBossAI : EnemyAI
    {
        public SpiderBossShooter shooter;
        public Rigidbody2D rb;


        public bool hasLaidWeb = false;


        public void Update()
        {

            if (IsServer && !hasLaidWeb && ai.reachedDestination)
            {
                hasLaidWeb = true;
                FreezeBoss();
            }
        }

        [ObserversRpc]
        public void FreezeBoss()
        {
            shooter.targetEnabled = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            shooter.EnableTargets();
        }
    }
};
