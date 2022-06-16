using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class UserManager : NetworkBehaviour
    {
        public static UserManager instance;

        public GameObject playerPrefab;

        public UserData localUserData;

        [SyncObject]
        public readonly SyncList<User> users = new SyncList<User>();


        private void Awake()
        {

            if (instance)
            {
                Debug.Log("there exists a UserManager already!");
                Destroy(this);
            }
            else instance = this;
        }

        private void Update()
        {
        }


    }
}