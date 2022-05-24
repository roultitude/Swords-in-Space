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


        int currentBurst = 0;

        [SyncVar(OnChange = nameof(UpdateRotation))]
        float rotation;



        public WeaponSO data;
        public override void OnStartServer()
        {
            base.OnStartServer();
            this.burstTimer = gameObject.AddComponent<Timer>();
            this.burstTimer.Setup(data.burstCD, false, false);
            this.burstTimer.timeout.AddListener(startBurst);

            this.atkTimer = gameObject.AddComponent<Timer>();
            this.atkTimer.Setup(data.atkCD, false, false);
            this.atkTimer.timeout.AddListener(FinishReload);


        }

        public void UpdateRotation(float oldVal, float newVal, bool isServer)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, newVal);
        }

        public void FinishReload()
        {
            canFire = true;
            //atkTimer.Start();
        }

        [ServerRpc(RequireOwnership = false)]
        public void Fire()
        {
            if (canFire)
            {
                canFire = false;
                atkTimer.Start();
                startBurst();
            }
        }

        private void startBurst()
        {
            if (currentBurst < data.burst)
            {
                spawnBullet();
                currentBurst += 1;
                this.burstTimer.Start();

            }
            else
            {
                currentBurst = 0;
            }

        }


        private void spawnBullet()
        {
            GameObject toAdd = Instantiate(data.bulletPrefab, transform.position, transform.rotation);
            toAdd.GetComponent<Bullet>().Setup(data.shotSpeed, data.shotLifeTime);
            Spawn(toAdd);
        }

        [ServerRpc(RequireOwnership = false)]
        public void Left()
        {
            rotation -= 10f;
        }

        [ServerRpc(RequireOwnership = false)]
        public void Right()
        {
            rotation += 10f;
        }







    }
};
