using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

namespace SwordsInSpace
{
    public class Shooter : NetworkBehaviour
    {
        // Start is called before the first frame update
        Timer burstTimer;
        Timer atkTimer;

        bool canFire = true;


        int currentBurst = 0;



        public WeaponSO data;
        public override void OnStartServer()
        {
            base.OnStartServer();
            this.burstTimer = gameObject.AddComponent<Timer>();
            this.burstTimer.Setup(data.burstCD, false, false);

            this.atkTimer = gameObject.AddComponent<Timer>();
            this.atkTimer.Setup(data.atkCD, false, false);

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
                Debug.Log("I AM SHOOTING");
                GameObject toAdd = Instantiate(data.bulletPrefab, transform.position, transform.rotation);
                //toAdd.GetComponent<Bullet>().Move(new Vector2(1, 1));
                Spawn(toAdd);
            }
        }



    }
};
