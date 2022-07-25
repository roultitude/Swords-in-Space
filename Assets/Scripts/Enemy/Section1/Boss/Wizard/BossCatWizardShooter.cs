using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{

    public class BossCatWizardShooter : EnemyShooter, RageInterface
    {
        public GameObject attackIndicator;

        public Vector2 normalAttackSpawnRangeFromShip;
        public int numNormalAttackIndicator;
        public int normalAttackIndicatorExplosionShots;

        public float rageCD;
        private bool isRaging = false;
        private EnemyAnimator anim;

        private void Awake()
        {
            anim = GetComponentInParent<BossCatWizardAI>().GetComponentInChildren<EnemyAnimator>();
        }
        public class BulletMovement
        {
            float currentTime = 0f;

            float maxTime = 0.2f;
            bool enabled = true;
            Vector3 origin;

            public BulletMovement(Vector3 origin)
            {
                this.origin = origin;
            }
            public void MovementFn(GameObject bullet)
            {
                if (!enabled) return;

                currentTime += Time.deltaTime;
                if (currentTime > maxTime)
                {
                    enabled = false;
                    Vector3 myLocation = origin;
                    Vector3 targetLocation = Ship.currentShip.transform.position;
                    targetLocation.z = myLocation.z;
                    Vector3 vectorToTarget = targetLocation - myLocation;

                    Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
                    Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

                    bullet.transform.rotation = Quaternion.RotateTowards(bullet.transform.rotation, targetRotation, 360f);

                }
            }
        }


        public override void Shoot()
        {
            /*Non Rage phase
                   Spawns 2 attackIndicators that explode in a burst of 36. Bullets have a slow move speed.
             */
            StartCoroutine(SpawnAttackIndicators());


        }



        public IEnumerator SpawnAttackIndicators()
        {
            for (int i = 0; i < numNormalAttackIndicator; i++)
            {

                Vector3 toSpawnLocalPos = new Vector3(Random.Range(normalAttackSpawnRangeFromShip.x, normalAttackSpawnRangeFromShip.y)
                    , Random.Range(normalAttackSpawnRangeFromShip.x, normalAttackSpawnRangeFromShip.y), 0);

                //Rotate to other quadrants
                if (Random.Range(0, 2) == 0)
                {
                    toSpawnLocalPos.x = -toSpawnLocalPos.x;
                }
                if (Random.Range(0, 2) == 0)
                {
                    toSpawnLocalPos.y = -toSpawnLocalPos.y;
                }

                GameObject toSpawn = Instantiate(attackIndicator, transform.position + toSpawnLocalPos, Quaternion.Euler(0, 0, 0));

                Spawn(toSpawn);

                toSpawn.GetComponent<AttackIndicator>().Setup(1.6f, OnAttackIndicatorTimerEnd);
                if (isRaging) anim.CrossFadeObserver("RagingAttack");
                else anim.CrossFadeObserver("Attack");

                yield return new WaitForSeconds(Random.Range(0.3f, 0.8f));

            }
            if (isRaging) anim.CrossFadeObserver("RagingIdle");
            else anim.CrossFadeObserver("Idle");
        }

        public void OnAttackIndicatorTimerEnd(GameObject indicator)
        {
            //Explode into many bullets

            for (float angle = 0; angle < 360; angle += (360f / normalAttackIndicatorExplosionShots))
            {
                if (isRaging)
                {
                    SpawnLocalRotation(offset: Quaternion.Euler(0, 0, angle), loc: indicator.transform.position, fn: new BulletMovement(indicator.transform.position).MovementFn, customBulletSpeed: shotSpeed * 2);
                }
                else
                {
                    SpawnLocalRotation(offset: Quaternion.Euler(0, 0, angle), loc: indicator.transform.position);
                }
            }

        }

        public void StartRagePhase()
        {
            anim.CrossFadeObserver("RagingIdle");
            bulletCd.waitTime = rageCD;
            isRaging = true;
            damage = 3;
            numNormalAttackIndicator = 3;

        }
    }
};
