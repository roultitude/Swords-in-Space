using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{
    public class PlayerInteractionManager : NetworkBehaviour
    {
        public List<int> data;

        [SerializeField]
        private InteractableIdManager interactables;
        private UserManager userManager;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            GameManager.OnNewSceneLoadEvent += () =>
            {

                interactables = Ship.currentShip.shipInterior.GetComponent<InteractableIdManager>();
            };
        }
        // Start is called before the first frame update
        void Start()
        {
            userManager = UserManager.instance;
            if (base.IsServer && data == null)
            {
                data = new List<int>();

            }
            interactables = GameObject.FindObjectOfType<InteractableIdManager>();
        }

        public void Interact()
        {
            if (!base.IsOwner) { return; }


            InteractQuery(base.Owner);


        }

        private string GetUsernameFromConnection(NetworkConnection conn)
        {
            foreach (User user in userManager.users)
            {
                if (user.Owner.ClientId == conn.ClientId)
                    return user.username;
            }

            return null;
        }

        private string GetUsernameFromId(int id)
        {

            foreach (User user in userManager.users)
            {

                if (user.Owner.ClientId == id)
                {
                    return user.username;
                }

            }

            return null;
        }

        [ServerRpc]
        void InteractQuery(NetworkConnection conn = null)
        {

            if (data.Count > 0)
            {
                foreach (int a in data)
                {
                    Interactable obj = interactables.GetInteractable(a);

                    if (obj)
                    {
                        if (Ship.currentShip.isPowerUp || obj.canUseOnPowerOut)
                            ReplyInteractQuery(conn, a);
                    }

                }

            }

        }

        [TargetRpc]
        void ReplyInteractQuery(NetworkConnection conn, int interactableId)
        {

            Interactable obj = interactables.GetInteractable(interactableId);
            //Debug.Log(obj.transform);
            if (obj)
            {
                if (Ship.currentShip.isPowerUp || obj.canUseOnPowerOut)
                    obj.Interact(gameObject);
            }

        }



        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!base.IsServer) { return; }
            Debug.Log(other.gameObject);
            Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();

            if (otherInteractable)
            {
                int id = interactables.GetId(otherInteractable);
                if (id != -1) 
                {
                    data.Add(interactables.GetId(otherInteractable));
                    otherInteractable.numPlayersNearby++;
                } 
            }


        }



        private void OnTriggerExit2D(Collider2D other)
        {


            if (!base.IsServer) { return; }


            Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();

            if (otherInteractable)
            {
                data.Remove(interactables.GetId(otherInteractable));
                TargetExitUi(Owner);
                otherInteractable.numPlayersNearby--;
            }



        }


        [TargetRpc]
        public void TargetExitUi(NetworkConnection conn)
        {

            GetComponent<PlayerInputManager>().ExitUI();
        }


    }
};
