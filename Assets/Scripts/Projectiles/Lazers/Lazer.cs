using DG.Tweening;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SwordsInSpace
{
    public class Lazer : Projectile
    {
        // Update is called once per frame
        public LineRenderer rend;
        public float fadeSpeed = 0.1f;
        public float range = 600f;
        public bool hasShot = false;

        public void Update()
        {
            if (!hasShot && IsServer)
            {
                hasShot = true;
                DoRaycast();

            }

        }

        public void DoRaycast()
        {
            RaycastHit2D[] collide = Physics2D.RaycastAll(gameObject.transform.position,
            (gameObject.transform.position + gameObject.transform.right * range).normalized,
            range);


            foreach (RaycastHit2D hit in collide)
            {

                if (hit.collider.gameObject.tag == gameObject.tag || hit.collider.gameObject.GetComponent<Projectile>() != null)
                    continue;


                EnemyAI enemy = hit.collider.gameObject.GetComponentInParent<EnemyAI>();
                Vector3 mypos = gameObject.transform.position;
                gameObject.transform.position = new Vector3(0, 0, 0);

                if (enemy != null)
                {
                    Damage(enemy);
                }
                Connect(mypos, hit.point);
                return;
            }

            Connect(gameObject.transform.position, gameObject.transform.position + gameObject.transform.right * range);
            return;

        }

        private void Damage(EnemyAI enemy)
        {
            enemy.takeDamage(damage);
        }

        [ObserversRpc]
        public void Connect(Vector3 start, Vector3 end)
        {
            rend.useWorldSpace = true;

            rend.positionCount = 2;
            rend.SetPositions(new Vector3[] { start, end });
            //rend.material.DOFade(0, fadeDur).OnComplete(() => { Despawn(); });

            StartCoroutine("Fadeout");


        }

        public override void Setup(double shotSpeed, double shotLifeTime, double damage)
        {
            this.damage = damage;

        }

        IEnumerator Fadeout()
        {
            for (float f = 1f; f >= -0.05f; f -= fadeSpeed * (float)TimeManager.TickDelta)
            {
                Color c = rend.startColor;
                c.a = f;
                rend.startColor = c;
                c = rend.endColor;
                c.a = f;
                rend.endColor = c;
                yield return null;
            }

            Despawn();
        }
    }
};
