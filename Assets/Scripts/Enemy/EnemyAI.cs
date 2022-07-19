using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;
using Pathfinding;


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
        public AudioClip onDamagedSound;

        public bool doTeleportIfFar = true;
        public float teleportCheckDuration = 5f;
        public float teleportIfFurtherThanDistance = 300f;

        private Vector2 teleportProximityToPlayer = new Vector2(100f, 200f);

        readonly float fadeTime = 0.8f;
        private Coroutine teleportCorountine;
        private bool isBoss;
        private Hpbar hpBar;
        protected AIPath ai;
        // Start is called before the first frame update
        protected void Start()
        {
            currentHp = maxHp;
            ai = GetComponent<AIPath>();
            teleportCorountine = StartCoroutine("farFromPlayerCheck");
            if (!IsServer)
            {
                GetComponent<Rigidbody2D>().isKinematic = true;

            }
            hpBar = GetComponentInChildren<Hpbar>();
            hpBar.DoFade(fadeTime);
        }

        private IEnumerator farFromPlayerCheck()
        {
            float currentTime = 0;
            float tickTime = 0.3f;

            while (true)
            {
                yield return new WaitForSeconds(tickTime);

                if ((gameObject.transform.position - Ship.currentShip.transform.position).magnitude > teleportIfFurtherThanDistance)
                {
                    currentTime += tickTime;
                }
                else
                {

                    currentTime = 0;
                }

                if (currentTime > teleportCheckDuration && doTeleportIfFar)
                {
                    //Debug.Log("Teleport?");
                    currentTime = 0;
                    TeleportNearPlayer();
                }

            }
        }

        private void TeleportNearPlayer()
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            Vector2 shipPos = new Vector2(Ship.currentShip.transform.position.x, Ship.currentShip.transform.position.y);
            int attempts = 10;

            for (int i = 0; i < attempts; i++)
            {
                pos = new Vector2(Random.Range(0, teleportProximityToPlayer.y), Random.Range(0, teleportProximityToPlayer.y));

                float distanceToShip = (pos - shipPos).magnitude;

                if (distanceToShip < teleportProximityToPlayer.y && distanceToShip > teleportProximityToPlayer.x && isValidSpawnLocation(pos))
                {
                    ai.Teleport(new Vector3(pos.x, pos.y, 0));
                }
            }


        }

        public bool isValidSpawnLocation(Vector3 pos)
        {
            return Physics2D.OverlapCircle(new Vector2(pos.x, pos.y), 5) == null;
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
            AudioManager.instance.ObserversPlay(onDamagedSound);
            if (currentHp <= 0)
            {
                OnDeath();
            }
        }

        public void SetBoss(bool isBoss)
        {
            this.isBoss = isBoss;
        }

        private void OnDeath()
        {
            if (!IsServer) return;
            if (isBoss) WorldManager.currentWorld.spawner.bossesKilled++;
            WorldManager.currentWorld.StartCoroutine(WorldManager.currentWorld.CheckIfComplete());
            Ship.currentShip.expManager.AddExp(exp);
            this.Despawn();
        }




    }

};
