using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class NeedleMover : EnemyMover
    {

        public float dashChargeTime;
        public double dashSpeed;
        public float dashTime;
        public float dashCD;

        private Vector2 targetLocation;

        public enum STATE
        {
            MOVING,
            CHARGEDASH,
            DASHING
        }
        [SerializeField]
        public STATE currentState = STATE.MOVING;

        private double currentTime = 0;
        Rigidbody2D rb;
        new public void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody2D>();
        }
        public void Update()
        {
            currentTime += Time.deltaTime;


            if (ai.reachedDestination && ai.canMove && currentState == STATE.MOVING && currentTime >= dashCD && CanSeePlayer())
            {
                OnReachDestination();
            }
            else if (currentState == STATE.CHARGEDASH && currentTime >= dashChargeTime)
            {
                OnFinishCharging();
            }
            else if (currentState == STATE.DASHING)
            {
                Dash();

                if (currentTime >= dashTime)
                {
                    OnFinishDashing();
                }

            }



        }

        public void OnTouchPlayer()
        {
            if (currentState == STATE.DASHING)
            {
                OnFinishDashing();
            }
        }


        private void OnReachDestination()
        {
            currentTime = 0;
            if (currentState == STATE.DASHING)
                return;
            currentState = STATE.DASHING;
            StopAstar();
            targetLocation = Ship.currentShip.transform.position;
            LookAtPlayer();
        }

        private void OnFinishCharging()
        {
            currentTime = 0;
            currentState = STATE.DASHING;

        }

        private void OnFinishDashing()
        {
            currentTime = 0;
            currentState = STATE.MOVING;
            rb.angularVelocity = 0f;
            rb.velocity = new Vector2(0, 0);
            ContinueAstar();
        }


        private void Dash()
        {
            rb.AddRelativeForce(new Vector2((float)dashSpeed, 0), ForceMode2D.Impulse);
            rb.angularVelocity = 0f;

            //transform.position += transform.right * Time.deltaTime * (float)dashSpeed;

        }

    }
};
