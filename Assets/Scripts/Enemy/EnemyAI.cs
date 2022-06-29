using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;

namespace SwordsInSpace
{
    public class EnemyAI : NetworkBehaviour
    {
        [SerializeField]
        public double maxHp;
        [SyncVar(OnChange = nameof(UpdateHpText))]
        private double currentHp;

        [SerializeField]
        public double exp;


        [SerializeField]
        Hpbar hpBar;



        readonly float fadeTime = 0.8f;


        // Start is called before the first frame update
        protected void Start()
        {
            currentHp = maxHp;

            if (!IsServer)
            {
                GetComponent<Rigidbody2D>().isKinematic = true;
            }

            hpBar.DoFade(fadeTime);
        }

        private void UpdateHpText(double oldint, double newint, bool server)
        {
            if (hpBar != null)
                hpBar.Resize((float)currentHp / (float)maxHp);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {


            Bullet bullet = collision.gameObject.GetComponentInParent<Bullet>();
            if (bullet != null && gameObject.tag != collision.gameObject.tag)
            {
                takeDamage(bullet.damage);
                bullet.OnHit();
            }

        }

        public void takeDamage(double dmg)
        {
            currentHp -= dmg;

            if (currentHp <= 0)
            {
                OnDeath();
            }
        }

        private void OnDeath()
        {
            if (!IsServer) return;
            WorldManager.currentWorld.StartCoroutine(WorldManager.currentWorld.CheckIfComplete());
            Ship.currentShip.expManager.AddExp(exp);
            this.Despawn();
        }




    }

};
