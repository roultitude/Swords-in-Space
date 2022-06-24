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
            maxHp,
            maxHpPercent,
            bulletSpeed,
            bulletSpeedPercent,
            shotSpeed,
            shotSpeedPercent,
            shotLifetime,
            shotLifetimePercent
        }
        [System.Serializable]
        public struct UpgradeAttributes
        {
            public UpgradeTypes type;
            public float amount;
        }


        public UpgradeAttributes[] statChanges;



        [System.Serializable]
        public enum UpgradeBoolsTypes
        {
            placeholder
        }
        [System.Serializable]
        public struct UpgradeBools
        {
            public UpgradeBoolsTypes type;
            public bool activated;
        }

        public UpgradeBools[] gameplayFlags;


    }
};