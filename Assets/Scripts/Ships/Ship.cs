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
        public ShipMover shipMover;
        public Transform shipExterior;
        public Transform shipInterior;
        public Transform spawnTransform;

        private void Awake()
        {
            currentShip = this;
            shipMover = this.GetComponentInChildren<ShipMover>();
        }
    }

}
