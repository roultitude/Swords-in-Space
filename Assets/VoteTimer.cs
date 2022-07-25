using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FishNet;

namespace SwordsInSpace
{
    public class VoteTimer : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]
        private Timer timer;

        public TextMeshProUGUI textBox;

        private void Awake()
        {
            if (!InstanceFinder.IsServer) return;
            timer.timeout.AddListener(VoteTimeout);
        }

        // Update is called once per frame
        void Update()
        {

            string timestr = "Time Remaining: " + (int)(timer.waitTime - timer.currentTime);
            if (timestr != textBox.text)
            {
                textBox.text = timestr;
            }
        }

        public void ResetTimer()
        {
            timer.Start();
        }

        void VoteTimeout()
        {
            Ship.currentShip.upgradeManager.AddRandomVote();
        }
    }
};