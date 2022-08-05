using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class EnemyAnimator : NetworkBehaviour
    {
        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        [ObserversRpc]
        public void CrossFadeObserver(string stateName)
        {
            anim.CrossFade(stateName, 0, 0);
        }

        
    }
}

