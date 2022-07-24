using SwordsInSpace;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace SwordsInSpace
{
    public class EnemyShooter : NetworkBehaviour
    {
        // Start is called before the first frame update

        [SerializeField]
        protected GameObject bullet;
        [SerializeField]
        public float range = 100f;

        public Timer bulletCd;

        public float shotSpeed;
        public double shotLifetime;
        public double damage;
        public double shotSpread = 0;
        public bool canFire = true;

        public Vector2 bulletScale = new Vector2(3f, 3f);

        public bool timerComplete = false;

        [SyncVar]
        public bool isAsleep = false;


        private bool IsInRange()
        {
            return range > Vector3.Distance(gameObject.transform.position, Ship.currentShip.gameObject.transform.position);
        }

        public void onTimerComplete()
        {
            timerComplete = true;
        }

        public void Update()
        {
            if (IsServer && canFire && timerComplete && !isAsleep && IsInRange())
            {
                Shoot();
                timerComplete = false;
                bulletCd.Start();
            }
        }

        public virtual void Shoot()
        {
            ShootAtPlayer();
        }

        protected void ShootAt(Transform startLoc, Transform endLoc, Bullet.BulletBehavior fn = null, Quaternion offset = default)
        {
            if (offset == default)
            {
                offset = Quaternion.Euler(0, 0, 0);
            }
            Vector3 myLocation = startLoc.position;
            Vector3 targetLocation = endLoc.position;
            targetLocation.z = myLocation.z;
            Vector3 vectorToTarget = targetLocation - myLocation;

            Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);
            Quaternion rotation = Quaternion.LookRotation(endLoc.position - startLoc.position, Vector3.forward);

            GameObject toAdd = Instantiate(bullet, startLoc.position, rotation);
            Bullet toSpawn = toAdd.GetComponent<Bullet>();

            toAdd.transform.rotation = Quaternion.RotateTowards(startLoc.rotation, targetRotation, 360f);


            toAdd.transform.rotation *= offset;
            toSpawn.Setup(shotSpeed, shotLifetime, damage, shotSpread, 1);
            toAdd.transform.localScale *= bulletScale;

            if (fn != null)
                toSpawn.AddMovementFunction(fn);
            Spawn(toAdd);
        }

        protected void ShootAtPlayer(Quaternion offset = default)
        {
            ShootAt(transform, Ship.currentShip.gameObject.transform, offset: offset);
        }



        protected void SpawnLocalRotation(Quaternion offset = default, Vector3 loc = default, Bullet.BulletBehavior fn = null, float customBulletSpeed = -1f)
        {
            if (offset == default)
            {
                offset = Quaternion.Euler(0, 0, 0);
            }

            if (loc == default)
            {
                loc = transform.position;
            }

            if (customBulletSpeed < 0)
            {
                customBulletSpeed = shotSpeed;
            }

            offset *= Quaternion.Euler(0, 0, 90);

            GameObject toAdd = Instantiate(bullet, loc, gameObject.transform.rotation);
            toAdd.transform.rotation = offset * transform.rotation;
            Bullet toSpawn = toAdd.GetComponent<Bullet>();
            toSpawn.Setup(customBulletSpeed, shotLifetime, damage, shotSpread, 1);
            toAdd.transform.localScale *= bulletScale;

            if (fn != null)
                toSpawn.AddMovementFunction(fn);

            Spawn(toAdd);
        }

    }
};