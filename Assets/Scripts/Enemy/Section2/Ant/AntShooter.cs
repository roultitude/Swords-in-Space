using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class AntShooter : EnemyShooter
    {
        public GameObject[] turrets;

        public override void Shoot()
        {
            foreach (GameObject obj in turrets)
            {
                SpawnLocalRotation(offset: Quaternion.Euler(0, 0, -90), loc: obj.transform.position);
            }

        }
    }
};
