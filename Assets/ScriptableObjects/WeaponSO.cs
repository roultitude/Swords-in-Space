using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    [CreateAssetMenu(fileName = "WeaponSO", menuName = "Assets/ScriptableObjects/WeaponSO")]
    public class WeaponSO : ScriptableObject
    {
        public Sprite turretSprite;

        public GameObject bulletPrefab;

        public AudioClip shootSound;

        public double damage = 1;

        public int shotSpeed = 1;

        public double shotLifeTime = 10;

        public double shotSpread = 15;

        public int burst = 3;

        public double atkCD = 5;

        public double burstCD = 0.3;

        public Vector2 bulletScale = new Vector2(3, 3);

        public float rotationSpeed = 10f;




    }
};