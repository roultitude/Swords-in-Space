using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Scened;
using FishNet.Connection;
using UnityEngine.Tilemaps;


namespace SwordsInSpace
{
    public class FireManager : NetworkBehaviour
    {

        public Fire[] fires;
        public Coroutine fireTracker;

        public float tickTime;
        public float tickChance;
        public float fireStartOnCollideChance;
        private bool fireActive = false;

        public void StartFire()
        {
            Debug.Log("Starting a fire");
            if (fires == null || fires.Length == 0)
                return;

            fires[Random.Range(0, fires.Length)].activate();

            if (fireTracker == null || !fireActive)
                fireTracker = StartCoroutine("TrackFire");

        }

        public IEnumerator TrackFire()
        {
            while (countFires() > 0)
            {
                fireActive = true;
                foreach (Fire f in fires)
                {
                    if (f.fireActive && Random.Range(0f, 1f) < tickChance)
                    {
                        switch (Random.Range(1, 5))
                        {
                            case 1:
                                if (f.Up != null && !f.Up.fireActive)
                                    f.Up.activate();
                                break;
                            case 2:
                                if (f.Down != null && !f.Down.fireActive)
                                    f.Down.activate();
                                break;
                            case 3:
                                if (f.Left != null && !f.Left.fireActive)
                                    f.Left.activate();
                                break;
                            case 4:
                                if (f.Right != null && !f.Right.fireActive)
                                    f.Right.activate();
                                break;

                        }
                    }
                }
                yield return new WaitForSeconds(tickTime);
            }
            fireActive = false;
            yield break;
        }

        private int countFires()
        {
            int i = 0;
            foreach (Fire f in fires)
            {
                if (f.gameObject.activeInHierarchy)
                {
                    i += 1;
                }
            }
            return i;
        }

        public void FireEventTrigger()
        {

            if (IsServer && Random.Range(0f, 1f) < fireStartOnCollideChance)
                StartFire();
        }
    }
};
