using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using static SwordsInSpace.UpgradeSO;

namespace SwordsInSpace
{
    public class ShipMover : NetworkBehaviour
    {
        public Rigidbody2D rb;
        public bool canMove;
        public Animator shipAnimator;


        public float nitroInvincibilityTime, speed, nitroMult, turnSpeed;

        [SyncVar]
        private float currentSpeed, currentNitroMult, currentTurnSpeed;
        [SyncVar(OnChange = nameof(OnChangeIsMoving))]
        public bool isMoving;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            canMove = false; //canMove from start for testing.

        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsClientOnly)
            {
                //rb.isKinematic = true;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Ship.currentShip.upgradeManager.OnUpgrade += ReloadUpgrades;
            currentSpeed = speed;
            currentNitroMult = nitroMult;
            currentTurnSpeed = turnSpeed;

        }
        public void ReloadUpgrades(Dictionary<UpgradeTypes, float> stats)
        {
            if (!IsServer) return;

            if (stats.ContainsKey(UpgradeTypes.shipSpeed))
            {
                float newCurrentSpeed = speed + stats[UpgradeTypes.shipSpeed];
                currentSpeed = Mathf.Clamp(newCurrentSpeed, 1f, 10000f);
            }

            if (stats.ContainsKey(UpgradeTypes.shipTurnSpeed))
            {
                float newTurnSpeed = turnSpeed + stats[UpgradeTypes.shipTurnSpeed];
                currentTurnSpeed = Mathf.Clamp(newTurnSpeed, 1f, 1000f);
            }
            if (stats.ContainsKey(UpgradeTypes.nitroMult))
            {
                float newNitroMult = nitroMult + stats[UpgradeTypes.nitroMult];
                currentNitroMult = Mathf.Clamp(newNitroMult, 50f, 10000f);
            }


        }

        public void Move(MoveData md, bool asServer, bool replaying = false)
        {
            if (!canMove) md = default;
            MovePredict(md, asServer, replaying);
            if(IsClient && !asServer)
            {
                SetIsMoving((md.Vertical != 0 || md.Horizontal != 0));
            }
        }

        [Replicate]
        private void MovePredict(MoveData md, bool asServer, bool replaying = false)
        {
            rb.AddForce(md.Vertical * transform.right * currentSpeed);
            if (md.Dashing && Ship.currentShip.CurrentNitroFuel > 0)
            {
                rb.AddForce(1 * transform.right * currentSpeed * (currentNitroMult - 1));
                Ship.currentShip.ChangeNitroFuel(-1);
                if (IsServer) Ship.currentShip.StartCoroutine(Ship.currentShip.StartInvincibilityFrames(nitroInvincibilityTime));
            }
            rb.AddTorque(md.Horizontal * -currentTurnSpeed);
        }

        public void Reconciliation(ReconcileData rd, bool asServer)
        {
            ReconciliationPredict(rd, asServer);
        }

        [Reconcile]
        private void ReconciliationPredict(ReconcileData rd, bool asServer)
        {
            transform.position = rd.Position;
            transform.rotation = rd.Rotation;
            rb.velocity = rd.Velocity;
            rb.angularVelocity = rd.AngularVelocity;
        }

        private void OnChangeIsMoving(bool oldMoving, bool newMoving, bool asServer)
        {
            if (newMoving) shipAnimator.CrossFade("ShipMoving", 0, 0);
            else shipAnimator.CrossFade("ShipIdle", 0, 0);
        }
        [ServerRpc]
        void SetIsMoving(bool newIsMoving)
        {
            isMoving = newIsMoving;
        }
    }
}