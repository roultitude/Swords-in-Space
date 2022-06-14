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
        public int maxHp;
        [SyncVar(OnChange = nameof(UpdateHpText))]
        private int currentHp;

        [SerializeField]
        public double exp;


        [SerializeField]
        Hpbar hpBar;



        readonly float fadeTime = 0.8f;


        // Start is called before the first frame update
        void Start()
        {
            currentHp = maxHp;

            if (!IsServer)
            {
                GetComponent<Rigidbody2D>().isKinematic = true;
            }

            hpBar.DoFade(fadeTime);
        }

        private void UpdateHpText(int oldint, int newint, bool server)
        {
            if (hpBar != null)
                hpBar.Resize((float)currentHp / (float)maxHp);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {


            Bullet bullet = collision.gameObject.GetComponentInParent<Bullet>();
            if (bullet != null && gameObject.tag != collision.gameObject.tag)
            {
                currentHp -= 1;
                bullet.OnHit();
                if (currentHp <= 0)
                {
                    OnDeath();
                }
            }

        }

        private void OnDeath()
        {
            Ship.currentShip.expManager.AddExp(exp);
            this.Despawn();
        }




    }

};
