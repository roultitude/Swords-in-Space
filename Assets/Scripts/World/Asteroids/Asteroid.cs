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


        [SyncVar]
        public double hp = 10;


        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (!IsServer)
                return;
            hp -= 1;
            Bullet bullet = coll.gameObject.GetComponentInParent<Bullet>();
            if (bullet != null)
                bullet.OnHit();
            hp -= bullet.damage;
            if (hp <= 0)
                this.Despawn();
        }


        public void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log(collision.gameObject);
            Ship obj = collision.gameObject.GetComponent<Ship>();

            if (obj != null)
            {

                Ship.currentShip.TakeDamage(Mathf.Clamp(Ship.currentShip.shipMover.rb.velocity.magnitude, 0, 7.5f));

            }

        }


    }

};