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

        [SyncVar(OnChange = nameof(onChange))]
        public bool fireActive = false;


        public UnityEvent onStartFire = new();
        public UnityEvent onEndFire = new();



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

        [ServerRpc(RequireOwnership = false)]
        public void activate()
        {
            fireActive = true;
        }


        public void deactivate()
        {
            fireActive = false;
        }


        public void onChange(bool past, bool current, bool isServer)
        {
            Debug.Log(current);
            if (current)
            {
                flameHp = 3;
                SetActiveAllChildren(true);
                onStartFire.Invoke();
            }
            else
            {
                SetActiveAllChildren(false);
                onEndFire.Invoke();
            }



        }


        private void SetActiveAllChildren(bool value)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(value);
                if (value)
                    gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
        }
    }
};
