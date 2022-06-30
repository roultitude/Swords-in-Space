using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class NeedleShooter : EnemyShooter
    {
        public Quaternion shot1RotationOffset;
        public Quaternion shot2RotationOffset;
        public override void Shoot()
        {

            SpawnLocalRotation(shot1RotationOffset);
            SpawnLocalRotation(shot2RotationOffset);



        }


    }

};