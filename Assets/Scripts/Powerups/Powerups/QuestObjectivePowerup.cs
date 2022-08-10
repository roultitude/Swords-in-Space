using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{

    public class QuestObjectivePowerup : Powerup
    {
        public override void OnPowerup()
        {
            WorldManager.currentWorld.spawner.collectObjective();
        }
    }
};