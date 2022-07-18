using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using static SwordsInSpace.UpgradeSO;

namespace SwordsInSpace
{
    public class PlayerMover : NetworkBehaviour
    {
        public Rigidbody2D rb;
        public bool canMove;



        [SerializeField]
        private float speed, rotationSpeed, dashMultiplier;

        [SyncVar]
        private float currentSpeed, currentDashMultiplier;

        private float lastInputAngle;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            canMove = true; //canMove from start for testing.
        }


        public override void OnStartServer()
        {
            base.OnStartServer();
            Ship.currentShip.upgradeManager.OnUpgrade += ReloadUpgrades;

        }
        public void ReloadUpgrades(Dictionary<UpgradeTypes, float> stats)
        {
            if (!IsServer) return;

            if (stats.ContainsKey(UpgradeTypes.playerMoveSpeed))
            {
                float newSpeed = speed + stats[UpgradeTypes.playerMoveSpeed];
                currentSpeed = Mathf.Clamp(newSpeed, 100f, 10000f);
            }

            if (stats.ContainsKey(UpgradeTypes.playerDashMult))
            {
                float newDashMultiplier = dashMultiplier + stats[UpgradeTypes.playerDashMult];
                currentDashMultiplier = Mathf.Clamp(newDashMultiplier, 0.05f, 5f);
            }


        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsClientOnly)
            {
                //rb.isKinematic = true;
            }
        }

        public void Move(MoveData md, bool asServer, bool replaying = false)
        {
            if (!canMove) md = default;
            MovePredict(md, asServer, replaying);
        }

        [Replicate]
        private void MovePredict(MoveData md, bool asServer, bool replaying = false)
        {
            Vector2 moveXY = new Vector2(md.Horizontal, md.Vertical).normalized * speed * (float)base.TimeManager.TickDelta;
            //Vector2 moveXYtrans = Ship.currentShip.shipExterior.TransformVector(moveXY);
            //Vector2 newPos = new Vector2(rb.transform.position.x + moveXY.x, rb.transform.position.y + moveXY.y);
            //rb.MovePosition(newPos);
            //rb.velocity = moveXY;

            rb.AddForce(moveXY);
            if (md.Dashing) rb.AddForce(moveXY * dashMultiplier, ForceMode2D.Impulse);
            float angle;
            if (md.Vertical == 0 && md.Horizontal == 0)
            {
                angle = lastInputAngle;
            }
            else
            {
                angle = Mathf.Atan2(moveXY.y, moveXY.x) * Mathf.Rad2Deg;
            }
            lastInputAngle = angle;

            rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), (float)base.TimeManager.TickDelta * rotationSpeed);
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

    }
}