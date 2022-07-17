using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class SpreadShot : BulletModifier
    {
        public SpreadShot()
        {
            thisUpgradeAttribute = new UpgradeSO.UpgradeAttributes();
            thisUpgradeAttribute.amount = 0;
            thisUpgradeAttribute.type = UpgradeSO.UpgradeTypes.spreadShot;
        }



        public override void Apply(List<AttackManager.BulletInfo> info)
        {
            List<AttackManager.BulletInfo> result = new List<AttackManager.BulletInfo>();
            while (info.Count > 0)
            {
                AttackManager.BulletInfo i = info[0];
                info.RemoveAt(0);
                result.Add(i);
                if (!i.isModifiable)
                {
                    continue;
                }

                int bulletsToFire = (int)(thisUpgradeAttribute.amount + 1);
                float bulletAngle = 90f / bulletsToFire;
                for (int j = 0; j < bulletsToFire - 1; j++)
                {
                    AttackManager.BulletInfo toSpawn = i with { };
                    toSpawn.bulletRotation = (Quaternion.Euler(toSpawn.bulletRotation) * Quaternion.Euler(0, 0, bulletAngle * (j + 1))).eulerAngles;
                    toSpawn.isModifiable = false;
                    toSpawn.isValid = true;
                    result.Add(toSpawn);

                    toSpawn = i with { };
                    toSpawn.bulletRotation = (Quaternion.Euler(toSpawn.bulletRotation) * Quaternion.Euler(0, 0, -bulletAngle * (j + 1))).eulerAngles;
                    toSpawn.isModifiable = false;
                    toSpawn.isValid = true;
                    result.Add(toSpawn);
                }
            }

            info.AddRange(result);
        }
    }
};

/*
 *         private class bulletMovement
        {
            float rotAmount;
            float maxAmount = 15;

            public bulletMovement(float amt)
            {
                rotAmount = Mathf.Clamp(amt, 0f, maxAmount);
            }
            public void MovementModifier(GameObject bullet)
            {
                bullet.transform.rotation *= Quaternion.Euler(0, 0, rotAmount * Time.deltaTime);
            }
        }
                    toSpawn.fn += new bulletMovement(j * 5).MovementModifier;
 * 
 * 
 * 
 * 
 * 
 */
