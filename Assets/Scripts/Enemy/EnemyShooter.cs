using SwordsInSpace;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class EnemyShooter : NetworkBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    GameObject bullet;
    [SerializeField]
    public float range = 100f;

    public double damage;

    private bool IsInRange()
    {
        return range > Vector3.Distance(gameObject.transform.position, Ship.currentShip.gameObject.transform.position);
    }



    public void Shoot()
    {
        if (IsServer && IsInRange())
        {
            Vector3 myLocation = transform.position;
            Vector3 targetLocation = Ship.currentShip.transform.position;
            targetLocation.z = myLocation.z;
            Vector3 vectorToTarget = targetLocation - myLocation;

            Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);





            Quaternion rotation = Quaternion.LookRotation(Ship.currentShip.gameObject.transform.position - gameObject.transform.position, Vector3.forward);


            GameObject toAdd = Instantiate(bullet, transform.position, rotation);

            toAdd.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f);

            toAdd.GetComponent<Bullet>().Setup(10f, 10f, damage);
            Spawn(toAdd);
        }
    }
}
