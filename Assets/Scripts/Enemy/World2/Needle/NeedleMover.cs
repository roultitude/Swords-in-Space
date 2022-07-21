using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class NeedleMover : EnemyMover
    {

        public float dashChargeTime;
        public double dashImpulse;
        public float dashTime;
        public float dashCD;

        public NeedleAI needleAI;


        public enum STATE
        {
            MOVING,
            CHARGEDASH,
            DASHING
        }
        [SerializeField]
        public STATE currentState = STATE.MOVING;

        private double currentTime = 0;
        public Rigidbody2D rb;

        new public void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody2D>();
        }
        public override void OnDisableEnemyMover()
        {
            currentState = STATE.MOVING;
            StopAstar();
        }

        public override void OnEnableEnemyMover()
        {
            ContinueAstar();
            currentState = STATE.MOVING;
            currentTime = 0;
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
            currentState = STATE.CHARGEDASH;
            StopAstar();
            LookAtPlayer();
        }

        private void OnFinishCharging()
        {
            currentTime = 0;
            currentState = STATE.DASHING;
            needleAI.onStartStopDash();

        }

        private void OnFinishDashing()
        {
            needleAI.onStartStopDash();
            currentTime = 0;
            currentState = STATE.MOVING;
            rb.angularVelocity = 0f;
            rb.velocity = new Vector2(0, 0);
            ContinueAstar();

        }


        private void Dash()
        {
            rb.AddRelativeForce(new Vector2(0, (float)dashImpulse * Time.deltaTime), ForceMode2D.Impulse);
            rb.angularVelocity = 0f;

            //transform.position += transform.right * Time.deltaTime * (float)dashSpeed;

        }


    }
};
