using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class User : NetworkBehaviour
    {
        public static User localUser;

        [SyncVar]
        public string username;

        [SyncVar]
        public Player controlledPlayer;




        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
            {
                localUser = this;
                UpdateUsername(UserManager.instance.localUserData.username);
                ServerSpawnPlayer();
                
            }
            
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            UserManager.instance.users.Add(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            UserManager.instance.users.Remove(this);
        }

        [ServerRpc]
        public void ServerSpawnPlayer()
        {
            GameObject player = Instantiate(UserManager.instance.playerPrefab, Ship.currentShip.spawnTransform.position, Quaternion.identity);
            controlledPlayer = player.GetComponent<Player>();
            controlledPlayer.controllingUser = this;
            Spawn(player, Owner);
        }

        [ServerRpc]
        public void UpdateUsername(string name)
        {
            username = name;
        }

    }
}