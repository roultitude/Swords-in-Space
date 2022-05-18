using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class InputManager : NetworkBehaviour
{
    private List<Interactable> interactables;
    private static Dictionary<NetworkConnection, List<Interactable>> data;

    // Start is called before the first frame update
    void Start()
    {
        if (base.IsServer && data == null)
        {
            data = new Dictionary<NetworkConnection, List<Interactable>>();

        }

        if (base.IsServer)
        {
            interactables = new List<Interactable>();
            data.Add(base.Owner, interactables);
        }

        
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
        Debug.Log("??");
        ReplyInteractQuery(conn);
    }

    [TargetRpc]
    void ReplyInteractQuery(NetworkConnection conn)
    {
        Debug.Log("Donkey");
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!base.IsServer) { return; }


        Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();
        if (otherInteractable)
        {
            interactables.Add(otherInteractable);
        }


    }



    private void OnTriggerExit2D(Collider2D other)
    {
        if (!base.IsServer) { return; }


        Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();

        if (otherInteractable)
        {
            interactables.Remove(otherInteractable);
        }

    }


}
