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
        TMPro.TextMeshProUGUI hpBar;

        // Start is called before the first frame update
        void Start()
        {
            currentHp = maxHp;
            hpBar.text = currentHp + " / " + maxHp;

            if (!IsServer)
            {
                GetComponent<Rigidbody2D>().isKinematic = true;
            }
        }

        private void UpdateHpText(int oldint, int newint, bool server)
        {
            hpBar.text = currentHp + " / " + maxHp;

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {


            Bullet bullet = collision.gameObject.GetComponentInParent<Bullet>();
            if (bullet != null && bullet.gameObject.tag == "Friendly")
            {
                currentHp -= 1;
            }

        }

    }

};
