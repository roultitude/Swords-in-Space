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
            if (id < data.Count)
            {
                return data[id];
            }
            return null;
        }

        public int GetId(Interactable inp)
        {
            if (data.Contains(inp))
            {
                return data.IndexOf(inp);
            }
            return -1;
        }
    }
};
