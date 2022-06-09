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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        [SerializeField]
        public int Hp;

        [SerializeField]
        public int MaxHp;

        public abstract void Interact(GameObject player);







    }
};
