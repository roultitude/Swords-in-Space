
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

namespace SwordsInSpace
{
    public abstract class Module : Interactable
    {
        /*
            Modules are unable to move and are spawned in at predefined locations.
        */

        [SerializeField]
        public Fire[] linkedFireCells;

        public Collider2D interactZone;
        public Collider2D collideZone;
        public void Start()
        {
            if (!IsServer)
                return;
            if (linkedFireCells.Length > 0)
            {
                foreach (Fire f in linkedFireCells)
                {
                    f.onStartFire.AddListener(() => { OnFireNearby(); });
                    f.onEndFire.AddListener(() => { OnFireExtinguishedNearby(); });
                    //Debug.Log("nice");
                }
            }
        }


        public void OnFireNearby()
        {

            DisableInteractZone(false);
        }

        public void OnFireExtinguishedNearby()
        {

            foreach (Fire f in linkedFireCells)
            {
                if (f.fireActive)
                    return;
            }


            DisableInteractZone(true);

        }

        [ObserversRpc]
        private void DisableInteractZone(bool value)
        {
            interactZone.enabled = value;
            collideZone.enabled = value;

        }


    }
};


