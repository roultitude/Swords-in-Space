using FishNet.Object.Synchronizing;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class SpiderBossShooter : EnemyShooter
    {
        public HexagonalPlacer innerWeb;
        public HexagonalPlacer outerWeb;
        public AIPath ai;

        [SyncVar(OnChange = nameof(EnableTargets))]
        public bool targetEnabled = false;

        List<SpiderTargets> myTargets;
        void Start()
        {
            myTargets = new List<SpiderTargets>();
            //Assign the targets to shoot.
            for (int i = 0; i < 6; i++)
            {
                SpiderTargets outerTarget = outerWeb.points[i].gameObject.GetComponent<SpiderTargets>();

                outerTarget.targets.Add(outerWeb.points[i - 1 >= 0 ? i - 1 : 5].transform);

                outerTarget.targets.Add(outerWeb.points[i + 1 < 6 ? i + 1 : 0].transform);

                outerTarget.targets.Add(innerWeb.points[i].transform);

                SpiderTargets innerTarget = innerWeb.points[i].gameObject.GetComponent<SpiderTargets>();

                innerTarget.targets.Add(innerWeb.points[i - 1 >= 0 ? i - 1 : 5].transform);

                innerTarget.targets.Add(innerWeb.points[i + 1 < 6 ? i + 1 : 0].transform);

                innerTarget.targets.Add(outerWeb.points[i].transform);

                innerTarget.targets.Add(gameObject.transform);

                myTargets.Add(outerTarget);
                myTargets.Add(innerTarget);
            }

            foreach (SpiderTargets target in myTargets)
            {
                target.gameObject.SetActive(false);
            }

        }

        public new void Update()
        {
            base.Update();
            targetEnabled = ai.reachedDestination;
        }

        public override void Shoot()
        {
            if (!IsServer)
                return;

            ShootAtPlayer();

            foreach (SpiderTargets spidertarget in myTargets)
            {
                foreach (Transform target in spidertarget.targets)
                {
                    ShootAt(spidertarget.gameObject.transform, target);
                }
            }
        }


        public void EnableTargets(bool prev, bool next, bool asServer)
        {
            foreach (SpiderTargets target in myTargets)
            {
                target.gameObject.SetActive(targetEnabled);
            }
        }




    }
};