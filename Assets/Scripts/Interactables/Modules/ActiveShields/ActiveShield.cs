using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class ActiveShield : Module
    {
        public GameObject UIDisplayPrefab;

        [SerializeField]
        private Shielder shielder;
        private Vector2 turnAxis;
        private GameObject UIDisplay;
        private DisplayManager manager;
        private PlayerInputManager currentPlayerInput;

        void OnEnable()
        {
            GameManager.OnNewSceneLoadEvent += SetupUI;
        }

        void OnDisable()
        {
            GameManager.OnNewSceneLoadEvent -= SetupUI;
            InstanceFinder.TimeManager.OnTick -= OnTick;
        }

        void SetupUI()
        {
            UIDisplay = Instantiate(UIDisplayPrefab, Vector3.zero, Quaternion.identity);
            manager = DisplayManager.instance;
            UIDisplay.SetActive(false);
        }

        void OnDisplayClosed()
        {
            SetOccupied(false);
            CameraManager.instance.ToggleWeaponCamera();
            currentPlayerInput.playerInput.actions["Move"].performed -= ActiveShieldInputMove;
            DisplayManager.instance.DisplayCloseEvent -= OnDisplayClosed;
            DisplayManager.instance.ToggleMobilePlayerDisplay(true);
            currentPlayerInput.SwitchView("PlayerView");
            currentPlayerInput = null;
            InstanceFinder.TimeManager.OnTick -= OnTick;
        }

        public override void OnInteract(GameObject player)
        {
            if (manager.Offer(UIDisplay))
            {
                SetOccupied(true);
                InstanceFinder.TimeManager.OnTick += OnTick;
                CameraManager.instance.ToggleWeaponCamera();
                currentPlayerInput = player.GetComponent<PlayerInputManager>();
                currentPlayerInput.SwitchView("ActiveShieldView");
                DisplayManager.instance.ToggleMobilePlayerDisplay(false);
                currentPlayerInput.playerInput.actions["Move"].performed += ActiveShieldInputMove;
                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
            }
        }

        private void ActiveShieldInputMove(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {

            if (obj.performed) turnAxis = obj.ReadValue<Vector2>();
            if (obj.canceled) turnAxis = Vector2.zero;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncTurnAxis(Vector2 turnAxis)
        {
            shielder.turnAxis = turnAxis;
        }


        public void OnTick()
        {
            SyncTurnAxis(turnAxis);
        }

    }
}