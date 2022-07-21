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

        public SpriteRenderer spriterenderer;
        public override void OnStartServer()
        {
            base.OnStartServer();
            upg = Ship.currentShip.upgradeManager.GetValidUpgrade();
            //Get a suitable upgrade.
            Setup(upg.upgradeSo.upgradeName);
        }

        [ObserversRpc(BufferLast = true, RunLocally = true, IncludeOwner = true)]
        public void Setup(string upgradeName)
        {
            Debug.Log(Ship.currentShip.upgradeManager.StringToUpgradeSO(upgradeName));
            spriterenderer.sprite = Ship.currentShip.upgradeManager.StringToUpgradeSO(upgradeName).sprite;
        }

        public override void OnPowerup()
        {
            if (!IsServer) return;
            Ship.currentShip.upgradeManager.AddUpgradeNoUI(upg.upgradeSo.upgradeName);

        }

    }
};