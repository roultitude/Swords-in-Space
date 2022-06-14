using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwordsInSpace
{
    public class UIUpgrades : NetworkBehaviour
    {
        // Start is called before the first frame update

        UpgradeSO[] upgrades;

        public UpgradeButton[] buttons;

        private UpgradeManager upgradeManager;


        void Start()
        {
            this.upgradeManager = Ship.currentShip.upgradeManager;


            upgrades = new UpgradeSO[3];

            buttons[0].gameObject.GetComponent<Button>().onClick.AddListener(OnClickLeftButton);
            buttons[1].gameObject.GetComponent<Button>().onClick.AddListener(OnClickMiddleButton);
            buttons[2].gameObject.GetComponent<Button>().onClick.AddListener(OnClickRightButton);

        }



        [ObserversRpc]
        public void SetUpgrades(string upgradestr1, string upgradestr2, string upgradestr3)
        {
            UpgradeSO upgrade1 = upgradeManager.StringToUpgradeSO(upgradestr1);
            UpgradeSO upgrade2 = upgradeManager.StringToUpgradeSO(upgradestr2);
            UpgradeSO upgrade3 = upgradeManager.StringToUpgradeSO(upgradestr3);


            upgrades[0] = upgrade1;
            upgrades[1] = upgrade2;
            upgrades[2] = upgrade3;

            buttons[0].UpdateButton(upgrade1);
            buttons[1].UpdateButton(upgrade2);
            buttons[2].UpdateButton(upgrade3);

        }




        [ServerRpc(RequireOwnership = false)]
        public void OnClickLeftButton()
        {
            upgradeManager.AddUpgrade(upgrades[0].name);
        }

        [ServerRpc(RequireOwnership = false)]
        public void OnClickMiddleButton()
        {
            upgradeManager.AddUpgrade(upgrades[1].name);
        }

        [ServerRpc(RequireOwnership = false)]
        public void OnClickRightButton()
        {
            upgradeManager.AddUpgrade(upgrades[2].name);
        }

    }
};