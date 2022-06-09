using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FishNet;
using FishNet.Object;

namespace SwordsInSpace
{
    public abstract class Weapon : Module
    {
        // Start is called before the first frame update
        [SerializeField]
        public WeaponSO data;

        public int currentAmmo;
        [SerializeField]
        public GameObject UIDisplay;
        [SerializeField]
        public GameObject[] shootersObj;

        protected List<Shooter> shooters;
        PlayerInputManager currentPlayerInput;
        private DisplayManager manager;
        private Vector2 turnAxis;
        private bool togglingAutoFire;
        private bool firing;
        void Awake()
        {
            shooters = new List<Shooter>();
            foreach (GameObject comp in shootersObj)
            {
                Shooter compShooter = comp.GetComponentInChildren<Shooter>();
                shooters.Add(compShooter);
                compShooter.data = data;
                compShooter.Setup();
            }
        }

        void Start()
        {
            UIDisplay = Instantiate(UIDisplay, Vector3.zero, Quaternion.identity);
            manager = DisplayManager.instance;
            UIDisplay.SetActive(false);

        }

        private void OnEnable()
        {
            //InstanceFinder.TimeManager.OnTick += OnTick;
        }
        private void OnDisable()
        {
            InstanceFinder.TimeManager.OnTick -= OnTick;
        }
        void OnDisplayClosed()
        {
            CameraManager.instance.ToggleWeaponCamera();
            currentPlayerInput.playerInput.actions["Move"].performed -= WeaponInputMove;
            currentPlayerInput.playerInput.actions["Fire"].performed -= WeaponInputFire;
            currentPlayerInput.playerInput.actions["ToggleAutoFire"].performed -= WeaponInputAutoFire;
            DisplayManager.instance.DisplayCloseEvent -= OnDisplayClosed;
            DisplayManager.instance.toggleMobilePlayerDisplay(true);
            currentPlayerInput.SwitchView("PlayerView");
            currentPlayerInput = null;
            InstanceFinder.TimeManager.OnTick -= OnTick;
        }

        public override void Interact(GameObject player)
        {
            if (manager.Offer(UIDisplay))
            {
                InstanceFinder.TimeManager.OnTick += OnTick;
                CameraManager.instance.ToggleWeaponCamera();
                currentPlayerInput = player.GetComponent<PlayerInputManager>();
                currentPlayerInput.SwitchView("WeaponView");
                DisplayManager.instance.toggleMobilePlayerDisplay(false);
                currentPlayerInput.playerInput.actions["Move"].performed += WeaponInputMove;
                currentPlayerInput.playerInput.actions["Fire"].performed += WeaponInputFire;
                currentPlayerInput.playerInput.actions["ToggleAutoFire"].performed += WeaponInputAutoFire;
                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
                player.GetComponent<PlayerMover>().canMove = false;
            }
        }

        private void WeaponInputMove(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {

            if (obj.performed) turnAxis = obj.ReadValue<Vector2>();
            if (obj.canceled) turnAxis = Vector2.zero;
        }

        private void WeaponInputFire(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed) firing = true;
        }

        private void WeaponInputAutoFire(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed) togglingAutoFire = true;
        }

        public abstract void Shoot();

        [ServerRpc(RequireOwnership = false)]
        private void SyncTurnAxis(Vector2 turnAxis)
        {
            foreach (Shooter comp in shooters)
            {
                comp.turnAxis = turnAxis;
            }
        }


        public void OnTick()
        {
            SyncTurnAxis(turnAxis);
            if (firing) 
            {
                foreach (Shooter comp in shooters)
                {
                    comp.Fire();
                }
                firing = false;
            }
                
            if (togglingAutoFire)
            {
                foreach (Shooter comp in shooters)
                {
                    comp.ToggleAutoFire();
                    comp.Fire();
                }
                togglingAutoFire = false;
            }
        }

    }
};