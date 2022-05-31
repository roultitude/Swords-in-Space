using FishNet.Component.Prediction;
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
            SetParent();
            //GetComponent<PlayerInputManager>().enabled = IsOwner;

            if (!IsOwner)
            {
                return;
            }
            controllingUser = User.localUser;
            controllingUser.controlledPlayer = this;
            CameraManager.instance.AttachToPlayer(transform);
        }

        public void SetParent()
        {
            this.transform.parent = Ship.currentShip.spawnTransform;
        }
    } 
}
