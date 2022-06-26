using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{
    public class Fire : Interactable
    {

        private int flameHp = 3;
        public Fire Up;
        public Fire Down;
        public Fire Left;
        public Fire Right;

        public bool fireActive = false;
        public bool trigger;
        public void Start()
        {

            SetActiveAllChildren(false);
        }

        public void Update()
        {
            if (trigger)
            {
                trigger = false;
                activate();
            }
        }
        public override void Interact(GameObject player)
        {
            Damage();
        }

        [ServerRpc(RequireOwnership = false)]
        private void Damage()
        {
            flameHp -= 1;
            if (flameHp < 0)
            {
                deactivate();
            }
        }
        [ObserversRpc]
        public void activate()
        {
            flameHp = 3;
            fireActive = true;
            SetActiveAllChildren(true);
        }

        [ObserversRpc]
        public void deactivate()
        {
            fireActive = false;
            SetActiveAllChildren(false);
        }


        private void SetActiveAllChildren(bool value)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(value);
            }
        }
    }
};
