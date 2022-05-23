using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class Player : NetworkBehaviour
    {
        [SerializeField]
        private Transform spawnTransform;

        [SyncVar]
        public User controllingUser;

        [SyncVar]
        public float health;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;
            controllingUser = User.localUser;
            CameraManager.instance.AttachToPlayer(transform);
        }
    } 
}
