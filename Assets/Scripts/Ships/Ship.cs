using FishNet.Connection;
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
        public Transform shipInteriorView;
        public Transform spawnTransform;
        public Transform playerTracker;
        private void Awake()
        {
            currentShip = this;
            shipMover = this.GetComponentInChildren<ShipMover>();
        }

        [ServerRpc(RequireOwnership =false)]
        public void changePilot(NetworkConnection conn = null)
        {
            if (Owner.IsActive) return; //serverside check for ownership
            base.GiveOwnership(conn);
        }
        [ServerRpc(RequireOwnership = false)]
        public void leavePilot(NetworkConnection conn = null)
        {
            if(Owner == conn)
            {
                base.RemoveOwnership();
            }
        }
    }

}
