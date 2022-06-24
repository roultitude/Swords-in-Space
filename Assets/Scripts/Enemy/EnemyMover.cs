using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

namespace SwordsInSpace
{
    public class EnemyMover : MonoBehaviour
    {
        protected AIPath ai;

        public bool debugCanSeePlayer = false;

        public void Start()
        {
            ai = gameObject.GetComponent<AIPath>();
        }

        public void Update()
        {
            if (debugCanSeePlayer)
            {
                CanSeePlayer();
                debugCanSeePlayer = false;
            }
        }

        protected void StopAstar()
        {
            ai.canMove = false;
            ai.isStopped = true;

        }

        protected void ContinueAstar()
        {
            ai.canMove = true;
            ai.isStopped = false;
        }

        protected void LookAt(Transform trans)
        {
            Vector3 myLocation = transform.position;
            Vector3 targetLocation = trans.position;
            targetLocation.z = myLocation.z;
            Vector3 vectorToTarget = targetLocation - myLocation;

            Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);
            Quaternion rotation = Quaternion.LookRotation(Ship.currentShip.gameObject.transform.position - gameObject.transform.position, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f);

        }

        protected void LookAtPlayer()
        {
            LookAt(Ship.currentShip.transform);
        }

        public bool CanSeePlayer()
        {
            Transform trans = Ship.currentShip.gameObject.transform;
            Vector3 difference = trans.position - transform.position;
            Vector2 difference2D = new Vector2(difference.x, difference.y);


            RaycastHit2D[] collide = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y),
            difference2D.normalized,
            difference2D.magnitude);

            foreach (RaycastHit2D hit in collide)
            {
                if (debugCanSeePlayer)
                {
                    Debug.Log(hit.collider.gameObject);
                }
                if (hit.collider.gameObject.GetComponent<ShipMover>() != null)
                {
                    return true;
                }
            }
            return false;
        }



    }
};