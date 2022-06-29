using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
namespace SwordsInSpace
{
    public class Bullet : Projectile
    {
        Timer timer;
        double shotSpeed;


        public override void Setup(double shotSpeed, double shotLifeTime, double damage, double spread)
        {
            timer = gameObject.AddComponent<Timer>();
            this.shotSpeed = shotSpeed;
            timer.Setup(shotLifeTime, false, true);
            timer.timeout.AddListener(OnTimeout);
            this.damage = damage;

        }

        public void OnHit()
        {
            if (IsServer)
                Despawn();
        }

        public void OnTimeout()
        {
            if (IsServer)
                Despawn();
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsServer) { return; }
            transform.position += transform.right * Time.deltaTime * (float)shotSpeed;

        }


    }
};
