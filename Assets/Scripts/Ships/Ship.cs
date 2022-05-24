using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class Ship : NetworkBehaviour
    {
        public static Ship currentShip;

        private void Awake()
        {
            currentShip = this;
        }
    }

}
