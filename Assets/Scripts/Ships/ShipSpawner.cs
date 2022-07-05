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

        public GameObject lobbyShipPrefab;

        public bool spawnLobbyShip;
        public override void OnStartServer()
        {
            base.OnStartServer();
            if (!InstanceFinder.IsServer) return;
            if (Ship.currentShip)
            {
                Ship.currentShip.transform.position = Vector2.zero;
                Destroy(this);
                return;
            }
            else
            {
                if (!shipPrefab) shipPrefab = defaultShipPrefab;
                GameObject shipObject;
                if (!spawnLobbyShip)
                {
                    shipObject = Instantiate(shipPrefab);
                }
                else
                {
                    shipObject = Instantiate(lobbyShipPrefab);
                }
                Spawn(shipObject);
                CameraManager.instance.SetupShipCams();
                Destroy(this.gameObject);
            }
        }
    }

}
