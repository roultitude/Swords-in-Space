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

            if (countdownText.enabled && Spawner.questObjectiveCollected < Spawner.NumQuestObjectivePowerupToSpawn)
            {
                countdownText.text = "Collect " + (Spawner.NumQuestObjectivePowerupToSpawn - Spawner.questObjectiveCollected) + " more fuel cells!";
            }
        }
    }

}
