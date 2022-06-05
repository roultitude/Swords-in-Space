using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
namespace SwordsInSpace
{
    public class Asteroid : NetworkBehaviour
    {
        // Start is called before the first frame update

        float shotSpeed = 0f;
        [SyncVar]
        public int hp = 10;

        public void Setup(float shotSpeed)
        {
            this.shotSpeed = shotSpeed;
        }


        // Update is called once per frame
        void Update()
        {
            transform.position += transform.right * Time.deltaTime * shotSpeed;
        }

        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (!IsServer)
                return;
            hp -= 1;
            Bullet bullet = coll.gameObject.GetComponentInParent<Bullet>();
            if (bullet != null)
                bullet.OnHit();

            if (hp <= 0)
                this.Despawn();
        }
    }

};