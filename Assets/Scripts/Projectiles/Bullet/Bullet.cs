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
        double shotSpread;

        private bool hasSpread = false;

        public override void Setup(double shotSpeed, double shotLifeTime, double damage, double spread)
        {
            timer = gameObject.AddComponent<Timer>();
            this.shotSpeed = shotSpeed;
            timer.Setup(shotLifeTime, false, true);
            timer.timeout.AddListener(OnTimeout);
            this.damage = damage;
            this.shotSpread = spread;

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

            if (!hasSpread)
            {
                hasSpread = true;
                gameObject.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range((float)-shotSpread / 2, (float)shotSpread / 2));
            }
            transform.position += transform.right * Time.deltaTime * (float)shotSpeed;


        }


    }
};
