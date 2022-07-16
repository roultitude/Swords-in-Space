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
            public BulletInfo(GameObject bulletBase, Vector3 bulletPosition, Vector3 bulletRotation, int bulletShotSpeed, double bulletShotSpread, double bulletShotLifeTime, double bulletDamage, Vector2 bulletScale, bool isModifiable)
            {
                this.bulletBase = bulletBase;
                this.bulletPosition = bulletPosition;
                this.bulletRotation = bulletRotation;
                this.bulletShotSpeed = bulletShotSpeed;
                this.bulletShotLifeTime = bulletShotLifeTime;
                this.bulletShotSpread = bulletShotSpread;
                this.bulletDamage = bulletDamage;
                this.isModifiable = isModifiable;
                this.bulletScale = bulletScale;
                this.fn = null;
            }



            public GameObject bulletBase;
            public Vector3 bulletPosition;
            public Vector3 bulletRotation;
            public int bulletShotSpeed;
            public double bulletShotLifeTime;
            public double bulletDamage;
            public double bulletShotSpread;
            public bool isModifiable;
            public Vector2 bulletScale;
            public MovementFunction fn;
        }

        public List<BulletModifier> modifiers = new List<BulletModifier>() { new MultiShot() };

        private List<BulletInfo> bullets;

        public void ReloadUpgrades(Dictionary<UpgradeTypes, float> stats)
        {
            foreach (UpgradeTypes type in stats.Keys)
            {
                foreach (BulletModifier mod in modifiers)
                {
                    if (mod.thisUpgradeAttribute.type == type)
                    {
                        mod.thisUpgradeAttribute.amount = stats[type];
                        break;
                    }
                }
            }
        }


        public List<BulletInfo> DraftBulletLocations(GameObject bulletBase, Vector3 bulletPosition, Vector3 bulletRotation, int bulletShotSpeed, double bulletShotSpread, double bulletShotLifeTime, double bulletDamage, Vector2 bulletScale)
        {
            bullets = new List<BulletInfo>
            {
                new BulletInfo(bulletBase, bulletPosition, bulletRotation, bulletShotSpeed, bulletShotSpread, bulletShotLifeTime, bulletDamage, bulletScale, true)
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