using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{
    public class Shielder : NetworkBehaviour
    {

        public Vector2 size = new Vector2(10, 10);

        // Start is called before the first frame update
        [SyncVar]
        private float pos = 0f;


        private void Update()
        {
            pos += 0.01f;
            Reposition(pos);

            var rot = Quaternion.LookRotation(gameObject.transform.position);
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, rot, 360f);

        }




        public void Reposition(float pos)
        {
            transform.position = new Vector2(size.x * (float)Math.Sin(pos), size.y * (float)Math.Cos(pos));
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            Bullet bullet = collision.gameObject.GetComponentInParent<Bullet>();
            //Debug.Log(bullet.gameObject.tag);
            if (bullet != null && (bullet.gameObject.tag == null || bullet.gameObject.tag != "Friendly"))
            {
                bullet.OnHit();
            }
        }
    }
};
