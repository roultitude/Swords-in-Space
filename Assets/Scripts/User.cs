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
            if (IsOwner) localUser = this;
            ServerSpawnPlayer();
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            UserManager.instance.users.Add(this);
            ServerSpawnPlayer();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            UserManager.instance.users.Remove(this);

        }

        [ServerRpc]
        public void ServerSpawnPlayer()
        {
            GameObject player = Instantiate(UserManager.instance.playerPrefab, UserManager.instance.spawnTransform.position, Quaternion.identity);

            Spawn(player, Owner);
            
        }
    }
}