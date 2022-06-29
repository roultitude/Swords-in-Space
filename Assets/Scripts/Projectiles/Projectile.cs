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
        public abstract void Setup(double shotSpeed, double shotLifeTime, double damage);

    }
};