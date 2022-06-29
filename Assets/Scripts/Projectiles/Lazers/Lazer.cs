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

        public GameObject afterimage;
        public float fadeSpeed = 0.1f;
        private double shotSpread;
        public float range = 600f;
        public bool hasShot = false;


        private Shooter shooter;

        public void Update()
        {
            if (!hasShot && IsServer)
            {
                hasShot = true;
                StartCoroutine("LazerLifetime");

            }

        }

        public void DoRaycast()
        {


            RaycastHit2D[] collide = Physics2D.RaycastAll(shooter.gameObject.transform.position,
            (shooter.gameObject.transform.position + Quaternion.Euler(0, 0, UnityEngine.Random.Range((float)-shotSpread / 2, (float)shotSpread / 2)) * shooter.gameObject.transform.right * range).normalized,
            range);


            foreach (RaycastHit2D hit in collide)
            {

                if (hit.collider.gameObject.tag == gameObject.tag || hit.collider.gameObject.GetComponent<Projectile>() != null)
                    continue;


                EnemyAI enemy = hit.collider.gameObject.GetComponentInParent<EnemyAI>();
                Vector3 mypos = shooter.gameObject.transform.position;
                //gameObject.transform.position = new Vector3(0, 0, 0);

                if (enemy != null)
                {
                    Damage(enemy);
                }
                Connect(mypos, hit.point);
                return;
            }

            Connect(shooter.gameObject.transform.position, shooter.gameObject.transform.position + shooter.gameObject.transform.right * range);
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


            GameObject after = Instantiate<GameObject>(afterimage, new Vector3(0, 0, 0), Quaternion.identity);
            after.GetComponent<LazerAfterimage>().Setup(start, end, fadeSpeed, rend.startColor, rend.endColor);


        }

        public override void Setup(double shotSpeed, double shotLifeTime, double damage, double spread)
        {
            this.damage = damage;
            this.shotLifeTime = shotLifeTime;
            this.shotSpread = spread;
        }

        public void setShooter(Shooter s)
        {
            shooter = s;
        }

        IEnumerator LazerLifetime()
        {
            double cd = shooter.data.burstCD;
            double currentTime = 0;
            while (currentTime < shotLifeTime)
            {
                DoRaycast();

                yield return new WaitForSeconds((float)cd);
                currentTime += cd;
            }
            StartCoroutine("Fadeout");
            yield return null;
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
