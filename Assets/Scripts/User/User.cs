using FishNet.Connection;
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

        [SyncVar]
        public bool isReady;


        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log("OnStartClient fired");
            if (IsOwner)
            {
                localUser = this;
                UpdateUsername(UserManager.instance.localUserData.username);
            }
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log("OnStartServer fired for User " + OwnerId);
            UserManager.instance.users.Add(this);
            if (!controlledPlayer)
            {
                SpawnPlayer();
            }
        }

        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);
            Debug.Log("Spawned for client" + connection.ClientId);
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
            if (!newPlayer) return;
            controlledPlayer.SetupPlayer();
        }
        private void OnNewSceneLoaded()
        {
            Debug.Log(controlledPlayer);
            if (!controlledPlayer || !controlledPlayer.gameObject.activeSelf)
            {
                Debug.Log("Asking server to spawn player");
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

        [ServerRpc]
        public void ServerSpawnPlayer()
        {
            SpawnPlayer();
        }

        void SpawnPlayer()
        {
            if (controlledPlayer) return; //only one player at a time
            GameObject player = Instantiate(UserManager.instance.playerPrefab, Ship.currentShip.spawnTransform.position, Quaternion.identity);
            controlledPlayer = player.GetComponent<Player>();
            controlledPlayer.controllingUser = this;
            Spawn(player, Owner);
            
            Debug.Log("Completed Spawn Player");
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