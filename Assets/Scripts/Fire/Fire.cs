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

        public int flameHp = 5;

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
                Ship.currentShip.fireManager.fireExtinguished(gameObject.transform.localPosition);
                Despawn();
            }
        }


    }
};
