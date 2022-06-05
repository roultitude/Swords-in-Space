using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
namespace SwordsInSpace
{
    public class Bullet : NetworkBehaviour
    {
        Timer timer;
        double shotSpeed;
        double shotLifeTime;

        public void Setup(double shotSpeed, double shotLifeTime)
        {
            timer = gameObject.AddComponent<Timer>();
            this.shotLifeTime = shotLifeTime;
            this.shotSpeed = shotSpeed;
            timer.Setup(shotLifeTime, false, true);
            timer.timeout.AddListener(OnTimeout);

        }
        public void OnHit()
        {
            if (IsServer)
                Despawn();
        }

        public void OnTimeout()
        {
            if (IsServer)
                Despawn();
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsServer) { return; }
            transform.position += transform.right * Time.deltaTime * (float)shotSpeed;

        }


    }
};
