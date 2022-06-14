using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwordsInSpace
{
    public class UpgradeManager : NetworkBehaviour
    {

        [System.Serializable]
        public class Upgrade
        {
            public UpgradeSO upgradeSo;
            public int upgradeCount;
        }

        public readonly double tier1Chance = 0.5;
        public Upgrade[] tier1Upgrade;

        public readonly double tier2Chance = 0.25;
        public Upgrade[] tier2Upgrade;

        public readonly double tier3Chance = 0.2;
        public Upgrade[] tier3Upgrade;

        public readonly double tier4Chance = 0.05;
        public Upgrade[] tier4Upgrade;

        public Dictionary<string, Upgrade> upgrades;

        [SerializeField]
        public GameObject UIDisplay;
        private DisplayManager manager;

        private UIUpgrades uiUpgrades;

        private int numUpgrades;

        public bool debugTrigger = false;
        public void Update()
        {
            if (debugTrigger)
            {
                debugTrigger = false;
                TriggerUpgrades(2);
            }
        }

        public void Start()
        {
            numUpgrades = 0;

            upgrades = new Dictionary<string, Upgrade>();

            Upgrade[][] tiers = { tier1Upgrade, tier2Upgrade, tier3Upgrade, tier4Upgrade };
            foreach (Upgrade[] upgradeArr in tiers)
            {
                foreach (Upgrade a in upgradeArr)
                {
                    upgrades.Add(a.upgradeSo.name, a);
                }
            }

            manager = DisplayManager.instance;
            uiUpgrades = UIDisplay.GetComponent<UIUpgrades>();
        }



        public void TriggerUpgrades(int numUpgrades)
        {
            if (!IsServer)
                return;

            this.numUpgrades = numUpgrades;
            RollUpgrades();
            ShowUpgradeScreen();

        }

        private void RollUpgrades()
        {
            if (!IsServer)
                return;



            double randChance = Random.Range(0.0f, 1.0f);

            randChance = 0.1; //To Remove after implmentation of higher Upgrade tiers.

            Upgrade[] thisTier;
            if (randChance <= 0.5)
            {

                thisTier = tier1Upgrade;


            }
            else if (randChance <= 0.75)
            {
                thisTier = tier2Upgrade;
            }
            else if (randChance <= 0.95)
            {
                thisTier = tier3Upgrade;
            }
            else
            {
                thisTier = tier4Upgrade;
            }

            uiUpgrades.SetUpgrades(thisTier[Random.Range(0, thisTier.Length)].upgradeSo.name,
                    thisTier[Random.Range(0, thisTier.Length)].upgradeSo.name,
                    thisTier[Random.Range(0, thisTier.Length)].upgradeSo.name);



        }






        public void ShowUpgradeScreen()
        {
            if (manager.Offer(UIDisplay))
            {
                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
            }
        }

        [ObserversRpc]
        public void BroadcastUpgradeScreen()
        {
            if (manager.Offer(UIDisplay))
            {
                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
            }
        }


        [ServerRpc(RequireOwnership = false)]
        public void AddUpgrade(string upgrd)
        {
            upgrades[upgrd].upgradeCount += 1;
            numUpgrades -= 1;
            Debug.Log("Upgrade Complete!");
            if (numUpgrades > 0)
            {
                RollUpgrades();
            }
        }


        void OnDisplayClosed()
        {
            DisplayManager.instance.DisplayCloseEvent -= OnDisplayClosed;

        }


        public UpgradeSO StringToUpgradeSO(string input)
        {
            return upgrades[input].upgradeSo;
        }

    }
};
