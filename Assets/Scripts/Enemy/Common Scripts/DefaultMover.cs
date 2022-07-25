using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class DefaultMover : EnemyMover
    {

        public override void OnSleepEnemyMover()
        {
            if (IsServer)
                StopAstar();
        }

        public override void OnAwakeEnemyMover()
        {
            if (IsServer)
                ContinueAstar();
        }

    }
};
