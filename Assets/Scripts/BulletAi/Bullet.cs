using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
namespace SwordsInSpace
{
    public class Bullet : NetworkBehaviour
    {
        public void OnHit()
        {
            Debug.Log("boom");
        }

        public void OnTimeout()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!IsServer) { return; }
            transform.position += transform.right * Time.deltaTime * 10;

        }


    }
};
