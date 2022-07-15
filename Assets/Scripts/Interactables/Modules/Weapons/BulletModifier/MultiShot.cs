using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SwordsInSpace
{
    public class MultiShot : BulletModifier
    {

        public override void Apply(List<AttackManager.BulletInfo> info)
        {
            List<AttackManager.BulletInfo> result = new List<AttackManager.BulletInfo>();
            while (info.Count > 0)
            {
                AttackManager.BulletInfo i = info[0];
                info.RemoveAt(0);

                AttackManager.BulletInfo toSpawn = i with { };
                Debug.Log(toSpawn.bulletPosition);
                toSpawn.bulletPosition += (Quaternion.Euler(toSpawn.bulletRotation) * Quaternion.Euler(0, 0, 90)) * Vector3.right;
                Debug.Log(toSpawn.bulletPosition);

                result.Add(i);
                result.Add(toSpawn);
                if (thisUpgradeAttribute.amount % 2 == 0)
                {
                    //Even number of bullets


                }
                else
                {
                    //Odd number of bullets

                }

            }

            info.AddRange(result);

        }

    }
};
