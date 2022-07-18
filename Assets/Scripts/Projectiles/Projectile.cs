using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;


namespace SwordsInSpace
{
    public abstract class Projectile : NetworkBehaviour
    {
        public double damage;
        public double shotLifeTime;
        public abstract void Setup(float shotSpeed, double shotLifeTime, double damage, double spread, int pierce);

    }
};