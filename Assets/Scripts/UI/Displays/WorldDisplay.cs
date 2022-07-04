using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object.Synchronizing;
using FishNet;

namespace SwordsInSpace
{
    public class WorldDisplay : Display
    {
        [SerializeField]
        TMPro.TextMeshProUGUI countdownText;

        [SerializeField]
        EnemySpawner enemySpawner;




        private void Update()
        {
            if (countdownText.enabled && enemySpawner.spawningComplete)
            {
                countdownText.enabled = false;
            }
                
            if(countdownText.enabled && enemySpawner.timeTillBoss> 0) 
            {
                countdownText.text = "Survive for " + (int) enemySpawner.timeTillBoss + " seconds!";
            }
        }
    }

}
