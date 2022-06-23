using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SwordsInSpace
{
    public class ExpManager : MonoBehaviour
    {
        private int baseExpToLevel = 50;

        public int currentLevel = 0;
        public double currentExp = 0;
        public int storedLevels = 0;

        public UnityEvent onLevel;

        public void Start()
        {
            onLevel = new UnityEvent();
            storedLevels = 0;
        }

        public int ExpToLevel(int level)
        {

            int a = baseExpToLevel * level;
            return baseExpToLevel + (int)(5 * level / Mathf.Log(a));
        }

        public void AddExp(double exp)
        {

            currentExp += exp;
            while (currentExp >= ExpToLevel(currentLevel))
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            currentExp -= ExpToLevel(currentLevel);
            currentLevel += 1;
            storedLevels += 1;
            onLevel.Invoke();
            Debug.Log("Leveled Up! " + currentLevel);
        }

        public int GetStoredLevels()
        {
            int output = storedLevels;
            storedLevels = 0;
            return output;
        }
    }
};