using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SwordsInSpace
{
    public class PlayerTracker : MonoBehaviour
    {
        private void Update()
        {
            if(User.localUser) transform.localPosition = User.localUser.controlledPlayer.transform.localPosition;
        }
    }
}