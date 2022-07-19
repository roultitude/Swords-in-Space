using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class HpPowerup : Powerup
    {
        public override void OnPowerup()
        {
            //May want to scale powerups with world level

            Ship.currentShip.AddHp(WorldSpawner.level * 25);
        }
    }
};