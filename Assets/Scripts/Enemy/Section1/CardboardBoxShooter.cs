using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class CardboardBoxShooter : EnemyShooter
    {

        public float bulletRotationSpeed;
        public int numRotatingBullets;

        private int burst = 3;
        private float burstcd = 1.3f;
        public GameObject bulletLocations;

        public float rotationPerShot;
        public float rotatingBulletSpeed;


        public class RotatingBullet
        {
            CardboardBoxShooter shooter;
            Vector3 currentShooterPos;
            float rotation;

            float currentTime = 0f;
            float startSpinTime = 0.3f;



            public RotatingBullet(CardboardBoxShooter shooter, float rotation)
            {
                this.shooter = shooter;
                this.currentShooterPos = shooter.gameObject.transform.position;
                this.rotation = rotation;
            }

            public void OnRotatingBulletMove(GameObject obj)
            {
                currentTime += Time.deltaTime;


                obj.transform.position += (shooter.transform.position - currentShooterPos);
                currentShooterPos = shooter.transform.position;
                if (currentTime > startSpinTime)
                {
                    obj.transform.rotation *= Quaternion.Euler(0, 0, rotation * Time.deltaTime);
                }
                else
                {
                    obj.transform.rotation *= Quaternion.Euler(0, 0, rotation * (Time.deltaTime * 3));
                }

            }
        }

        public override void Shoot()
        {
            if (!IsServer) return;

            StartCoroutine(StartCrossShots());
            Quaternion rot = Quaternion.Euler(0, 0, 0);
            for (int i = 0; i < numRotatingBullets; i++)
            {
                SpawnLocalRotation(customBulletSpeed: rotatingBulletSpeed, offset: rot, loc: transform.position, fn: new RotatingBullet(this, bulletRotationSpeed).OnRotatingBulletMove);
                rot *= Quaternion.Euler(0, 0, 360f / numRotatingBullets);
            }

        }



        public IEnumerator StartCrossShots()
        {
            for (int i = 0; i < burst; i++)
            {

                for (int quadrant = 0; quadrant < 4; quadrant++)
                {
                    foreach (Transform child in bulletLocations.transform)
                    {
                        SpawnLocalRotation(offset: child.rotation, loc: child.position);
                    }
                    bulletLocations.transform.rotation *= Quaternion.Euler(0, 0, 90);

                }


                bulletLocations.transform.rotation *= Quaternion.Euler(0, 0, 45);
                yield return new WaitForSeconds(burstcd);
            }

        }
    }
};