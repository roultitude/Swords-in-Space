using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FishNet.Object;

namespace SwordsInSpace
{
    public class BossCatChargerMover : DefaultMover, RageInterface
    {
        private double currentTime = -10;
        public double dashCD;
        public double rageDashCD;
        public double dashImpulse;
        public Rigidbody2D rb;
        public Collider2D enemyCollider;
        private EnemyAnimator anim;
        private SpriteRenderer sprite;
        private void Awake()
        {
            anim = GetComponentInChildren<EnemyAnimator>();
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        public void Update()
        {
            if (!IsServer) return;

            currentTime += Time.deltaTime;
            sprite.flipY = transform.rotation.eulerAngles.z < 180;

            if (ai.reachedDestination && currentTime > dashCD)
            {
                LookAtPlayer();
                StartCoroutine(Dash());
                currentTime = 0;
            }

        }

        public IEnumerator Dash()
        {
            if (dashCD == rageDashCD)
            {
                anim.CrossFadeObserver("RageAttack");
            }
            else anim.CrossFadeObserver("Attack");
            double currentDashTime = 0;
            double maxDashTime = 2;
            StopAstar();
            enemyCollider.isTrigger = false;
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
            if (dashCD == rageDashCD)
            {
                anim.CrossFadeObserver("RageIdle");
            }
            else anim.CrossFadeObserver("Idle");
        }

        public void StartRagePhase()
        {
            dashCD = rageDashCD;
            TintRage();
            anim.CrossFadeObserver("RageIdle");
        }

        [ObserversRpc]
        void TintRage()
        {
            sprite.DOColor(new Color(1f, 0.8f, 0.8f), 2f);
        }
    }
};
