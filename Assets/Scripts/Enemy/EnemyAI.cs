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
        Sprite whitePixel;

        HealthBar hpBar;


        // Start is called before the first frame update
        void Start()
        {
            currentHp = maxHp;

            if (!IsServer)
            {
                GetComponent<Rigidbody2D>().isKinematic = true;
            }

            hpBar = HealthBar.Create(new Vector2(0, 50), new Vector2(maxHp * 250, 25), 5f, gameObject, whitePixel);
            hpBar.transform.parent = gameObject.transform;
            hpBar.DoFade(0.8f);
        }

        private void UpdateHpText(int oldint, int newint, bool server)
        {
            if (hpBar != null)
                hpBar.setSize((float)currentHp / (float)maxHp);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {


            Bullet bullet = collision.gameObject.GetComponentInParent<Bullet>();
            if (bullet != null && bullet.gameObject.tag == "Friendly")
            {
                currentHp -= 1;
                if (currentHp <= 0)
                {
                    onDeath();
                }
            }

        }

        private void onDeath()
        {
            this.Despawn();
        }




    }

};