using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class FishMover : DefaultMover
    {
        [SerializeField]
        private float reboundPauseTime;


        public void OnStartRebound()
        {
            StopAllCoroutines();
            StartCoroutine(Rebound());
        }

        public IEnumerator Rebound()
        {
            StopAstar();
            yield return new WaitForSeconds(reboundPauseTime);
            ContinueAstar();
        }
    }
}

