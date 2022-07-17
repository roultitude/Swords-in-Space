using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using System;

namespace SwordsInSpace
{
    public class Bullet : Projectile
    {
        public Timer timer;
        public double shotSpeed;
        double shotSpread;
        public delegate void BulletBehavior(GameObject bullet);
        BulletBehavior moveFunction;
        BulletBehavior onHitFunction;
        BulletBehavior onTimeoutFunction;

        private bool hasSpread = false;
        public int pierce = 1;

        public override void Setup(double shotSpeed, double shotLifeTime, double damage, double spread, int pierce)
        {
            timer = gameObject.AddComponent<Timer>();
            this.shotSpeed = shotSpeed;
            timer.Setup(shotLifeTime, false, true);
            timer.timeout.AddListener(OnTimeout);
            this.damage = damage;
            this.shotSpread = spread;
            this.pierce = pierce;
            moveFunction += BaseMovementFunction;
        }

        public void AddMovementFunction(BulletBehavior fn)
        {
            moveFunction += fn;
        }

        public void DebugMovementFunction(GameObject bullet)
        {
            bullet.transform.rotation *= Quaternion.Euler(0, 0, 5 * Time.deltaTime);
        }

        public void BaseMovementFunction(GameObject bullet)
        {
            bullet.transform.position += bullet.transform.right * Time.deltaTime * (float)shotSpeed;
        }




        public void OnHit()
        {


            if (!IsServer) return;
            pierce -= 1;

            onHitFunction?.Invoke(gameObject);
            if (pierce <= 0)
                OnTimeout();
        }

        public void OnTimeout()
        {
            onTimeoutFunction?.Invoke(gameObject);
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
            moveFunction?.Invoke(gameObject);


        }

        internal void AddOnHitFunction(BulletBehavior callOnHit)
        {
            onHitFunction += callOnHit;
        }

        internal void AddTimeoutFunction(BulletBehavior callOnTimeout)
        {
            onTimeoutFunction += callOnTimeout;
        }
    }
};
