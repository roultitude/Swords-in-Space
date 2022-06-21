using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SwordsInSpace
{
    public class UserManager : NetworkBehaviour
    {
        public static UserManager instance;

        public GameObject playerPrefab;

        public UserData localUserData;


        [SyncObject]
        public readonly SyncList<User> users = new SyncList<User>();

        [SyncVar]
        public bool allUsersReady;

        private void Awake()
        {

            if (instance)
            {
                Debug.Log("there exists a UserManager already!"); //destroy newest
                Destroy(this);
            }
            else instance = this;
        }

        private void Update()
        {
            if (!IsServer) return;
            allUsersReady = users.All(user => user.isReady);
        }

    }
}