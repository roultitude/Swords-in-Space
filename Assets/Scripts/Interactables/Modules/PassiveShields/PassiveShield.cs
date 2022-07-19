using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SwordsInSpace.UpgradeSO;

namespace SwordsInSpace
{
    public class PassiveShield : Module
    {
        // Start is called before the first frame update
        [SerializeField]
        public GameObject UIDisplayPrefab;
        private DisplayManager manager;

        private PassiveShieldDisplay display;

        private GameObject UIDisplay;
        PlayerInputManager currentPlayerInput;

        public float baseHealAmount = 1f;
        public float baseComboHealAmount = 1f;

        public float currentHealAmount, currentComboHealAmount;

        private void OnEnable()
        {
            GameManager.OnNewSceneLoadEvent += SetupUI;
            currentHealAmount = baseHealAmount;
            currentComboHealAmount = baseComboHealAmount;
            Ship.currentShip.upgradeManager.OnUpgrade += ReloadUpgrades;
        }
        void OnDisable()
        {
            GameManager.OnNewSceneLoadEvent -= SetupUI;
        }

        void SetupUI()
        {
            UIDisplay = Instantiate(UIDisplayPrefab, Vector3.zero, Quaternion.identity);
            manager = DisplayManager.instance;
            UIDisplay.SetActive(false);
            display = UIDisplay.GetComponent<PassiveShieldDisplay>();


        }

        public void ReloadUpgrades(Dictionary<UpgradeTypes, float> stats)
        {

            if (stats.ContainsKey(UpgradeTypes.healOnBeat))
            {
                currentHealAmount = baseHealAmount + stats[UpgradeTypes.healOnBeat];
                currentHealAmount = Mathf.Clamp(currentHealAmount, baseHealAmount, float.MaxValue);
            }

            if (stats.ContainsKey(UpgradeTypes.healOnCombo))
            {
                currentComboHealAmount = baseComboHealAmount + stats[UpgradeTypes.healOnCombo];
                currentComboHealAmount = Mathf.Clamp(currentComboHealAmount, baseComboHealAmount, float.MaxValue);
            }


        }


        public override void OnInteract(GameObject player)
        {
            if (manager.Offer(UIDisplay, this))
            {
                SetOccupied(true);
                currentPlayerInput = player.GetComponent<PlayerInputManager>();


                currentPlayerInput.SwitchView("PassiveShieldView");

                currentPlayerInput.playerInput.actions["LeftDrumstick"].performed += PassiveShieldInputLeft;
                currentPlayerInput.playerInput.actions["RightDrumstick"].performed += PassiveShieldInputRight;

                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
                display.startGame();
            }
        }
        void OnDisplayClosed()
        {
            if (display.increment > 0)
                healFromCombo(display.increment);
            SetOccupied(false);
            currentPlayerInput.SwitchView("PlayerView");

            currentPlayerInput.playerInput.actions["LeftDrumstick"].performed -= PassiveShieldInputLeft;
            currentPlayerInput.playerInput.actions["RightDrumstick"].performed -= PassiveShieldInputRight;


            DisplayManager.instance.DisplayCloseEvent -= OnDisplayClosed;
            currentPlayerInput = null;

        }



        public void PassiveShieldInputLeft(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed)
            {
                display.Drum("L");
            }
        }
        public void PassiveShieldInputRight(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed)
            {
                display.Drum("R");
            }
        }

        [ServerRpc(RequireOwnerShip = false)]
        public void healFromBeat()
        {
            Ship.currentShip.AddHp(currentHealAmount);
        }

        [ServerRpc(RequireOwnerShip = false)]
        public void healFromCombo(float increment)
        {
            Ship.currentShip.AddHp(increment * currentComboHealAmount);
        }

    }
};