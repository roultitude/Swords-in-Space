using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class Needle : EnemyMover
    {

        public float dashChargeTime;
        public double dashSpeed;
        public float dashTime;

        private Vector2 targetLocation;

        private enum STATE
        {
            MOVING,
            DASHING
        }
        [SerializeField]
        private STATE currentState = STATE.MOVING;
        new public void Start()
        {
            base.Start();
            onDash.AddListener(AfterDash);
        }
        public void Update()
        {
            if (ai.reachedDestination && ai.canMove && currentState == STATE.MOVING)
            {
                OnReachDestination();
            }

        }

        private void OnReachDestination()
        {
            if (currentState == STATE.DASHING)
                return;
            currentState = STATE.DASHING;
            StopAstar();
            targetLocation = Ship.currentShip.transform.position;
            StartCoroutine(ChargeDash());
        }

        private IEnumerator ChargeDash()
        {
            LookAtPlayer();
            new WaitForSeconds(dashChargeTime);
            StartCoroutine(Dash(dashSpeed, dashTime));
            yield break;
        }

        public void AfterDash()
        {
            currentState = STATE.MOVING;

            ContinueAstar();
        }

    }
};
