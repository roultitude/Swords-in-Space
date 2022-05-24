using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class WeaponStarter : Weapon
    {
        public override void Shoot()
        {
            Debug.Log("HELLOR SIR");
            foreach (Shooter comp in shooters)
            {
                Debug.Log("WAKE UP");
                comp.Fire();
            }
        }
    }
};
