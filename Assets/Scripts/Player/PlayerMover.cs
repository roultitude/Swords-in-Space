using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;

namespace SwordsInSpace
{
    public class PlayerMover : NetworkBehaviour
    {
        [SerializeField]
        private float speed, rotationSpeed;
        private float lastInputAngle;
        public Rigidbody2D rb;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }


        private void Update()
        {

        }
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsClientOnly)
            {
                //rb.isKinematic = true;
            }
        }

        public void Move(PlayerInputManager.MoveData md, bool asServer, bool replaying = false)
        {
            MovePredict(md, asServer, replaying);
        }
        [Replicate]
        private void MovePredict(PlayerInputManager.MoveData md, bool asServer, bool replaying = false)
        {
            Vector2 moveXY = new Vector2(md.Horizontal,md.Vertical).normalized * speed * (float)base.TimeManager.TickDelta;
            //Vector2 newPos = new Vector2(rb.transform.position.x + moveXY.x, rb.transform.position.y + moveXY.y);
            //rb.MovePosition(newPos);
            //rb.velocity = moveXY;
            rb.AddForce(moveXY);

            float angle;
            if (md.Vertical == 0 && md.Horizontal == 0)
            {
                angle = lastInputAngle;
            }
            else
            {
                angle = Mathf.Atan2(md.Vertical, md.Horizontal) * Mathf.Rad2Deg;
            }
            lastInputAngle = angle;
            
            rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), (float)base.TimeManager.TickDelta * rotationSpeed);
        }

        public void Reconciliation(PlayerInputManager.ReconcileData rd, bool asServer)
        {
            ReconciliationPredict(rd, asServer);
        }

        [Reconcile]
        private void ReconciliationPredict(PlayerInputManager.ReconcileData rd,bool asServer)
        {
            transform.position = rd.Position;
            transform.rotation = rd.Rotation;
            rb.velocity = rd.Velocity;
            rb.angularVelocity = rd.AngularVelocity;
        }

    }
}