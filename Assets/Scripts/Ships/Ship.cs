using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static SwordsInSpace.UpgradeSO;

namespace SwordsInSpace
{
    public class Ship : NetworkBehaviour
    {
        public static Ship currentShip;
        public ShipMover shipMover;
        public Transform shipExterior;
        public Transform shipInterior;
        public Transform shipInteriorView;
        public Transform spawnTransform;
        public Transform playerTracker;
        public UpgradeManager upgradeManager;
        public ExpManager expManager;

        [SerializeField]
        ShipSO data;

        [SerializeField]
        GameObject UiHpBar;

        [SerializeField]
        Color TintNoPower;

        [SerializeField]
        GameObject background;




        [SyncVar]
        public double CurrentHp;

        [SyncVar]
        public double CurrentMaxHp;

        public bool isPowerUp = true;

        private void Awake()
        {
            currentShip = this;
            shipMover = this.GetComponentInChildren<ShipMover>();
            CurrentHp = data.ShipMaxHp;
            CurrentMaxHp = data.ShipMaxHp;

        }

        [ServerRpc(RequireOwnership = false)]
        public void ReloadStats()
        {
            Dictionary<UpgradeTypes, float> stats = upgradeManager.TallyUpgrades();
            double TallyMaxHp = data.ShipMaxHp;

            //Base increases
            foreach (UpgradeTypes type in stats.Keys)
            {
                switch (type)
                {
                    case UpgradeTypes.maxHp:
                        TallyMaxHp += stats[type];

                        break;

                }
            }

            //%Increases, to be applied after base increase
            foreach (UpgradeTypes type in stats.Keys)
            {
                switch (type)
                {
                    case UpgradeTypes.maxHpPercent:
                        TallyMaxHp *= (100 + stats[type]) / 100;
                        break;

                }
            }


            //Assignment of values
            if (CurrentMaxHp != TallyMaxHp)
                SetMaxHp(TallyMaxHp);
        }


        [ServerRpc(RequireOwnership = false)]
        private void SetMaxHp(double amt)
        {
            double hppercent = CurrentHp / CurrentMaxHp;

            CurrentMaxHp = amt;

            CurrentHp = CurrentMaxHp * hppercent;

            if (CurrentMaxHp < CurrentHp)
                CurrentHp = CurrentMaxHp;


        }


        public void LevelTransition()
        {
            if (!IsServer)
            {
                return;
            }

            int storedLevels = expManager.GetStoredLevels();
            if (storedLevels > 0)
            {
                upgradeManager.TriggerUpgrades(storedLevels);
            }
        }

        public void PowerDown()
        {

            background.GetComponent<RawImage>().color = TintNoPower;
            isPowerUp = false;
            //Kick everyone out of their UI!
            User.localUser.controlledPlayer.gameObject.GetComponent<PlayerInputManager>().ExitUI();
        }

        public void PowerUp()
        {
            background.GetComponent<RawImage>().color = Color.white;
            isPowerUp = true;
        }



        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsServer)
            {
                return;
            }

            Bullet bullet = collision.gameObject.GetComponentInParent<Bullet>();
            //Debug.Log(bullet.gameObject.tag);
            if (bullet != null && (bullet.gameObject.tag == null || bullet.gameObject.tag != "Friendly"))
            {

                CurrentHp -= bullet.damage;
                bullet.OnHit();
                UpdateHpBar();

                if (CurrentHp <= 0)
                {
                    Despawn();
                }

            }
        }


        [ObserversRpc]
        public void UpdateHpBar()
        {
            UiHpBar.GetComponent<UIHpBar>().Resize((float)CurrentHp / (float)CurrentMaxHp);
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddHp(int amt)
        {
            CurrentHp += amt;
            if (CurrentHp > CurrentMaxHp)
                CurrentHp = CurrentMaxHp;

            UpdateHpBar();
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangePilot(NetworkConnection conn = null)
        {
            if (Owner.IsActive) return; //serverside check for ownership
            Debug.Log("pilot changed to " + conn.ClientId);
            base.GiveOwnership(conn);
        }
        [ServerRpc(RequireOwnership = false)]
        public void LeavePilot(NetworkConnection conn = null)
        {
            if (Owner == conn)
            {
                base.RemoveOwnership();
                Debug.Log("pilot changed to none");
            }
        }
    }

}
