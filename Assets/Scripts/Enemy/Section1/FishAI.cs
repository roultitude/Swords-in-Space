using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class FishAI : EnemyAI
    {
        [SerializeField]
        private float contactDamage, reboundForce;


        private FishMover mover;
        private SpriteRenderer sprite;

        private void Awake()
        {

            mover = GetComponent<FishMover>();
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Ship obj = collision.gameObject.GetComponent<Ship>();

            if (obj != null)
            {
                Ship.currentShip.TakeDamage(contactDamage);
                rb.AddForce(collision.GetContact(0).normal * reboundForce, ForceMode2D.Impulse); //fish bounces off ship upon collision
                mover.OnStartRebound();
            }

        }

        private void Update()
        {
            //Debug.Log(transform.rotation.eulerAngles.z);
            sprite.flipY = transform.rotation.eulerAngles.z < 180;
            //Debug.Log(sprite.flipX);
        }
    }
}