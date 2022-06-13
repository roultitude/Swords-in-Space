using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    [CreateAssetMenu(menuName = "Assets/ScriptableObjects")]
    public class ItemSO : ScriptableObject
    {
        /*
         * Reflects Item changes available for items to alter
         * Iterpreted by Ship at runtime to upgrade.
         */

        public int maxHp = 0;
        public int maxHpPercent = 0;

        public int bulletSpeed = 0;
        public int bulletSpeedPercent = 0;

        public int shotSpeed = 0;
        public int shotSpeedPercent = 0;

        public int shotLifetime = 0;
        public int shotLifetimePercent = 0;

        /*
         * Todo Add support to alter more stats
         */


    }
};