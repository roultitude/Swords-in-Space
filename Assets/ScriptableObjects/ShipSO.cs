using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    [CreateAssetMenu(fileName = "ShipSO", menuName = "Assets/ScriptableObjects/ShipSO")]
    public class ShipSO : ScriptableObject
    {
        public double ShipMaxHp;
        public int ShipMaxNitroFuel;
        public float ShipMaxSpeed;

        public float ShipInvincibilityTime;

        public float ShipFireInvincibilityTime;
    }
};