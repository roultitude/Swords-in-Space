using System.Collections;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using static SwordsInSpace.UpgradeSO;

namespace SwordsInSpace
{

    public class Power : Module
    {
        // Start is called before the first frame update

        [SerializeField]
        private float maxAmount = 100;

        [SerializeField]
        private int drainRate = 5;

        [SerializeField]
        private int refillRate = 3;

        [SerializeField]
        AudioClip chargeSound;

        [SyncVar(OnChange = nameof(OnChangeCurrentAmount))]
        public float currentAmount = 100;

        public bool supplyingPower = true;

        [SerializeField]
        public Hpbar bar;

        [SyncVar(OnChange = nameof(OnChangeAnimLocked))]
        bool animLocked = false;

        IEnumerator animLocker;

        private new void Start()
        {
            base.Start();
            Ship.currentShip.upgradeManager.OnUpgrade += ReloadUpgrades;

            fetchBaseStats();
        }

        public void OnChangeCurrentAmount(float bef, float current, bool serv)
        {
            bar.Resize(current / maxAmount);
        }
        private void fetchBaseStats()
        {
            maxAmount = Ship.currentShip.data.powerMaxAmount;
            drainRate = Ship.currentShip.data.powerDrainRate;
            refillRate = Ship.currentShip.data.powerRefillRate;
            clampStats();
        }

        private void clampStats()
        {
            drainRate = (int)Mathf.Clamp(drainRate, 1, maxAmount / 2f);
            refillRate = Mathf.Clamp(refillRate, drainRate + 1, refillRate);
        }
        public void ReloadUpgrades(Dictionary<UpgradeTypes, float> stats)
        {


            //Base increases
            foreach (UpgradeTypes type in stats.Keys)
            {
                switch (type)
                {
                    case UpgradeTypes.powerMaxAmount:
                        maxAmount = Ship.currentShip.data.powerMaxAmount + stats[type];
                        break;

                    case UpgradeTypes.powerDrainRate:
                        drainRate = Ship.currentShip.data.powerDrainRate + (int)stats[type];

                        break;

                    case UpgradeTypes.powerRefillRate:
                        refillRate = Ship.currentShip.data.powerRefillRate + (int)stats[type];

                        break;
                }
            }
            clampStats();

        }


        // Update is called once per frame
        void Update()
        {
            if (!IsServer) return;

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


                Ship.currentShip.updateShipBackgroundColor(Mathf.Clamp(4 * currentAmount / maxAmount, 0f, 1f));



                supplyingPower = true;
            }

            //bar.Resize(currentAmount / maxAmount);
        }


        public override void OnInteract(GameObject obj)
        {
            FillPower();
        }
        [ServerRpc(RequireOwnership = false)]
        public void FillPower()
        {
            currentAmount += refillRate;
            AudioManager.instance.ObserversPlay(chargeSound);
            if (currentAmount > maxAmount)
                currentAmount = maxAmount;
            if (animLocker != null) StopCoroutine(animLocker);
            animLocker = PlayAnimOnInteract();
            StartCoroutine(animLocker);
        }

        protected override void switchAnim()
        {
            if (!animLocked)
            {
                base.switchAnim();
            }
            else anim.CrossFade("PlayerOccupied", 0, 0);
        }
        IEnumerator PlayAnimOnInteract()
        {
            animLocked = true;
            switchAnim();
            yield return new WaitForSeconds(2f);
            animLocked = false;
            switchAnim();
            animLocker = null;
        }

        private void OnChangeAnimLocked(bool oldAnimLocked, bool newAnimLocked, bool isServer)
        {
            switchAnim();
        }

    }
}
;