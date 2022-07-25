using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class AttackIndicator : NetworkBehaviour
    {
        public float time;

        public delegate void OnTimerEndDelegate(GameObject obj);
        public OnTimerEndDelegate OnTimerEnd;


        public void Setup(float time, OnTimerEndDelegate fn)
        {
            if (!IsServer) return;

            this.time = time;
            StartCoroutine(StartTimer());
            OnTimerEnd += fn;
        }

        private IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(time);
            OnTimerEnd?.Invoke(gameObject);
            Despawn();
        }

    }
};
