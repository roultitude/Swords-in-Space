using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

namespace SwordsInSpace
{
    public class PlayerTracker : NetworkBehaviour
    {

        private void Update()
        {
            if (IsServerOnly) return;
            if (User.localUser)
            {
                if (User.localUser.controlledPlayer) transform.localPosition = User.localUser.controlledPlayer.transform.localPosition;
            }
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
        }
    }
}