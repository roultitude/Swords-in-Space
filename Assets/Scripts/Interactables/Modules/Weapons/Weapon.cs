using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public abstract class Weapon : Module
    {
        // Start is called before the first frame update
        [SerializeField]
        public WeaponSO data;

        public int currentAmmo;
        public Shooter[] shooters;

        void Awake()
        {

            shooters = GetComponentsInChildren<Shooter>();

            foreach (Shooter comp in shooters)
            {
                Debug.Log(comp.name);
                comp.data = data;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Interact(GameObject player)
        {
            Shoot();
        }

        public abstract void Shoot();


    }
};