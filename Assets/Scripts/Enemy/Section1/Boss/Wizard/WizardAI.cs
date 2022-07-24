using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class WizardAI : EnemyAI, RageInterface
    {
        [SerializeField]
        GameObject RengarCatPrefab;
        [SerializeField]
        GameObject FencingCatPrefab;

        RengarAI RengarCatAI;
        FencerAI FencingCatAI;

        public bool hasRagedFriend = false;


        public bool debugEnrage = false;

        private void Update()
        {
            if (debugEnrage && IsServer)
            {
                debugEnrage = false;
                StartRagePhase();
                GetComponentInChildren<WizardShooter>().StartRagePhase();
            }
            DoAllyCheck();
        }


        public override void OnStartServer()
        {
            base.OnStartServer();
            gameObject.transform.position = new Vector3(0, 20, 0);

            GameObject toSpawn = Instantiate(FencingCatPrefab, new Vector3(-20, 20, 0), transform.rotation);

            Spawn(toSpawn);

            FencingCatAI = toSpawn.GetComponent<FencerAI>();



            toSpawn = Instantiate(RengarCatPrefab, new Vector3(20, 20, 0), transform.rotation);
            //toSpawn.GetComponent<EnemyAI>().SetBoss(true);
            Spawn(toSpawn);

            RengarCatAI = toSpawn.GetComponent<RengarAI>();
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
                GetComponentInChildren<WizardShooter>().enabled = false;
                GetComponentInChildren<Collider2D>().enabled = false;
                GetComponentInChildren<SpriteRenderer>().enabled = false;
                ai.canMove = false;
                ai.isStopped = true;
                canDoBossDash = false;
            }
        }

        public void StartRagePhase()
        {
            GetComponentInChildren<WizardShooter>().StartRagePhase();
            StartCoroutine(BulkUp());
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