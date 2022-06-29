using FishNet.Object;
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

        [SerializeField]
        public bool canUseOnPowerOut = false;

        public abstract void Interact(GameObject player);







    }
};
