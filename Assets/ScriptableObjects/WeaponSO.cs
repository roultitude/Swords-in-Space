using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    [CreateAssetMenu(menuName = "Assets/ScriptableObjects")]
    public class WeaponSO : ScriptableObject
    {
        public Sprite turretSprite;

        public GameObject bulletPrefab;

        public int damage = 1;

        public int shotSpeed = 1;

        public double shotLifeTime = 10;

        public int burst = 3;

        public int maxAmmo = 3;

        public int ammoRefill = 1;

        public double atkCD = 5;

        public double burstCD = 0.3;
    }
};