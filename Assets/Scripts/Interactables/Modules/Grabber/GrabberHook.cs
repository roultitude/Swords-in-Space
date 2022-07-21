using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class GrabberHook : NetworkBehaviour
    {

        private float forwardSpeed = 0;
        private float retractSpeed = 0;
        private Transform launcherTransform;
        private float despawnThreshold = 0.5f;
        bool isRetracting;
        public void Setup(float forwardSpeed, float retractSpeed, float forwardTime, Transform launcherTransform)
        {
            this.forwardSpeed = forwardSpeed;
            this.retractSpeed = retractSpeed;
            this.launcherTransform = launcherTransform;
            StartCoroutine(ForwardTracker(forwardTime));

        }
        private void Update()
        {
            if (!IsServer) { return; }
            if (!isRetracting) transform.position += transform.right * Time.deltaTime * forwardSpeed;
            else
            {
                transform.position -= (transform.position - launcherTransform.position).normalized * Time.deltaTime * retractSpeed;
                if (Vector2.Distance(transform.position, launcherTransform.position) < despawnThreshold) Despawn();
            }
        }

        private IEnumerator ForwardTracker(float forwardTime)
        {
            yield return new WaitForSeconds(forwardTime);
            isRetracting = true;
        }

        
    }
}

