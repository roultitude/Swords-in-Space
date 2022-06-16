using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

namespace SwordsInSpace
{
    public class PlayerTracker : NetworkBehaviour
    {
        [SerializeField]
        private Transform interiorTransform;
        private Transform realPlayer;

        private void Update()
        {
            if (IsServerOnly) return;
            if (User.localUser)
            {
                if (User.localUser.controlledPlayer) realPlayer = User.localUser.controlledPlayer.transform;
            }
            if(realPlayer) transform.localPosition
                        = new Vector2(
                            Mathf.Lerp(transform.localPosition.x, interiorTransform.InverseTransformPoint(realPlayer.position).x, 0.15f),
                            Mathf.Lerp(transform.localPosition.y, interiorTransform.InverseTransformPoint(realPlayer.position).y, 0.15f)
                            );
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
        }
    }
}