using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace SwordsInSpace
{
    public class DamageListener : MonoBehaviour
    {
        public UnityEvent onDamage;
        // Start is called before the first frame update
        void Start()
        {
            onDamage = new UnityEvent();
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {


            Bullet bullet = collision.gameObject.GetComponentInParent<Bullet>();
            //Debug.Log(bullet.gameObject.tag);
            if (bullet != null && (bullet.gameObject.tag == null || bullet.gameObject.tag != "Friendly"))
            {
                Debug.Log("ow");
            }

        }
    }
};
