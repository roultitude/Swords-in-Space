using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{
    public class Shooter : NetworkBehaviour
    {
        // Start is called before the first frame update

        public Vector2 turnAxis;



        [SerializeField]
        private float rotationMin, rotationMax;

        public WeaponSO data;
        public void Setup(WeaponSO data)
        {
            this.data = data;
        }

 


        public void SpawnBullet()
        {
            if (!IsServer)
                return;
            GameObject toAdd = Instantiate(data.bulletPrefab, transform.position, transform.rotation);
            Lazer lazerSetup = toAdd.GetComponent<Lazer>();

            if (lazerSetup != null)
                lazerSetup.setShooter(this);

            toAdd.GetComponent<Projectile>().Setup(data.shotSpeed, data.shotLifeTime, data.damage, data.shotSpread);
            toAdd.transform.localScale = data.bulletScale;

            toAdd.tag = "Friendly";
            Spawn(toAdd);
        }

        private void Update()
        {
            if (!IsServer) return; //only rotate on server
            transform.localRotation = Quaternion.Euler(0, 0,
                Mathf.Clamp(transform.localRotation.eulerAngles.z - data.rotationSpeed * turnAxis.x * Time.fixedDeltaTime, rotationMin, rotationMax - 0.25f));
        }










    }
};
