using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class NeedleAI : EnemyAI
    {
        // Start is called before the first frame update
        [SerializeField]
        NeedleMover mover;



        public void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log(collision.gameObject);
            Ship obj = collision.gameObject.GetComponent<Ship>();

            if (obj != null)
            {
                Ship.currentShip.TakeDamage(mover.dashSpeed / 10);
                mover.OnTouchPlayer();
            }

        }


    }
};
