using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class GrabberShooter : Shooter
    {
        [SerializeField] SpriteRenderer grappleSprite;

        [ObserversRpc]
        public void UpdateSprite(bool shot)
        {
            grappleSprite.enabled = shot;
        }
    }

}
