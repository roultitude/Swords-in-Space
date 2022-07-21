using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class DefaultMover : EnemyMover
    {

        public override void OnSleepEnemyMover()
        {
            StopAstar();
        }

        public override void OnAwakeEnemyMover()
        {
            ContinueAstar();
        }

    }
};
