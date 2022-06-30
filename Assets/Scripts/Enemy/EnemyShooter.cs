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

        public double shotSpeed;
        public double shotLifetime;
        public double damage;
        public double shotSpread = 0;
        public bool canFire = true;

        public bool timerComplete = false;
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
            if (IsServer && canFire && timerComplete && IsInRange())
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

        protected void ShootAtPlayer()
        {
            Vector3 myLocation = transform.position;
            Vector3 targetLocation = Ship.currentShip.transform.position;
            targetLocation.z = myLocation.z;
            Vector3 vectorToTarget = targetLocation - myLocation;

            Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);





            Quaternion rotation = Quaternion.LookRotation(Ship.currentShip.gameObject.transform.position - gameObject.transform.position, Vector3.forward);


            GameObject toAdd = Instantiate(bullet, transform.position, rotation);

            toAdd.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f);

            toAdd.GetComponent<Bullet>().Setup(shotSpeed, shotLifetime, damage, shotSpread);
            Spawn(toAdd);
        }
        protected void SpawnLocalRotation(Quaternion rot)
        {

            SpawnLocalRotation(rot, transform.position);

        }

        protected void SpawnLocalRotation(Quaternion rot, Vector3 loc)
        {
            GameObject toAdd = Instantiate(bullet, loc, gameObject.transform.rotation);
            toAdd.transform.rotation = rot * transform.rotation;
            toAdd.GetComponent<Bullet>().Setup(shotSpeed, shotLifetime, damage, shotSpread);
            Spawn(toAdd);
        }
    }
};