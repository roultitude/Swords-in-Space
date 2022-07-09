using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{
    public class InteractableIdManager : NetworkBehaviour
    {
        // Start is called before the first frame update
        [SyncObject]
        public readonly SyncList<Interactable> data = new SyncList<Interactable>();

        public override void OnStartServer()
        {
            base.OnStartServer();


            foreach (Interactable kid in GetComponentsInChildren<Interactable>())
            {
                data.Add(kid);
            }
        }

        public Interactable GetInteractable(int id)
        {
            Debug.Log("getting interactable : " + id);
            Debug.Log("getInteractable has : " + data.Count);
            if (id < data.Count)
            {
                return data[id];
            }
            return null;
        }

        public int GetId(Interactable inp)
        {
            Debug.Log("getID has : " + data.Count);
            if (data.Contains(inp))
            {
                return data.IndexOf(inp);
            }
            return -1;
        }
    }
};
