using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class FishAI : EnemyAI
    {
        [SerializeField]
        private float contactDamage, reboundForce;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Ship obj = collision.gameObject.GetComponent<Ship>();

            if (obj != null)
            {
                Ship.currentShip.TakeDamage(contactDamage);
                rb.AddForce(collision.GetContact(0).normal * reboundForce,ForceMode2D.Impulse); //fish bounces off ship upon collision
            }

        }
    }
}