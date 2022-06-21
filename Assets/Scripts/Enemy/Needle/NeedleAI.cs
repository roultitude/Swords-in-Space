using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class NeedleAI : EnemyAI
    {
        // Start is called before the first frame update

        public NeedleMover mover;
        public NeedleShooter shooter;


        public void onStartStopDash()
        {

            shooter.canFire = !shooter.canFire;
            Debug.Log(shooter.canFire);
        }
        public void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log(collision.gameObject);
            Ship obj = collision.gameObject.GetComponent<Ship>();

            if (obj != null)
            {
                Ship.currentShip.TakeDamage(mover.dashImpulse / 10);
                mover.OnTouchPlayer();
            }

        }


    }
};