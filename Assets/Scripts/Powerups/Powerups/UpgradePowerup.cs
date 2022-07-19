using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SwordsInSpace.UpgradeManager;

namespace SwordsInSpace
{
    public class UpgradePowerup : Powerup
    {

        Upgrade upg;

        SpriteRenderer spriterenderer;
        public void Start()
        {
            if (!IsServer) return;
            upg = Ship.currentShip.upgradeManager.GetValidUpgrade();
            //Get a suitable upgrade.
            Setup(upg.upgradeSo.upgradeName);
        }

        [ObserversRpc(BufferLast = true, RunLocally = true)]
        public void Setup(string upgradeName)
        {
            spriterenderer.sprite = Ship.currentShip.upgradeManager.StringToUpgradeSO(upgradeName).sprite;
        }

        public override void OnPowerup()
        {
            if (!IsServer) return;
            Ship.currentShip.upgradeManager.AddUpgradeNoUI(upg.upgradeSo.upgradeName);

        }

    }
};