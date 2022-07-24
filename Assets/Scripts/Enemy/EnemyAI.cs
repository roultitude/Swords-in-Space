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

        private Vector2 teleportProximityToPlayer = new Vector2(100f, 200f);

        readonly float fadeTime = 0.8f;
        private Coroutine teleportCorountine;
        public bool isBoss;
        private Hpbar hpBar;
        protected AIPath ai;

        public double detectionRange = 30;

        [SyncVar(OnChange = nameof(ChangeActiveState))]
        private bool isActive = true;

        protected Rigidbody2D rb;

        [SerializeField]
        private AfterImageSpawner imageSpawner;

        private float bossAfterimageDistance = 5f;
        private float bossAfterimageCD = 0.05f;
        private Vector2 bossTeleportProximityToPlayer = new Vector2(5f, 30f);


        // Start is called before the first frame update
        protected void Start()
        {
            rb = GetComponent<Rigidbody2D>();
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


        public IEnumerator ScanDuringSleep()
        {
            while (true)
            {
                if (isActive)
                    break;

                float distance = Vector3.Distance(Ship.currentShip.gameObject.transform.position, gameObject.transform.position);
                if (distance <= detectionRange)
                    SetActive();

                yield return new WaitForSeconds(0.3f);
            }
        }

        private void ChangeActiveState(bool prev, bool current, bool serv)
        {
            if (current)
            {
                SetActive();
            }
            else
            {
                SetInactive();
            }

        }

        public void SetInactive()
        {

            gameObject.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

            isActive = false;
            StartCoroutine("ScanDuringSleep");
            foreach (EnemyMover m in GetComponentsInChildren<EnemyMover>())
                m.OnSleepEnemyMover();

            foreach (EnemyShooter s in GetComponentsInChildren<EnemyShooter>())
                s.isAsleep = true;
        }

        private void SetActive()
        {
            if (isActive)
                return;

            isActive = true;
            foreach (EnemyMover mover in GetComponentsInChildren<EnemyMover>())
            {
                mover.OnAwakeEnemyMover();
            }
            foreach (EnemyShooter s in GetComponentsInChildren<EnemyShooter>())
                s.isAsleep = false;

        }

        private IEnumerator farFromPlayerCheck()
        {
            float currentTime = 0;
            float tickTime = 0.3f;
            float maxDistance = isBoss ? bossTeleportProximityToPlayer.y * 1.5f : teleportProximityToPlayer.y * 1.5f;
            while (true)
            {
                yield return new WaitForSeconds(tickTime);

                if (!isActive)
                    continue;

                if ((gameObject.transform.position - Ship.currentShip.transform.position).magnitude > maxDistance)
                {
                    currentTime += tickTime;
                }
                else
                {

                    currentTime = 0;
                }

                if (currentTime > teleportCheckDuration && doTeleportIfFar)
                {
                    currentTime = 0;
                    if (isBoss)
                    {
                        StartCoroutine("BossTeleport");
                    }
                    else
                    {
                        TeleportNearPlayer();
                    }

                    //Debug.Log("Teleport?");


                }

            }
        }

        private IEnumerator BossTeleport()
        {
            Debug.Log("Boss teleport");
            Vector3 loc = getPosNearPlayer(bossTeleportProximityToPlayer);
            Vector3 currentLoc = gameObject.transform.position;

            if (currentLoc == loc)
                yield break;

            float bossAfterImageCount = Vector3.Distance(loc, currentLoc) / bossAfterimageDistance;

            rb.isKinematic = true;
            for (float prog = 0f; prog <= 1.0f; prog += 1f / (float)bossAfterImageCount)
            {
                imageSpawner.SpawnImage();
                ai.Teleport(new Vector3(Mathf.Lerp(currentLoc.x, loc.x, prog), Mathf.Lerp(currentLoc.y, loc.y, prog), 0));

                yield return new WaitForSeconds(bossAfterimageCD);
            }

            rb.isKinematic = false;


        }

        private void TeleportNearPlayer()
        {
            ai.Teleport(getPosNearPlayer(teleportProximityToPlayer));
        }

        private Vector3 getPosNearPlayer(Vector2 proximity)
        {
            Vector2 pos;
            Vector2 shipPos = new Vector2(Ship.currentShip.transform.position.x, Ship.currentShip.transform.position.y);
            int attempts = 50;

            for (int i = 0; i < attempts; i++)
            {
                pos = new Vector2(Random.Range(0, proximity.y - proximity.x) + proximity.x, Random.Range(0, proximity.y - proximity.x) + proximity.x);
                if (Random.Range(0, 2) == 0)
                {
                    pos.x = -pos.x;
                }

                if (Random.Range(0, 2) == 0)
                {
                    pos.y = -pos.y;
                }

                pos += shipPos;

                if (isValidSpawnLocation(pos))
                {
                    return new Vector3(pos.x, pos.y, 0);
                }
            }

            return new Vector3(transform.position.x, transform.position.y, 0);
        }

        public bool isValidSpawnLocation(Vector3 pos)
        {
            return Physics2D.OverlapCircle(new Vector2(pos.x, pos.y), 1) == null;
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

            if (!isActive)
                SetActive();

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
