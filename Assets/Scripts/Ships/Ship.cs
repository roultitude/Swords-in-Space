using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        [SerializeField]
        ShipSO data;

        [SerializeField]
        GameObject UiHpBar;

        public double CurrentHp;

        private void Awake()
        {
            currentShip = this;
            shipMover = this.GetComponentInChildren<ShipMover>();
            CurrentHp = data.MaxHp;

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

                CurrentHp -= 1;
                bullet.OnHit();
                updateHpBar();

                if (CurrentHp <= 0)
                {
                    Despawn();
                }

            }
        }
        [ObserversRpc]
        public void updateHpBar()
        {
            UiHpBar.GetComponent<UIHpBar>().Resize((float)CurrentHp / (float)data.MaxHp);
        }

        [ServerRpc(RequireOwnership = false)]
        public void addHp(int amt)
        {
            CurrentHp += amt;
            if (CurrentHp > data.MaxHp)
                CurrentHp = data.MaxHp;

            updateHpBar();
        }

        [ServerRpc(RequireOwnership = false)]
        public void changePilot(NetworkConnection conn = null)
        {
            if (Owner.IsActive) return; //serverside check for ownership
            Debug.Log("pilot changed to " + conn.ClientId);
            base.GiveOwnership(conn);
        }
        [ServerRpc(RequireOwnership = false)]
        public void leavePilot(NetworkConnection conn = null)
        {
            if (Owner == conn)
            {
                base.RemoveOwnership();
                Debug.Log("pilot changed to none");
            }
        }
    }

}
