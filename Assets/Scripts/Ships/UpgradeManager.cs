using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SwordsInSpace.UpgradeSO;
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
        public GameObject upgradesDisplay;

        private UpgradesDisplay uiUpgrades => upgradesDisplay.GetComponent<UpgradesDisplay>();

        [SyncVar]
        public int numUpgrades;

        public bool debugTrigger = false;

        private string[] upgradeChoice;

        private bool hasMadeChoice = false;

        private int votesMade = 0;
        private Dictionary<string, int> votes;

        PlayerInputManager currentPlayerInput;

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
                    if (a != null && !upgrades.ContainsKey(a.upgradeSo.name))
                        upgrades.Add(a.upgradeSo.name, a);
                }
            }

        }



        public void TriggerUpgrades(int numUpgrades)
        {
            if (!IsServer)
                return;

            this.numUpgrades = numUpgrades;
            RollUpgrades();
            BroadcastUpgradeScreen();

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

            ShowUpgrades(thisTier[Random.Range(0, thisTier.Length)].upgradeSo.name,
        thisTier[Random.Range(0, thisTier.Length)].upgradeSo.name,
        thisTier[Random.Range(0, thisTier.Length)].upgradeSo.name);



        }

        [ObserversRpc(RunLocally = true)]
        public void ShowUpgrades(string upgrade1, string upgrade2, string upgrade3)
        {
            hasMadeChoice = false;
            upgradeChoice = new string[] { upgrade1, upgrade2, upgrade3 };
            Debug.Log(upgrade1 + "\t" + upgrade2 + "\t" + upgrade3 + "\t" + uiUpgrades == null);
            uiUpgrades.SetUpgrades(upgrade1, upgrade2, upgrade3);

            upgradesDisplay.GetComponentInChildren<VoteTimer>().ResetTimer();

            if (IsServer)
            {
                votesMade = 0;
                votes = new Dictionary<string, int>();
                votes.Add(upgrade1, 0);
                if (!votes.ContainsKey(upgrade2))
                    votes.Add(upgrade2, 0);
                if (!votes.ContainsKey(upgrade3))
                    votes.Add(upgrade3, 0);
            }
        }

        public void ShowUpgradeScreen()
        {
            if (DisplayManager.instance.Offer(upgradesDisplay))
            {

                Player player = User.localUser.controlledPlayer;
                currentPlayerInput = player.GetComponent<PlayerInputManager>();
                currentPlayerInput.SwitchView("UpgradeView");
                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
                currentPlayerInput.playerInput.actions["Left"].performed += OnClickLeftButton;
                currentPlayerInput.playerInput.actions["Middle"].performed += OnClickMiddleButton;
                currentPlayerInput.playerInput.actions["Right"].performed += OnClickRightButton;
                player.GetComponent<PlayerMover>().canMove = false;

            }
        }

        void OnDisplayClosed()
        {
            DisplayManager.instance.DisplayCloseEvent -= OnDisplayClosed;
            currentPlayerInput.playerInput.actions["Left"].performed -= OnClickLeftButton;
            currentPlayerInput.playerInput.actions["Middle"].performed -= OnClickMiddleButton;
            currentPlayerInput.playerInput.actions["Right"].performed -= OnClickRightButton;
            User.localUser.controlledPlayer.GetComponent<PlayerMover>().canMove = true;
            currentPlayerInput.SwitchView("PlayerView");

        }

        [ObserversRpc]
        public void BroadcastUpgradeScreen()
        {
            ShowUpgradeScreen();
        }


        public void AddUpgrade(string upgrd)
        {
            upgrades[upgrd].upgradeCount += 1;
            numUpgrades -= 1;
            Debug.Log("Upgrade Complete!");
            if (numUpgrades > 0)
            {

                RollUpgrades();
            }
            else
            {
                Ship.currentShip.ReloadStats();
                BroadcastCloseScreen();
                GameManager.instance.GoToLevel("GameScene", true, true);
            }
        }


        [ObserversRpc]
        public void BroadcastCloseScreen()
        {
            DisplayManager.instance.Close();
        }


        public Dictionary<UpgradeTypes, float> TallyUpgrades()
        {
            Dictionary<UpgradeTypes, float> stats = new Dictionary<UpgradeTypes, float>();
            foreach (Upgrade up in upgrades.Values)
            {
                if (up.upgradeCount == 0)
                    continue;

                foreach (UpgradeAttributes a in up.upgradeSo.statChanges)
                {
                    if (!stats.ContainsKey(a.type))
                    {
                        stats.Add(a.type, a.amount * up.upgradeCount);
                    }
                    else
                    {
                        stats[a.type] += a.amount * up.upgradeCount;
                    }


                }
            }

            return stats;
        }





        public UpgradeSO StringToUpgradeSO(string input)
        {
            return upgrades[input].upgradeSo;
        }


        public void OnClickLeftButton(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (hasMadeChoice)
                return;

            hasMadeChoice = true;
            SendRPCUpgrade(upgradeChoice[0], 0, User.localUser.username);

        }



        public void OnClickMiddleButton(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (hasMadeChoice)
                return;

            hasMadeChoice = true;
            SendRPCUpgrade(upgradeChoice[1], 1, User.localUser.username);

        }


        public void OnClickRightButton(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (hasMadeChoice)
                return;

            hasMadeChoice = true;
            SendRPCUpgrade(upgradeChoice[2], 2, User.localUser.username);



        }

        [ObserversRpc]
        public void AddVote(int num, string username)
        {
            uiUpgrades.AddVote(num, username);

        }

        [ServerRpc(RequireOwnership = false)]
        private void SendRPCUpgrade(string str, int num, string username)
        {

            votesMade++;
            votes[str] += 1;


            if (votesMade >= UserManager.instance.users.Count)
            {
                List<String> ties = new List<String>();
                int maxValue = -1;
                foreach (String key in votes.Keys) //n = 3
                {
                    if (votes[key] > maxValue)
                    {
                        ties.Clear();
                        ties.Add(key);
                    }
                    else if (votes[key] == maxValue)
                    {
                        ties.Add(key);
                    }
                }

                AddUpgrade(ties[Random.Range(0, ties.Count)]);
                return;

            }
            AddVote(num, username);
        }

        public void AddRandomUpgrade()
        {
            if (hasMadeChoice)
                return;
            hasMadeChoice = false;
            int randomupgradenum = Random.Range(0, upgradeChoice.Length);
            SendRPCUpgrade(upgradeChoice[randomupgradenum], randomupgradenum, User.localUser.username);
        }


    }
};
