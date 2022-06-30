using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class FacePlayerOnPathEndMover : EnemyMover
    {
        // Start is called before the first frame update

        public float rotationSpeed = 90f;
        private void Update()
        {
            if (ai.reachedDestination)
            {
                LookAt(Ship.currentShip.transform, rotationSpeed);
            }
        }
    }
};