using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class NitroBoost : Powerup
    {
        public override void OnPowerup()
        {
            Ship.currentShip.ChangeNitroFuel(1);
        }


    }
};