using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class FiveFishesAI : EnemyAI
    {
        [SerializeField]
        private float turnSpeed;



        new void Start()
        {
            base.Start();
            //gameObject.GetComponent<Pathfinding.AIDestinationSetter>().target = Ship.currentShip.transform;

        }

        // Update is called once per frame
        void Update()
        {
            rb.AddTorque(turnSpeed * Time.deltaTime);
        }
    }
}
