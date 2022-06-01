using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace SwordsInSpace
{
    public class EnemyAI : NetworkBehaviour
    {
        [SerializeField]
        public int maxHp;
        private int currentHp;

        // Start is called before the first frame update
        void Start()
        {
            currentHp = maxHp;
            GetComponentInChildren<DamageListener>().onDamage.AddListener(Damage);
        }

        // Update is called once per framet
        void Update()
        {

        }

        public void Damage()
        {
            Debug.Log("hit enemy");
        }


    }
};
