using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{
    public abstract class Weapon : Module
    {
        // Start is called before the first frame update
        [SerializeField]
        public WeaponSO data;

        public int currentAmmo;
        [SerializeField]
        public GameObject UIDisplayPrefab;
        [SerializeField]
        public GameObject[] shootersObj;

        protected List<Shooter> shooters;
        PlayerInputManager currentPlayerInput;
        private DisplayManager manager;
        private Vector2 turnAxis;
        private bool togglingAutoFire;
        private bool firing;
        private GameObject UIDisplay;
        Timer burstTimer;
        Timer atkTimer;

        [SyncVar]
        public bool canFire = true;
        bool autoFire = false;

        int currentBurst = 0;

        public float percentageReloaded;

        void OnEnable()
        {
            shooters = new List<Shooter>();
            foreach (GameObject comp in shootersObj)
            {
                Shooter compShooter = comp.GetComponentInChildren<Shooter>();
                shooters.Add(compShooter);
                compShooter.Setup(data);
            }
            this.burstTimer = gameObject.AddComponent<Timer>();
            this.burstTimer.Setup(data.burstCD, false, false);
            this.burstTimer.timeout.AddListener(StartBurst);

            this.atkTimer = gameObject.AddComponent<Timer>();
            this.atkTimer.Setup(data.atkCD, false, false);
            this.atkTimer.timeout.AddListener(FinishReload);
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
            currentPlayerInput.playerInput.actions["Move"].performed -= WeaponInputMove;
            currentPlayerInput.playerInput.actions["Fire"].performed -= WeaponInputFire;
            currentPlayerInput.playerInput.actions["ToggleAutoFire"].performed -= WeaponInputAutoFire;
            DisplayManager.instance.DisplayCloseEvent -= OnDisplayClosed;
            DisplayManager.instance.ToggleMobilePlayerDisplay(true);
            currentPlayerInput.SwitchView("PlayerView");
            currentPlayerInput = null;
            InstanceFinder.TimeManager.OnTick -= OnTick;
            SyncTurnAxis(Vector2.zero);
        }

        public override void OnInteract(GameObject player)
        {
            if (manager.Offer(UIDisplay,this))
            {
                SetOccupied(true);
                InstanceFinder.TimeManager.OnTick += OnTick;
                CameraManager.instance.ToggleWeaponCamera();
                currentPlayerInput = player.GetComponent<PlayerInputManager>();
                currentPlayerInput.SwitchView("WeaponView");
                DisplayManager.instance.ToggleMobilePlayerDisplay(false);
                currentPlayerInput.playerInput.actions["Move"].performed += WeaponInputMove;
                currentPlayerInput.playerInput.actions["Fire"].performed += WeaponInputFire;
                currentPlayerInput.playerInput.actions["ToggleAutoFire"].performed += WeaponInputAutoFire;
                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
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

        [ServerRpc(RequireOwnership = false)]
        public void ToggleAutoFire()
        {
            autoFire = !autoFire;
        }

        public void FinishReload()
        {
            canFire = true;
            if (autoFire)
            {
                StartAttack();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void Fire()
        {
            if (canFire)
            {
                StartAttack();
            }
        }

        public void StartAttack()
        {
            if (!IsServer) { return; }//Sanity check
            canFire = false;
            OnFireClient();
            atkTimer.Start();
            StartBurst();
        }

        private void StartBurst()
        {
            if (!IsServer) { return; }//Sanity check

            if (currentBurst < data.burst)
            {
                if (autoFire) {; }
                //Left();
                foreach (Shooter comp in shooters)
                {
                    comp.SpawnBullet();
                    AudioManager.instance.ObserversPlay(data.shootSound);
                }
                currentBurst += 1;
                this.burstTimer.Start();

            }
            else
            {
                currentBurst = 0;
            }

        }

        public void OnTick()
        {
            
            SyncTurnAxis(turnAxis);
            if (firing) 
            {
                Fire();
                firing = false;
            }
                
            if (togglingAutoFire)
            {
                ToggleAutoFire();
                Fire();
                togglingAutoFire = false;
            }
        }

        [ObserversRpc]
        public void OnFireClient()
        {
            atkTimer.Start();
        }
        private void Update()
        {
            percentageReloaded = 1 - (float)((atkTimer.waitTime - atkTimer.currentTime) / data.atkCD);
        }
    }
};