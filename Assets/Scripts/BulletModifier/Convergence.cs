using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class Convergence : BulletModifier
    {
        private class bulletMovement
        {
            float timePassed = 0.3f;
            float mainBulletSpeed;
            Vector3 targetPos;
            Vector3 targetRot;

            Bullet bull;
            bool hasCached = false;
            public bulletMovement(float bulletSpeed, Vector3 originalPos, Vector3 originalRot)
            {
                mainBulletSpeed = bulletSpeed;
                targetPos = originalPos;
                targetRot = originalRot;
                targetPos += (Quaternion.Euler(targetRot) * Vector3.right) * 20;
            }

            public void MovementModifier(GameObject bullet)
            {
                if (!hasCached)
                {
                    bull = bullet.GetComponent<Bullet>();
                }

                timePassed -= Time.deltaTime;





                if (timePassed <= 0f)
                {
                    timePassed = 88f;
                    Vector3 myLocation = bullet.transform.position;
                    Vector3 targetLocation = targetPos;
                    targetLocation.z = myLocation.z;
                    Vector3 vectorToTarget = targetLocation - myLocation;

                    Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
                    Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

                    bullet.transform.rotation = Quaternion.RotateTowards(bullet.transform.rotation, targetRotation, 360f);


                }

            }
        }
        public Convergence()
        {
            thisUpgradeAttribute = new UpgradeSO.UpgradeAttributes();
            thisUpgradeAttribute.amount = 0;
            thisUpgradeAttribute.type = UpgradeSO.UpgradeTypes.convergence;
        }



        public override void Apply(List<AttackManager.BulletInfo> info)
        {
            AttackManager.BulletInfo mainBullet = null;
            foreach (AttackManager.BulletInfo bullet in info)
            {
                if (bullet.isModifiable)
                {
                    mainBullet = bullet;
                }
            }
            if (mainBullet != null)
            {
                foreach (AttackManager.BulletInfo bullet in info)
                {
                    if (bullet == mainBullet)
                        continue;



                    bullet.CallOnMove += new bulletMovement(mainBullet.bulletShotSpeed, mainBullet.bulletPosition, mainBullet.bulletRotation).MovementModifier;

                }
            }





        }
    }
};


