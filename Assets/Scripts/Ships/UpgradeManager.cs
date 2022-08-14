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
            public int upgradeCount = 0;
            public int upgradeMaxCount = -1;
        }

        public readonly double tier1Chance = 0.5;
        public List<Upgrade> tier1Upgrade;
        public List<Upgrade> tier1CombatUpgrade;

        public readonly double tier2Chance = 0.25;
        public List<Upgrade> tier2Upgrade;
        public List<Upgrade> tier2CombatUpgrade;

        public readonly double tier3Chance = 0.2;
        public List<Upgrade> tier3Upgrade;
        public List<Upgrade> tier3CombatUpgrade;

        public readonly double tier4Chance = 0.05;
        public List<Upgrade> tier4Upgrade;
        public List<Upgrade> tier4CombatUpgrade;

        public Dictionary<string, Upgrade> upgrades;


        [SerializeField]
        public GameObject upgradesDisplay;

        private UpgradesDisplay uiUpgrades => upgradesDisplay.GetComponent<UpgradesDisplay>();


        public int numUpgrades;

        public bool debugTrigger = false;

        private string[] upgradeChoice;

        private bool hasMadeChoice = false;

        private int votesMade = 0;
        private Dictionary<string, int> votes;

        PlayerInputManager currentPlayerInput;

        public delegate void OnUpgradeEvent(Dictionary<UpgradeTypes, float> stats);

        public OnUpgradeEvent OnUpgrade;

        private bool givenCombatUpgrade = false;


        public void Update()
        {
            if (debugTrigger)
            {
                debugTrigger = false;
                TriggerUpgrades();
            }
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            numUpgrades = 0;

            upgrades = new Dictionary<string, Upgrade>();

            List<Upgrade>[] tiers = { tier1Upgrade, tier2Upgrade, tier3Upgrade, tier4Upgrade, tier1CombatUpgrade, tier2CombatUpgrade, tier3CombatUpgrade, tier4CombatUpgrade };
            foreach (List<Upgrade> upgradeArr in tiers)
            {
                foreach (Upgrade a in upgradeArr)
                {
                    if (a != null && !upgrades.ContainsKey(a.upgradeSo.upgradeName))
                        upgrades.Add(a.upgradeSo.upgradeName, a);
                }
            }

            OnUpgrade?.Invoke(TallyUpgrades());

        }



        public void TriggerUpgrades()
        {
            if (!IsServer)
                return;
            givenCombatUpgrade = false;
            this.numUpgrades = 3;

            UpdateNumUpgradesClient(numUpgrades);
            RollUpgrades();
            BroadcastUpgradeScreen();

        }

        public Upgrade GetValidUpgrade()
        {

            double randChance = Random.Range(0.0f, 1.0f);



            List<Upgrade> thisTier;
            if (randChance <= 0.75)
            {

                thisTier = tier1Upgrade;

            }
            else if (randChance <= 0.9)
            {
                thisTier = tier2Upgrade;
            }
            else
            {
                thisTier = tier3Upgrade;
            }


            return thisTier[Random.Range(0, thisTier.Count)];

        }


        private void RollUpgrades()
        {
            if (!IsServer)
                return;



            double randChance = Random.Range(0.0f, 1.0f);

            int tier;
            List<Upgrade> thisTier;
            if (randChance <= 0.5)
            {

                thisTier = givenCombatUpgrade ? tier1Upgrade : tier1CombatUpgrade;
                tier = 1;

            }
            else if (randChance <= 0.8)
            {
                thisTier = givenCombatUpgrade ? tier2Upgrade : tier2CombatUpgrade;
                tier = 2;
            }
            else if (randChance <= 0.95)
            {
                thisTier = givenCombatUpgrade ? tier3Upgrade : tier3CombatUpgrade;
                tier = 3;
            }
            else
            {
                thisTier = givenCombatUpgrade ? tier4Upgrade : tier4CombatUpgrade;
                tier = 4;

                if (thisTier.Count > 0)
                {
                    //If the team does not want the tier 4 upgrade, switch to a tier 3 one. Last upgrade is tier 3.


                    ShowUpgrades(thisTier[Random.Range(0, thisTier.Count)].upgradeSo.upgradeName,
                  thisTier[Random.Range(0, thisTier.Count)].upgradeSo.upgradeName,
                  tier3Upgrade[Random.Range(0, tier3Upgrade.Count)].upgradeSo.upgradeName, tier);

                    return;

                }
                else
                {

                    thisTier = givenCombatUpgrade ? tier3Upgrade : tier3CombatUpgrade;
                    tier = 3;
                }

            }
            if (!givenCombatUpgrade)
                givenCombatUpgrade = true;

            ShowUpgrades(thisTier[Random.Range(0, thisTier.Count)].upgradeSo.upgradeName,
    thisTier[Random.Range(0, thisTier.Count)].upgradeSo.upgradeName,
    thisTier[Random.Range(0, thisTier.Count)].upgradeSo.upgradeName, tier);




        }

        [ObserversRpc(RunLocally = true)]
        public void ShowUpgrades(string upgrade1, string upgrade2, string upgrade3, int tier)
        {
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

            if (IsServerOnly) return;

            hasMadeChoice = false;
            upgradeChoice = new string[] { upgrade1, upgrade2, upgrade3 };

            uiUpgrades.SetUpgrades(upgrade1, upgrade2, upgrade3);

            upgradesDisplay.GetComponentInChildren<VoteTimer>().ResetTimer();
            uiUpgrades.upgradepanel.UpdateColor(tier);

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
            if (numUpgrades > 0)
            {
                DisplayManager.instance.Offer(upgradesDisplay);
                return;
            }
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

        public void RemoveUpgradeFromPool(Upgrade upg)
        {

            List<Upgrade>[] tiers = { tier1Upgrade, tier2Upgrade, tier3Upgrade, tier4Upgrade };
            foreach (List<Upgrade> upgradeArr in tiers)
            {
                for (int i = 0; i < upgradeArr.Count; i++)
                {
                    if (upgradeArr[i] == upg)
                    {
                        upgradeArr.RemoveAt(i);
                        return;
                    }
                }
            }
        }
        [ObserversRpc]
        public void UpdateNumUpgradesClient(int newVal)
        {
            if (IsServer) return;
            numUpgrades = newVal;
        }

        public void AddUpgrade(string upgrd)
        {

            upgrades[upgrd].upgradeCount += 1;
            numUpgrades -= 1;
            UpdateNumUpgradesClient(numUpgrades);
            Debug.Log("Upgrade Complete!" + upgrd);

            if (upgrades[upgrd].upgradeMaxCount != -1 && upgrades[upgrd].upgradeCount >= upgrades[upgrd].upgradeMaxCount)
            {
                Debug.Log("Maxxed out" + upgrd);
                RemoveUpgradeFromPool(upgrades[upgrd]);
            }

            if (numUpgrades > 0)
            {
                RollUpgrades();
            }
            else
            {
                OnUpgrade?.Invoke(TallyUpgrades());
                BroadcastCloseScreen();
                StartCoroutine(DelayedSwitchScene("GameScene", 1f));

            }
        }

        public void AddUpgradeNoUI(string upgrd)
        {
            upgrades[upgrd].upgradeCount += 1;

            if (upgrades[upgrd].upgradeMaxCount != -1 && upgrades[upgrd].upgradeCount >= upgrades[upgrd].upgradeMaxCount)
            {
                Debug.Log("Maxxed out" + upgrd);
                RemoveUpgradeFromPool(upgrades[upgrd]);
            }
            OnUpgrade?.Invoke(TallyUpgrades());
        }

        IEnumerator DelayedSwitchScene(string sceneName, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            GameManager.instance.GoToLevel(sceneName, true, true);
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
                    Debug.Log(key + "\t" + votes[key]);
                    if (votes[key] > maxValue)
                    {
                        maxValue = votes[key];
                        ties.Clear();
                        ties.Add(key);
                    }
                    else if (votes[key] == maxValue)
                    {
                        ties.Add(key);
                    }
                }

                foreach (string s in ties)
                {
                    Debug.Log(s);
                }

                string upgrd = ties[Random.Range(0, ties.Count)];

                AddUpgrade(upgrd);
                return;

            }
            AddVote(num, username);
        }

        public void AddRandomVote()
        {
            if (IsServerOnly) return;
            if (hasMadeChoice)
                return;
            hasMadeChoice = true;
            int randomupgradenum = Random.Range(0, upgradeChoice.Length);
            SendRPCUpgrade(upgradeChoice[randomupgradenum], randomupgradenum, User.localUser.username);
        }


    }
};
