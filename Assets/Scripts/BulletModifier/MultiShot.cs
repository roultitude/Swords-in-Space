using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SwordsInSpace
{
    public class MultiShot : BulletModifier
    {
        //private Vector2 bulletScale = new Vector2(1, 1);
        public MultiShot()
        {
            thisUpgradeAttribute = new UpgradeSO.UpgradeAttributes();
            thisUpgradeAttribute.amount = 0;
            thisUpgradeAttribute.type = UpgradeSO.UpgradeTypes.multiShot;
        }

        private float SpeedScale(int amt)
        {
            return 1f / (1f + 0.07f * amt);
        }
        public override void Apply(List<AttackManager.BulletInfo> info)
        {

            List<AttackManager.BulletInfo> result = new List<AttackManager.BulletInfo>();
            while (info.Count > 0)
            {
                AttackManager.BulletInfo i = info[0];
                info.RemoveAt(0);

                if (!i.isModifiable)
                {
                    result.Add(i);
                    continue;
                }


                int bulletsToFire = (int)(thisUpgradeAttribute.amount * 2 + 1);

                for (int j = 0; j < (bulletsToFire / 2); j++)
                {
                    float offset = bulletsToFire % 2 == 0 ? 0.5f : 1f;

                    AttackManager.BulletInfo toSpawn = i with { };
                    toSpawn.bulletPosition += (Quaternion.Euler(toSpawn.bulletRotation) * Quaternion.Euler(0, 0, 90)) * Vector3.right * ((j + 1) * offset);
                    toSpawn.isModifiable = false;
                    toSpawn.isValid = true;
                    toSpawn.bulletScale = toSpawn.bulletScale / 2;
                    toSpawn.bulletDamage = toSpawn.bulletDamage / 4;
                    toSpawn.bulletShotSpeed = (toSpawn.bulletShotSpeed * SpeedScale(j + 1));
                    result.Add(toSpawn);

                    toSpawn = i with { };
                    toSpawn.bulletPosition -= (Quaternion.Euler(toSpawn.bulletRotation) * Quaternion.Euler(0, 0, 90)) * Vector3.right * ((j + 1) * offset);
                    toSpawn.isModifiable = false;
                    toSpawn.isValid = true;
                    toSpawn.bulletScale = toSpawn.bulletScale / 2;
                    toSpawn.bulletDamage = toSpawn.bulletDamage / 4;

                    toSpawn.bulletShotSpeed = (toSpawn.bulletShotSpeed * SpeedScale(j + 1));
                    result.Add(toSpawn);
                }


                if (bulletsToFire % 2 == 0)
                {
                    i.isValid = false;
                }

                result.Add(i);
            }

            info.AddRange(result);

        }

    }
};
