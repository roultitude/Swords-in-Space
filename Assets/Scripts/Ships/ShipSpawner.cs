using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

namespace SwordsInSpace
{
    public class ShipSpawner : NetworkBehaviour
    {
        public static GameObject shipPrefab;

        public GameObject defaultShipPrefab;

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (!InstanceFinder.IsServer) return;
            if (Ship.currentShip)
            {
                Destroy(this);
                return;
            }
            else
            {
                if (!shipPrefab) shipPrefab = defaultShipPrefab;
                GameObject shipObject = Instantiate(shipPrefab);
                Spawn(shipObject);
                CameraManager.instance.SetupShipCams();
                Destroy(this);
            }
        }
    }

}
