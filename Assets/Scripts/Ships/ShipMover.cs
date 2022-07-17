using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{
    public class ShipMover : NetworkBehaviour
    {
        public Rigidbody2D rb;
        public bool canMove;


        public float speed, turnSpeed, nitroMult, nitroInvincibilityTime;

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

        public void Move(MoveData md, bool asServer, bool replaying = false)
        {
            if (!canMove) md = default;
            MovePredict(md, asServer, replaying);
        }

        [Replicate]
        private void MovePredict(MoveData md, bool asServer, bool replaying = false)
        {
            rb.AddForce(md.Vertical * transform.right * speed);
            if (md.Dashing && Ship.currentShip.CurrentNitroFuel > 0)
            {
                rb.AddForce(1 * transform.right * speed * (nitroMult - 1));
                Ship.currentShip.ChangeNitroFuel(-1);
                if (IsServer) Ship.currentShip.StartCoroutine(Ship.currentShip.StartInvincibilityFrames(nitroInvincibilityTime));
            }
            rb.AddTorque(md.Horizontal * -turnSpeed);
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