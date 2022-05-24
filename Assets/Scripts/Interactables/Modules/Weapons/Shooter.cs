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

            this.atkTimer = gameObject.AddComponent<Timer>();
            this.atkTimer.Setup(data.atkCD, false, false);
        }

        public void UpdateRotation(float oldVal, float newVal, bool isServer)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, newVal);
        }

        public void FireEvent()
        {
            canFire = true;
            //atkTimer.Start();
        }

        [ServerRpc(RequireOwnership = false)]
        public void Fire()
        {
            if (canFire)
            {
                GameObject toAdd = Instantiate(data.bulletPrefab, transform.position, transform.rotation);
                //toAdd.GetComponent<Bullet>().Move(new Vector2(1, 1));
                Spawn(toAdd);
                rotation += 10f;
            }
        }



    }
};
