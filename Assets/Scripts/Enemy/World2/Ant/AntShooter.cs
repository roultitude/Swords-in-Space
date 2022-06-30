using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class AntShooter : EnemyShooter
    {
        public Quaternion RotationOffset;
        public GameObject[] turrets;

        public override void Shoot()
        {
            foreach (GameObject obj in turrets)
            {
                SpawnLocalRotation(RotationOffset, obj.transform.position);
            }

        }
    }
};
