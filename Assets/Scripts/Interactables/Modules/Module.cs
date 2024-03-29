
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

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


        [SyncVar(OnChange = nameof(onChangeOccupied))]
        protected bool isOccupied = false;
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

            EnableInteractZone(false);
        }

        public void OnFireExtinguishedNearby()
        {

            foreach (Fire f in linkedFireCells)
            {
                if (f.fireActive)
                    return;
            }


            EnableInteractZone(true);

        }
        public override void Interact(GameObject player)
        {
            Debug.Log(name + " is occupied: " + isOccupied);
            if (!isOccupied) OnInteract(player);
        }
        public abstract void OnInteract(GameObject player);

        [ObserversRpc]
        private void EnableInteractZone(bool value)
        {
            interactZone.enabled = value;
            collideZone.enabled = value;

        }

        [ServerRpc(RequireOwnership = false)]
        protected void SetOccupied(bool occupied)
        {
            isOccupied = occupied;
        }

        void onChangeOccupied(bool oldOccupied, bool newOccupied, bool isServer)
        {
            switchAnim();
        }

        protected override void switchAnim()
        {
            if (numPlayersNearby > 0)
            {
                if (isOccupied)
                {
                    anim.CrossFade("PlayerOccupied", 0, 0);
                }
                else anim.CrossFade("PlayerNearby", 0, 0);
            }
            else
            {
                anim.CrossFade("Idle", 0, 0);
            }
        }


    }
};


