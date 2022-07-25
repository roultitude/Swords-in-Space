using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class RengarMover : FacePlayerOnPathEndMover, RageInterface
    {
        private double currentTime = -10;
        public double dashCD;
        public double rageDashCD;
        public double dashImpulse;
        public Rigidbody2D rb;
        public Collider2D enemyCollider;

        public void Update()
        {
            if (!IsServer) return;

            currentTime += Time.deltaTime;


            if (ai.reachedDestination && currentTime > dashCD)
            {
                LookAtPlayer();
                StartCoroutine(Dash());
                currentTime = 0;
            }

        }

        public IEnumerator Dash()
        {

            double currentDashTime = 0;
            double maxDashTime = 2;
            StopAstar();

            while (currentDashTime < maxDashTime || (transform.position - Ship.currentShip.transform.position).magnitude < 10f)
            {
                currentTime = 0;
                currentDashTime += Time.deltaTime;

                rb.AddRelativeForce(new Vector2(0, (float)dashImpulse * Time.deltaTime), ForceMode2D.Impulse);
                rb.angularVelocity = 0f;

                yield return null;
            }

            rb.velocity = Vector2.zero;
            enemyCollider.isTrigger = false;

            ContinueAstar();
        }

        public void StartRagePhase()
        {
            dashCD = rageDashCD;
        }


    }
};
