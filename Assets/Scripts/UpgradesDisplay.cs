using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SwordsInSpace
{
    public class UpgradesDisplay : Display
    {
        // Start is called before the first frame update



        public UpgradeButton[] buttons;

        [SerializeField]
        private TMPro.TextMeshProUGUI upgradesRemainingText;

        private UpgradeManager upgradeManager;
        private UpgradeSO[] upgrades;
        private Canvas canvas;
        public override void Awake()
        {
            base.Awake();
            GameManager.OnNewSceneLoadEvent += OnNewSceneLoaded;
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
        }

        private void OnDisable()
        {
            GameManager.OnNewSceneLoadEvent -= OnNewSceneLoaded;
        }

        public void SetUpgrades(string upgradestr1, string upgradestr2, string upgradestr3)
        {
            GetComponent<Canvas>().enabled = true;
            if (upgradeManager == null || upgrades == null)
            {
                this.upgradeManager = Ship.currentShip.upgradeManager;
                upgrades = new UpgradeSO[3];
            }

            UpgradeSO upgrade1 = upgradeManager.StringToUpgradeSO(upgradestr1);
            UpgradeSO upgrade2 = upgradeManager.StringToUpgradeSO(upgradestr2);
            UpgradeSO upgrade3 = upgradeManager.StringToUpgradeSO(upgradestr3);


            upgrades[0] = upgrade1;
            upgrades[1] = upgrade2;
            upgrades[2] = upgrade3;

            buttons[0].UpdateButton(upgrade1);
            buttons[1].UpdateButton(upgrade2);
            buttons[2].UpdateButton(upgrade3);

            buttons[0].ClearVote();
            buttons[1].ClearVote();
            buttons[2].ClearVote();


        }

        public void AddVote(int buttonnum, string username)
        {
            buttons[buttonnum].AddVote(username);
        }
        private void Update()
        {
            if (canvas.enabled)
            {
                upgradesRemainingText.text = "Upgrades Left: " + upgradeManager.numUpgrades;
            }
        }
        private void OnNewSceneLoaded()
        {
            Ship.currentShip.upgradeManager.upgradesDisplay = gameObject;
        }

        public void triggerDebug()
        {
            Ship.currentShip.upgradeManager.debugTrigger = true;
        }




    }
};