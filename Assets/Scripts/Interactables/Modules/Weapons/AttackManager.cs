using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using static SwordsInSpace.UpgradeSO;
using static SwordsInSpace.Bullet;

namespace SwordsInSpace
{

    public class AttackManager : NetworkBehaviour
    {


        public record BulletInfo
        {
            public BulletInfo(GameObject bulletBase, Vector3 bulletPosition, Vector3 bulletRotation, int bulletShotSpeed, double bulletShotSpread, double bulletShotLifeTime, double bulletDamage, Vector2 bulletScale, int bulletPierce, bool isModifiable, bool isValid)
            {
                this.bulletBase = bulletBase;
                this.bulletPosition = bulletPosition;
                this.bulletRotation = bulletRotation;
                this.bulletShotSpeed = bulletShotSpeed;
                this.bulletShotLifeTime = bulletShotLifeTime;
                this.bulletShotSpread = bulletShotSpread;
                this.bulletDamage = bulletDamage;
                this.isModifiable = isModifiable;
                this.isValid = isValid;
                this.bulletScale = bulletScale;
                this.bulletPierce = bulletPierce;
                this.CallOnMove = null;
                this.CallOnHit = null;
                this.CallOnTimeout = null;
            }



            public GameObject bulletBase;
            public Vector3 bulletPosition;
            public Vector3 bulletRotation;
            public int bulletShotSpeed;
            public double bulletShotLifeTime;
            public double bulletDamage;
            public double bulletShotSpread;
            public int bulletPierce;
            public bool isModifiable; //This bullet will be modified by other upgrades down the line.
            public bool isValid; //This bullet will be produced.
            public Vector2 bulletScale;
            public BulletBehavior CallOnMove;
            public BulletBehavior CallOnHit;
            public BulletBehavior CallOnTimeout;
        }

        public List<BulletModifier> modifiers = new List<BulletModifier>() {


            //Modifiers bullet producers

            //Non Modifier bullet producers
            new MultiShot(),
            new SpreadShot(),


            //Effects

            new Convergence(),

    };


        private List<BulletInfo> bullets;

        public void ReloadUpgrades(Dictionary<UpgradeTypes, float> stats)
        {

            foreach (UpgradeTypes type in stats.Keys)
            {
                Debug.Log(type + "\t" + stats[type]);
                foreach (BulletModifier mod in modifiers)
                {
                    if (mod.thisUpgradeAttribute.type == type)
                    {
                        Debug.Log(mod.thisUpgradeAttribute.type + "UPGRADE FOR BULLETS" + stats[type]);
                        mod.thisUpgradeAttribute.amount = stats[type];
                        break;
                    }
                }
            }
        }


        public List<BulletInfo> DraftBulletLocations(GameObject bulletBase, Vector3 bulletPosition, Vector3 bulletRotation, int bulletShotSpeed, double bulletShotSpread, double bulletShotLifeTime, double bulletDamage, Vector2 bulletScale, int bulletPierce)
        {
            bullets = new List<BulletInfo>
            {
                new BulletInfo(bulletBase, bulletPosition, bulletRotation, bulletShotSpeed, bulletShotSpread, bulletShotLifeTime, bulletDamage, bulletScale, bulletPierce, true, true)
            };

            foreach (BulletModifier modifier in modifiers)
            {
                if (modifier.thisUpgradeAttribute.amount == 0)
                    continue;

                modifier.Apply(bullets);
            }
            return bullets;
        }
    }
};