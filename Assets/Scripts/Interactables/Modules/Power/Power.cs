using System.Collections;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

namespace SwordsInSpace
{

    public class Power : Module
    {
        // Start is called before the first frame update

        [SerializeField]
        private int maxAmount = 100;

        [SerializeField]
        private int drainRate = 5;

        public int refillRate = 3;

        [SyncVar]
        public float currentAmount = 100;

        public bool supplyingPower = true;

        [SerializeField]
        public Hpbar bar;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (currentAmount == 0)
            {

                return;
            }
            currentAmount -= drainRate * Time.deltaTime;
            if (currentAmount < 0)
            {
                currentAmount = 0;
                supplyingPower = false;
                Ship.currentShip.PowerDown();
            }
            else
            {
                if (!supplyingPower)
                    Ship.currentShip.PowerUp();
                supplyingPower = true;
            }

            bar.Resize(currentAmount / maxAmount);
        }


        public override void Interact(GameObject obj)
        {
            FillPower();
        }
        [ServerRpc(RequireOwnership = false)]
        public void FillPower()
        {
            currentAmount += refillRate;
            if (currentAmount > maxAmount)
                currentAmount = maxAmount;
        }

    }
}
;