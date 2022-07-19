using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public abstract class Interactable : NetworkBehaviour
    {
        /*
            Interactables are objects in the game that can be interacted by the User via 
            interact button (Pick up / Use).
            
            They do not change/interact when the User enters / leaves its collider.
        */

        [SyncVar(OnChange = nameof(OnNumPlayersNearbyChanged))]
        public int numPlayersNearby;

        [SerializeField]
        public bool canUseOnPowerOut = false;
        [SerializeField]
        protected Animator anim;

        public abstract void Interact(GameObject player);

        protected virtual void OnNumPlayersNearbyChanged(int oldNumPlayer, int newNumPlayer, bool isServer)
        {
            switchAnim();
        }


        protected virtual void switchAnim()
        {
            if (!anim) return;
            if (numPlayersNearby > 0)
            {
                anim.CrossFade("PlayerNearby", 0, 0);
            }
            else
            {
                anim.CrossFade("Idle", 0, 0);
            }
        }




    }
};
