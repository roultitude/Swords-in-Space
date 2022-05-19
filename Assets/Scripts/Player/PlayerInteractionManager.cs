using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

namespace SwordsInSpace
{
    public class PlayerInteractionManager : NetworkBehaviour
    {

        private static Dictionary<int, List<int>> data;
        private InteractableIdManager interactables;

        // Start is called before the first frame update
        void Start()
        {
            if (base.IsServer && data == null)
            {
                data = new Dictionary<int, List<int>>();

            }

            if (base.IsServer)
            {
               
                data.Add(base.Owner.ClientId, new List<int>());
            }

            interactables = GameObject.FindObjectOfType<InteractableIdManager>();


        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("f"))
            {
                Interact();
            }

        }

        void Interact()
        {
            if (!base.IsOwner) { return; }
            InteractQuery(base.Owner);

        }

        [ServerRpc]
        void InteractQuery(NetworkConnection conn = null)
        {
            Debug.Log(conn.ClientId);
            ReplyInteractQuery(conn, data[conn.ClientId][0]);
        }

        [TargetRpc]
        void ReplyInteractQuery(NetworkConnection conn, int interactableId)
        {
            Debug.Log(interactables.GetInteractable(interactableId));
            interactables.GetInteractable(interactableId).Interact(gameObject);
        }



        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!base.IsServer) { return; }


            Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();
            
            if (otherInteractable)
            {
                data[base.Owner.ClientId].Add(interactables.GetId(otherInteractable));
            }


        }



        private void OnTriggerExit2D(Collider2D other)
        {
            if (!base.IsServer) { return; }


            Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();

            if (otherInteractable)
            {
                data[base.Owner.ClientId].Remove(interactables.GetId(otherInteractable));
            }

        }


    }
};
