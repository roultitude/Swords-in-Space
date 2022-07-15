using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SwordsInSpace
{
    public class MultiShot : BulletModifier
    {
        public MultiShot()
        {
            thisUpgradeAttribute = new UpgradeSO.UpgradeAttributes();
            thisUpgradeAttribute.amount = 1;
            thisUpgradeAttribute.type = UpgradeSO.UpgradeTypes.multiShot;
        }

        public override void Apply(List<AttackManager.BulletInfo> info)
        {
            if (thisUpgradeAttribute.amount == 1)
            {
                return;
            }

            List<AttackManager.BulletInfo> result = new List<AttackManager.BulletInfo>();
            while (info.Count > 0)
            {
                AttackManager.BulletInfo i = info[0];
                info.RemoveAt(0);


                for (int j = 0; j < (int)(thisUpgradeAttribute.amount / 2); j++)
                {
                    float offset = thisUpgradeAttribute.amount % 2 == 0 ? 0.5f : 1f;

                    AttackManager.BulletInfo toSpawn = i with { };
                    toSpawn.bulletPosition += (Quaternion.Euler(toSpawn.bulletRotation) * Quaternion.Euler(0, 0, 90)) * Vector3.right * (j + offset);
                    result.Add(toSpawn);

                    toSpawn = i with { };
                    toSpawn.bulletPosition -= (Quaternion.Euler(toSpawn.bulletRotation) * Quaternion.Euler(0, 0, 90)) * Vector3.right * (j + offset);
                    result.Add(toSpawn);
                }


                if (thisUpgradeAttribute.amount % 2 == 1)
                    result.Add(i);



            }

            info.AddRange(result);

        }

    }
};
