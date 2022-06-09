using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{
    public class Shooter : NetworkBehaviour
    {
        // Start is called before the first frame update
        Timer burstTimer;
        Timer atkTimer;

        bool canFire = true;

        bool autoFire = false;
        public Vector2 turnAxis;

        int currentBurst = 0;

        public WeaponSO data;
        public void Setup()
        {
            this.burstTimer = gameObject.AddComponent<Timer>();
            this.burstTimer.Setup(data.burstCD, false, false);
            this.burstTimer.timeout.AddListener(StartBurst);

            this.atkTimer = gameObject.AddComponent<Timer>();
            this.atkTimer.Setup(data.atkCD, false, false);
            this.atkTimer.timeout.AddListener(FinishReload);


        }

        [ServerRpc(RequireOwnership = false)]
        public void ToggleAutoFire()
        {
            autoFire = !autoFire;
        }

        public void FinishReload()
        {
            canFire = true;
            if (autoFire)
            {
                StartAttack();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void Fire()
        {
            if (canFire)
            {
                StartAttack();
            }
        }

        public void StartAttack()
        {
            if (!IsServer) { return; }//Sanity check
            canFire = false;
            atkTimer.Start();
            StartBurst();
        }

        private void StartBurst()
        {
            if (!IsServer) { return; }//Sanity check

            if (currentBurst < data.burst)
            {
                if (autoFire) {; }
                    //Left();
                SpawnBullet();
                currentBurst += 1;
                this.burstTimer.Start();

            }
            else
            {
                currentBurst = 0;
            }

        }


        private void SpawnBullet()
        {
            GameObject toAdd = Instantiate(data.bulletPrefab, transform.position, transform.rotation);
            toAdd.GetComponent<Bullet>().Setup(data.shotSpeed, data.shotLifeTime);
            Spawn(toAdd);
        }

        private void Update()
        {
            if (!IsServer) return; //only rotate on server
            transform.localRotation = Quaternion.Euler(0,0, transform.localRotation.eulerAngles.z - data.rotationSpeed * turnAxis.x * Time.fixedDeltaTime);
        }










    }
};
