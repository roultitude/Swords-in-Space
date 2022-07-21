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
        Spawner Spawner;




        private void Update()
        {
            if (countdownText.enabled && Spawner.spawningComplete)
            {
                countdownText.enabled = false;
            }

            if (countdownText.enabled && Spawner.timeTillBoss > 0)
            {
                countdownText.text = "Survive for " + (int)Spawner.timeTillBoss + " seconds!";
            }
        }
    }

}
