using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class BossCatWizardAI : EnemyAI, RageInterface
    {
        [SerializeField]
        GameObject RengarCatPrefab;
        [SerializeField]
        GameObject FencingCatPrefab;

        BossCatChargerAI RengarCatAI;
        BossCatFencerAI FencingCatAI;
        SpriteRenderer sprite;
        public bool hasRagedFriend = false;


        public bool debugEnrage = false;

        private void Awake()
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
        private void Update()
        {
            if (debugEnrage && IsServer)
            {
                debugEnrage = false;
                StartRagePhase();
                GetComponentInChildren<BossCatWizardShooter>().StartRagePhase();
            }
            sprite.flipY = transform.rotation.eulerAngles.z < 180;
            DoAllyCheck();
        }


        public override void OnStartServer()
        {
            base.OnStartServer();
            gameObject.transform.position = new Vector3(0, 20, 0);

            GameObject toSpawn = Instantiate(FencingCatPrefab, new Vector3(-20, 20, 0), transform.rotation);

            Spawn(toSpawn);

            FencingCatAI = toSpawn.GetComponent<BossCatFencerAI>();



            toSpawn = Instantiate(RengarCatPrefab, new Vector3(20, 20, 0), transform.rotation);
            //toSpawn.GetComponent<EnemyAI>().SetBoss(true);
            Spawn(toSpawn);

            RengarCatAI = toSpawn.GetComponent<BossCatChargerAI>();
        }

        public void DoAllyCheck()
        {
            if (!IsServer) return;
            int deadCat = 0;
            RageInterface aliveCat = null;
            if (currentHp <= 0)
            {
                deadCat++;
            }
            else
            {
                aliveCat = this;
            }







            if (RengarCatAI == null || RengarCatAI.GetCurrentHP() <= 0)
            {
                deadCat++;
            }
            else
            {
                aliveCat = RengarCatAI;
            }

            if (FencingCatAI == null || FencingCatAI.GetCurrentHP() <= 0)
            {
                deadCat++;
            }
            else
            {
                aliveCat = FencingCatAI;
            }



            if (deadCat == 2 && aliveCat != null && !hasRagedFriend)
            {
                hasRagedFriend = true;
                aliveCat.StartRagePhase();
            }
            if (deadCat == 3)
            {
                base.OnDeath();
            }
        }



        public override void takeDamage(double dmg)
        {
            currentHp -= dmg;
            AudioManager.instance.ObserversPlay(onDamagedSound);

            if (!isActive)
                base.SetActive();

            if (currentHp <= 0)
            {
                GetComponentInChildren<BossCatWizardShooter>().enabled = false;
                GetComponentInChildren<Collider2D>().enabled = false;
                GetComponentInChildren<SpriteRenderer>().enabled = false;
                ai.canMove = false;
                ai.isStopped = true;
                canDoBossDash = false;
            }
        }

        public void StartRagePhase()
        {
            GetComponentInChildren<BossCatWizardShooter>().StartRagePhase();
            StartCoroutine(BulkUp());
            StartCoroutine(RageStateRepositioning());

        }

        public IEnumerator RageStateRepositioning()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);
                StartCoroutine(BossTeleport());
            }
        }


        public IEnumerator BulkUp()
        {
            int pumps = 3;
            for (int i = 0; i < pumps; i++)
            {
                gameObject.transform.localScale *= 1.25f;
                currentHp += 10;
                maxHp += 10;
                yield return new WaitForSeconds(0.8f);
            }
        }
    }
};