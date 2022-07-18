using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using static SwordsInSpace.UpgradeSO;

namespace SwordsInSpace
{
    public class Shielder : NetworkBehaviour
    {

        public Vector2 size = new Vector2(10, 10);
        public Vector2 turnAxis = new Vector2(0, 0);

        [SerializeField]
        private float turnSpeed, baseShieldSize;


        private float currentTurnSpeed;
        private float currentShieldSize;
        // Start is called before the first frame update
        private float pos = 0f;
        public override void OnStartServer()
        {
            base.OnStartServer();
            Ship.currentShip.upgradeManager.OnUpgrade += ReloadUpgrades;

        }
        public void ReloadUpgrades(Dictionary<UpgradeTypes, float> stats)
        {
            if (!IsServer) return;

            if (stats.ContainsKey(UpgradeTypes.shieldSize))
            {
                float newShieldSize = baseShieldSize + stats[UpgradeTypes.shieldSize];
                newShieldSize = Mathf.Clamp(newShieldSize, 0.1f, 10f);
                gameObject.transform.localScale = new Vector3(newShieldSize, newShieldSize, 0);
            }

            if (stats.ContainsKey(UpgradeTypes.shieldTurnSpeed))
            {
                float newShieldTurnSpeed = turnSpeed + stats[UpgradeTypes.shieldTurnSpeed];
                currentTurnSpeed = Mathf.Clamp(newShieldTurnSpeed, 0.1f, 20f);
            }


        }

        private void Update()
        {
            if (!IsServer) return;
            pos += turnAxis.x * currentTurnSpeed * Time.deltaTime;
            Reposition(pos);

            var rot = Quaternion.LookRotation(Vector3.forward, gameObject.transform.localPosition) * Quaternion.Euler(0, 0, 90);
            gameObject.transform.localRotation = Quaternion.RotateTowards(gameObject.transform.rotation, rot, 360f);

        }




        public void Reposition(float pos)
        {
            transform.localPosition = new Vector2(size.x * (float)Math.Sin(pos), size.y * (float)Math.Cos(pos));
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            Bullet bullet = collision.gameObject.GetComponentInParent<Bullet>();
            //Debug.Log(bullet.gameObject.tag);
            if (bullet != null && (bullet.gameObject.tag == null || bullet.gameObject.tag != "Friendly"))
            {
                bullet.OnHit();
            }
        }
    }
};
