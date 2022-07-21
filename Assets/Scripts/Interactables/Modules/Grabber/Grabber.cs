using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class Grabber : Module
    {

        [SerializeField]
        public GameObject UIDisplayPrefab;
        [SerializeField]
        public GameObject GrabberLauncher;
        [SerializeField]
        float rotSpeed, baseForwardSpeed, baseRetractSpeed, baseForwardTime;
        [SerializeField]
        GameObject grabberHookPrefab;

        private GrabberHook currentGrabberHook;
        private GrabberLauncher grabber;
        private Vector2 turnAxis;
        private bool launching;
        private GameObject UIDisplay;
        private DisplayManager manager;



        PlayerInputManager currentPlayerInput;

        void OnEnable()
        {
            grabber = GrabberLauncher.GetComponent<GrabberLauncher>();
            grabber.UpdateRotSpeed(rotSpeed);

            GameManager.OnNewSceneLoadEvent += SetupUI;
            //Ship.currentShip.upgradeManager.OnUpgrade += ReloadUpgrades;

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
            currentPlayerInput.playerInput.actions["Move"].performed -= GrabberInputMove;
            currentPlayerInput.playerInput.actions["Fire"].performed -= GrabberInputLaunch;
            DisplayManager.instance.DisplayCloseEvent -= OnDisplayClosed;
            DisplayManager.instance.ToggleMobilePlayerDisplay(true);
            currentPlayerInput.SwitchView("PlayerView");
            currentPlayerInput = null;
            InstanceFinder.TimeManager.OnTick -= OnTick;
            SyncTurnAxis(Vector2.zero);
        }

        public override void OnInteract(GameObject player)
        {
            if (manager.Offer(UIDisplay, this))
            {
                SetOccupied(true);
                InstanceFinder.TimeManager.OnTick += OnTick;
                CameraManager.instance.ToggleWeaponCamera();
                currentPlayerInput = player.GetComponent<PlayerInputManager>();
                currentPlayerInput.SwitchView("WeaponView");
                DisplayManager.instance.ToggleMobilePlayerDisplay(false);
                currentPlayerInput.playerInput.actions["Move"].performed += GrabberInputMove;
                currentPlayerInput.playerInput.actions["Fire"].performed += GrabberInputLaunch;
                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
            }
        }

        private void GrabberInputMove(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {

            if (obj.performed) turnAxis = obj.ReadValue<Vector2>();
            if (obj.canceled) turnAxis = Vector2.zero;
        }

        private void GrabberInputLaunch(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed) launching = true;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncTurnAxis(Vector2 turnAxis)
        {
            grabber.turnAxis = turnAxis;
        }

        public void OnTick()
        {

            SyncTurnAxis(turnAxis);
            if (launching)
            {
                
                Launch();
                launching = false;
            }

        }

        [ServerRpc(RequireOwnership = false)]
        public void Launch()
        {
            if (currentGrabberHook) return; //dont launch if already out
            GameObject toAdd = Instantiate(grabberHookPrefab, grabber.transform.position, grabber.transform.rotation);
            GrabberHook gh = toAdd.GetComponent<GrabberHook>();
            gh.Setup(baseForwardSpeed, baseRetractSpeed, baseForwardTime,grabber.transform);
            currentGrabberHook = gh;
            Spawn(toAdd);
            
        }
    }
};