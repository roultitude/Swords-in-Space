using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class BossCatFencerShooter : EnemyShooter, RageInterface
    {
        // Start is called before the first frame update

        public GameObject leftClaw;
        public GameObject rightClaw;


        public float clawStrikeBurstCD;
        public float clawStrikeBulletDistanceScale;

        public GameObject mouth;
        public int mouthShots;
        public float mouthShotBurstCD;
        private EnemyAnimator anim;
        public EnemyMover mover;

        public bool isRagePhase = false;

        private void Awake()
        {
            anim = mover.GetComponentInChildren<EnemyAnimator>();
        }

        public class BulletMovement
        {
            float currentTime = 0f;

            float maxTime = 0.8f;
            bool enabled = true;
            float rot;
            public BulletMovement(float rot)
            {
                this.rot = rot;
            }
            public void MovementFn(GameObject bullet)
            {
                if (!enabled) return;

                currentTime += Time.deltaTime;
                if (currentTime > maxTime)
                {
                    enabled = false;

                    bullet.transform.rotation = Quaternion.Euler(0, 0, rot) * bullet.transform.rotation;
                    /*

                    Vector3 myLocation = bullet.transform.position;
                    Vector3 targetLocation = Ship.currentShip.transform.position;
                    targetLocation.z = myLocation.z;
                    Vector3 vectorToTarget = targetLocation - myLocation;

                    Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
                    Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

                    bullet.transform.rotation = Quaternion.RotateTowards(bullet.transform.rotation, targetRotation, 360f);
                    */
                }
            }
        }


        public override void Shoot()
        {
            if (isRagePhase)
            {
                StartCoroutine(RageClawStrike());
            }
            else
            {
                StartCoroutine(ClawStrike());
            }


        }

        public IEnumerator ClawStrike()
        {
            
            foreach (GameObject obj in new GameObject[] { leftClaw, rightClaw })
            {
                mover.LookAtPlayer();
                foreach (Transform pos in obj.transform)
                {
                    ShootAt(pos.position, Ship.currentShip.transform.position);
                }
                anim.CrossFadeObserver("Attack");
                yield return new WaitForSeconds(clawStrikeBurstCD);

            }

            for (int i = 0; i < mouthShots; i++)
            {
                ShootAt(mouth.transform.position, Ship.currentShip.transform.position);
                anim.CrossFadeObserver("Attack");
                yield return new WaitForSeconds(mouthShotBurstCD);
            }
            anim.CrossFadeObserver("Idle");
        }

        public IEnumerator RageClawStrike()
        {
            
            for (int i = 0; i < 8; i++)
            {


                float offset = 15;
                mover.LookAtPlayer();

                SpawnLocalRotation(Quaternion.Euler(0, 0, offset), leftClaw.transform.position, fn: new BulletMovement(-offset * 2).MovementFn);

                SpawnLocalRotation(Quaternion.Euler(0, 0, -offset), rightClaw.transform.position, fn: new BulletMovement(offset * 2).MovementFn);

                offset += 10f;
                anim.CrossFadeObserver("RageAttack");
                yield return new WaitForSeconds(clawStrikeBurstCD);

            }


            for (int i = 0; i < mouthShots * 3; i++)
            {
                ShootAt(mouth.transform.position, Ship.currentShip.transform.position);
                anim.CrossFadeObserver("RageAttack");
                yield return new WaitForSeconds(mouthShotBurstCD / 2);
            }

        }

        public void StartRagePhase()
        {
            isRagePhase = true;
        }
    }

};