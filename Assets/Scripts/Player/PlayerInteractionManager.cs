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
        public static Dictionary<string, List<int>> data;

        [SerializeField]
        private InteractableIdManager interactables;
        private UserManager userManager;

        private void Awake()
        {
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
                data = new Dictionary<string, List<int>>();

            }

            if (base.IsServer)
            {
                string username = GetUsernameFromConnection(Owner);
                Debug.Log("Welcome on board, " + username);
                if (!data.ContainsKey(username))
                    data.Add(username, new List<int>());
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
            string username = GetUsernameFromConnection(conn);
            if (data.ContainsKey(username)
                && data[username].Count >= 1
                && data[username][0] >= 0)
                ReplyInteractQuery(conn, data[username][0]);
        }

        [TargetRpc]
        void ReplyInteractQuery(NetworkConnection conn, int interactableId)
        {
            Interactable obj = interactables.GetInteractable(interactableId);
            Debug.Log(interactables);
            Debug.Log(obj);
            if (obj)
            {
               
                Debug.Log(Ship.currentShip.isPowerUp);
                if (Ship.currentShip.isPowerUp || obj.canUseOnPowerOut)
                    obj.Interact(gameObject);
            }

        }



        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!base.IsServer) { return; }


            Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();

            if (otherInteractable)
            {
                data[GetUsernameFromId(base.Owner.ClientId)].Add(interactables.GetId(otherInteractable));
            }


        }



        private void OnTriggerExit2D(Collider2D other)
        {
            if (!base.IsServer) { return; }


            Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();

            if (otherInteractable)
            {
                data[GetUsernameFromId(base.Owner.ClientId)].Remove(interactables.GetId(otherInteractable));
            }

        }


    }
};
