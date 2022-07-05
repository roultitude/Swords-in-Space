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

        [SyncVar(OnChange = nameof(OnPlayerSpawned))]
        public Player controlledPlayer;

        [SyncVar]
        public bool isReady;


        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
            {
                localUser = this;
                UpdateUsername(UserManager.instance.localUserData.username);
            }
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            UserManager.instance.users.Add(this);
            if (!controlledPlayer)
            {
                ServerSpawnPlayer();
            }   
        }

        private void OnEnable()
        {
            GameManager.OnNewSceneLoadEvent += OnNewSceneLoaded;
        }
        private void OnDisable()
        {
            GameManager.OnNewSceneLoadEvent -= OnNewSceneLoaded;
        }

        public void OnPlayerSpawned(Player oldPlayer, Player newPlayer, bool onServer)
        {
            Debug.Log(newPlayer);
            if (!newPlayer) return;
            controlledPlayer.SetupPlayer();
        }
        private void OnNewSceneLoaded()
        {
            if (!controlledPlayer)
            {
                ServerSpawnPlayer();
            }
            else
            {
                controlledPlayer.SetupPlayer();
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            UserManager.instance.users.Remove(this);
        }

        [ServerRpc(RequireOwnership =false)]
        public void ServerSpawnPlayer()
        {
            if (controlledPlayer) return; //only one player at a time
            GameObject player = Instantiate(UserManager.instance.playerPrefab, Ship.currentShip.spawnTransform.position, Quaternion.identity);
            controlledPlayer = player.GetComponent<Player>();
            controlledPlayer.controllingUser = this;
            Spawn(player, Owner);
        }

        [ServerRpc]
        public void UpdateUsername(string name)
        {
            username = name;
            GameManager.instance.MessageRPC(username + " has joined the game!");
        }

        [ServerRpc]
        public void ServerSetIsReady(bool ready)
        {
            isReady = ready;
        }

    }
}