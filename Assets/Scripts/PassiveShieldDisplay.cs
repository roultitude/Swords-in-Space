using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class PassiveShieldDisplay : Display
    {

        public float BeatTime = 0.3f;
        public float BeatChance = 0.8f;


        public float incrementPercent = 0.03f;

        [SerializeField]
        private float increment = 0;

        public float threshold = 0.5f;

        [SerializeField]
        private Timer beatTracker;

        [SerializeField]
        private GameObject drumBeat;

        [SerializeField]
        private GameObject drum;

        [SerializeField]
        private List<Drumbeat> drumbeats;

        public void Start()
        {
            drumbeats = new List<Drumbeat>();
        }

        public void startGame()
        {
            increment = 0;
            beatTracker.Start();
        }

        public void Beat()
        {
            if (Random.Range(0f, 1f) < BeatChance)
            {
                GameObject spawned = Instantiate(drumBeat, gameObject.transform);
                Drumbeat drumbeat = spawned.GetComponent<Drumbeat>();
                drumbeat.SetIdentity(Random.Range(0, 2) == 0 ? "L" : "R");
                drumbeats.Add(drumbeat);
            }
        }

        public void Drum(string dir)
        {

            Drumbeat drumbeat = drumbeats[0];
            drumbeats.RemoveAt(0);
            if (drumbeat.identity == dir)
            {
                increment++;
            }
            else
            {
                Debug.Log("CCCCOMBO BREAKER");
                increment = 0;
            }


        }

        private void Success()
        {
            increment++;
        }

    }
};
