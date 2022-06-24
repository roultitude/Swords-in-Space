using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SwordsInSpace
{
    public class LevelCompleteDisplay : Display
    {
        [SerializeField]
        private float countdownDuration;
        [SerializeField]
        private TMPro.TextMeshProUGUI text;


        private float countdownTimeLeft;

        public override void Awake()
        {
            countdownTimeLeft = countdownDuration;
        }

        private void Update()
        {
            countdownTimeLeft -= Time.deltaTime;
            UpdateText();
        }

        private void UpdateText()
        {
            text.text = "Congratulations! You've defeated all the enemies. Warping in " + ((int) countdownTimeLeft).ToString() + "s!";
        }
    }
}

