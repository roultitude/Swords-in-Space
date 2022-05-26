using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class WeaponStarter : Weapon
    {
        public override void Shoot()
        {
            foreach (Shooter comp in shooters)
            {
                comp.Fire();
            }
        }
    }
};
