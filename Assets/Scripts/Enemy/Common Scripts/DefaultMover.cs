using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class DefaultMover : EnemyMover
    {

        public override void OnDisableEnemyMover()
        {
            StopAstar();
        }

        public override void OnEnableEnemyMover()
        {
            ContinueAstar();
        }

    }
};
