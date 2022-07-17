using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    [CreateAssetMenu(fileName = "UpgradeSO", menuName = "Assets/ScriptableObjects/UpgradeSO")]
    public class UpgradeSO : ScriptableObject
    {
        /*
         * Reflects Upgrade changes available for items to alter
         * Iterpreted by Ship at runtime to upgrade.
         */
        public string upgradeName;
        public string description;
        public Sprite sprite;

        [System.Serializable]
        public enum UpgradeTypes
        {
            //Ship Stat Changes
            maxHp,
            maxHpPercent,
            maxShipSpeed,

            //Weapon State Changes
            shotSpeed,
            shotDamage,
            shotLifetime,


            //Bullet Spawning Upgrades
            multiShot, //100% Chance, + 2 bullet fired. Bullets fired cannot be multiplied
            spreadShot, //100% Chance, +2 bullets fired divided in a 180 cone. Bullets fired cannot be multiplied



            //Bullet Pathing Upgrades
            bulletCurve, //Stacking increases curve 
            bulletWave, //Stacking increases amplitude

            //Special Bullet Upgrades
            convergence

        }
        [System.Serializable]
        public struct UpgradeAttributes
        {
            public UpgradeTypes type;
            public float amount;
        }


        public UpgradeAttributes[] statChanges;


    }
};