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
        public Vector2 turnAxis = new Vector2(0,0);

        [SerializeField]
        private float turnSpeed;
        // Start is called before the first frame update
        private float pos = 0f;
        

        private void Update()
        {
            if (!IsServer) return;
            pos += turnAxis.x * turnSpeed * Time.deltaTime;
            Reposition(pos);

            var rot = Quaternion.LookRotation(Vector3.forward, gameObject.transform.localPosition);
            gameObject.transform.localRotation = Quaternion.RotateTowards(gameObject.transform.rotation, rot, 360f);

        }




        public void Reposition(float pos)
        {
            transform.localPosition = new Vector2(size.x * (float)Math.Sin(pos), size.y * (float)Math.Cos(pos));
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
