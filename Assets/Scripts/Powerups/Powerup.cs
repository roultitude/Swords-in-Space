using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public abstract class Powerup : NetworkBehaviour
    {
        public abstract void OnPowerup();

    }

};