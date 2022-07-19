using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using static SwordsInSpace.UpgradeSO;

namespace SwordsInSpace
{
    public abstract class Weapon : Module
    {
        // Start is called before the first frame update
        [SerializeField]
        public WeaponSO baseData;

        public struct WeaponStruct
        {
            public WeaponStruct(WeaponSO baseData)
            {
                turretSprite = baseData.turretSprite;
                bulletPrefab = baseData.bulletPrefab;
                shootSound = baseData.shootSound;
                damage = baseData.damage;
                shotSpeed = baseData.shotSpeed;
                shotLifeTime = baseData.shotLifeTime;
                shotSpread = baseData.shotSpread;
                burst = baseData.burst;
                burstCD = baseData.burstCD;
                atkCD = baseData.atkCD;
                bulletScale = baseData.bulletScale;
                rotationSpeed = baseData.rotationSpeed;
                pierce = baseData.pierce;
            }
            public Sprite turretSprite;
            public GameObject bulletPrefab;
            public AudioClip shootSound;
            public double damage;
            public float shotSpeed;
            public double shotLifeTime;
            public double shotSpread;
            public int burst;
            public int pierce;
            public double atkCD;
            public double burstCD;
            public Vector2 bulletScale;
            public float rotationSpeed;
        }

        public WeaponStruct data;

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
        public bool isSuperWeapon;
        Timer burstTimer;
        Timer atkTimer;

        [SyncVar]
        public bool canFire = true;
        bool autoFire = false;

        int currentBurst = 0;
        private AttackManager attackManager;

        public float percentageReloaded;

        void OnEnable()
        {
            shooters = new List<Shooter>();
            data = new WeaponStruct(baseData);

            foreach (GameObject comp in shootersObj)
            {
                Shooter compShooter = comp.GetComponentInChildren<Shooter>();
                shooters.Add(compShooter);
                compShooter.UpdateRotSpeed(baseData.rotationSpeed);
            }

            attackManager = GetComponent<AttackManager>();

            this.burstTimer = gameObject.AddComponent<Timer>();
            this.burstTimer.Setup(data.burstCD, false, false);
            this.burstTimer.timeout.AddListener(StartBurst);

            this.atkTimer = gameObject.AddComponent<Timer>();
            this.atkTimer.Setup(data.atkCD, false, false);
            this.atkTimer.timeout.AddListener(FinishReload);
            GameManager.OnNewSceneLoadEvent += SetupUI;
            Ship.currentShip.upgradeManager.OnUpgrade += ReloadUpgrades;

        }

        public void ReloadUpgrades(Dictionary<UpgradeTypes, float> stats)
        {

            attackManager.ReloadUpgrades(stats);

            //Base increases
            foreach (UpgradeTypes type in stats.Keys)
            {
                switch (type)
                {
                    case UpgradeTypes.shotSpeed:
                        data.shotSpeed = baseData.shotSpeed + stats[type];
                        break;

                    case UpgradeTypes.shotDamage:
                        data.damage = baseData.damage + stats[type];
                        if (data.damage <= 0.1)
                        {
                            data.damage = 0.1;
                        }
                        break;

                    case UpgradeTypes.shotLifetime:
                        data.shotLifeTime = baseData.shotLifeTime + stats[type];
                        if (data.shotLifeTime < 0.1)
                        {
                            data.shotLifeTime = 0.1;
                        }
                        break;

                    case UpgradeTypes.shotBurst:
                        int newBurst = baseData.burst + (int)stats[type];
                        newBurst = (int)Mathf.Clamp(newBurst, 1, int.MaxValue);
                        data.burst = newBurst;

                        break;

                    case UpgradeTypes.shotSpread:
                        double newShotSpread = baseData.shotSpread + stats[type];
                        newShotSpread = (int)Mathf.Clamp((float)newShotSpread, 0, 360);
                        data.shotSpread = newShotSpread;
                        break;


                }
            }

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
            if (manager.Offer(UIDisplay, this))
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

            StartBurst();
        }

        private void StartBurst()
        {
            if (!IsServer) { return; }//Sanity check

            if (currentBurst < data.burst)
            {
                if (autoFire) {; }

                foreach (Shooter comp in shooters)
                {
                    if (isSuperWeapon)
                    {
                        GameObject toAdd = Instantiate(data.bulletPrefab, comp.transform.position, Quaternion.Euler(comp.transform.rotation.eulerAngles));
                        Lazer lazerSetup = toAdd.GetComponent<Lazer>();
                        if (lazerSetup != null)
                            lazerSetup.SetupLazer(data.shotLifeTime, data.damage, data.shotSpread, data.burstCD, data.shootSound, comp);

                        toAdd.tag = "Friendly";

                        Spawn(toAdd);
                        continue;
                    }



                    List<AttackManager.BulletInfo> toSpawn = attackManager.DraftBulletLocations(data.bulletPrefab, comp.transform.position, comp.transform.rotation.eulerAngles, data.shotSpeed, data.shotSpread, data.shotLifeTime, data.damage, data.bulletScale, data.pierce);


                    foreach (AttackManager.BulletInfo spawnData in toSpawn)
                    {
                        if (!spawnData.isValid) continue;

                        GameObject toAdd = Instantiate(spawnData.bulletBase, spawnData.bulletPosition, Quaternion.Euler(spawnData.bulletRotation));
                        Bullet toAddBullet = toAdd.GetComponent<Bullet>();

                        toAddBullet.AddMovementFunction(spawnData.CallOnMove);
                        toAddBullet.AddOnHitFunction(spawnData.CallOnHit);
                        toAddBullet.AddDespawnFunction(spawnData.CallOnDespawn);

                        toAddBullet.Setup(spawnData.bulletShotSpeed, spawnData.bulletShotLifeTime, spawnData.bulletDamage, spawnData.bulletShotSpread, spawnData.bulletPierce);
                        toAdd.transform.localScale = spawnData.bulletScale;

                        toAdd.tag = "Friendly";

                        Spawn(toAdd);
                    }
                    AudioManager.instance.ObserversPlay(data.shootSound);
                }
                currentBurst += 1;
                this.burstTimer.Start();

                if (currentBurst >= data.burst)
                {
                    atkTimer.Start();
                }

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