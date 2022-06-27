using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using UnityEngine.Events;

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

        public UnityEvent onStartFire = new();
        public UnityEvent onEndFire = new();
        public void Start()
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
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
            onStartFire.Invoke();
        }

        [ObserversRpc]
        public void deactivate()
        {
            fireActive = false;
            SetActiveAllChildren(false);
            onEndFire.Invoke();
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
