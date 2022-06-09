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
        
        [SyncVar]
        public SteerState currentSteerState;

        [SerializeField]
        private float speed, turnSpeed, nitroMult, backMult;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            canMove = false; //canMove from start for testing.
            currentSteerState = SteerState.OFF;
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
            //Vector2 newPos = new Vector2(rb.transform.position.x + moveXY.x, rb.transform.position.y + moveXY.y);
            //rb.MovePosition(newPos);
            //rb.velocity = moveXY;
            float mod = 0;
            if (currentSteerState == SteerState.NITRO) mod = nitroMult;
            else if (currentSteerState == SteerState.BACKWARD) mod = backMult;
            else if (currentSteerState == SteerState.FORWARD) mod = 1;
            rb.AddForce(mod * transform.right * speed);
            float targetRotation = rb.rotation - md.Horizontal * turnSpeed;
            rb.rotation = targetRotation;
            //rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, Quaternion.AngleAxis(targetRotation, Vector3.forward), (float)base.TimeManager.TickDelta * turnSpeed);
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