using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

namespace SwordsInSpace
{
    public class PlayerTracker : NetworkBehaviour
    {
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
                            Mathf.Lerp(transform.localPosition.x,realPlayer.localPosition.x,0.03f),
                            Mathf.Lerp(transform.localPosition.y, realPlayer.localPosition.y, 0.03f)
                            );
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
        }
    }
}