using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;

namespace SwordsInSpace
{
    public class PlayerMover : NetworkBehaviour
    {
        public Rigidbody2D rb;
        public bool canMove;

        [SerializeField]
        private float speed, rotationSpeed, dashMultiplier;

        private float lastInputAngle;
        private Vector2 shipLastFrame;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            canMove = true; //canMove from start for testing.
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsClientOnly)
            {
                //rb.isKinematic = true;
            }
            shipLastFrame = Ship.currentShip.transform.position;
        }
        private void Update()
        {
            //Vector2 shipThisFrame = Ship.currentShip.transform.position;
            //Vector2 positionOffset = shipThisFrame - shipLastFrame;
           // shipLastFrame = shipThisFrame;
           // transform.position = (Vector2) transform.position + positionOffset;
        }
        public void Move(PlayerInputManager.MoveData md, bool asServer, bool replaying = false)
        {
            if (!canMove) md = default;
            MovePredict(md, asServer, replaying);
        }

        [Replicate]
        private void MovePredict(PlayerInputManager.MoveData md, bool asServer, bool replaying = false)
        {
            Vector2 moveXY = new Vector2(md.Horizontal, md.Vertical).normalized * speed * (float)base.TimeManager.TickDelta;
            Vector2 moveXYtrans = Ship.currentShip.shipExterior.TransformVector(moveXY);
            //Vector2 newPos = new Vector2(rb.transform.position.x + moveXY.x, rb.transform.position.y + moveXY.y);
            //rb.MovePosition(newPos);
            //rb.velocity = moveXY;
            
            rb.AddForce(moveXYtrans);
            if (md.Dashing) rb.AddForce(moveXYtrans * dashMultiplier, ForceMode2D.Impulse);
            float angle;
            if (md.Vertical == 0 && md.Horizontal == 0)
            {
                angle = lastInputAngle;
            }
            else
            {
                angle = Mathf.Atan2(moveXYtrans.y, moveXYtrans.x) * Mathf.Rad2Deg;
            }
            lastInputAngle = angle;
            
            rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), (float)base.TimeManager.TickDelta * rotationSpeed);
        }

        public void Reconciliation(PlayerInputManager.PlayerReconcileData rd, bool asServer)
        {
            ReconciliationPredict(rd, asServer);
        }

        [Reconcile]
        private void ReconciliationPredict(PlayerInputManager.PlayerReconcileData rd,bool asServer)
        {
            transform.localPosition = rd.LocalPosition;
            transform.localRotation = rd.LocalRotation;
            rb.velocity = rd.Velocity;
            rb.angularVelocity = rd.AngularVelocity;
        }

    }
}