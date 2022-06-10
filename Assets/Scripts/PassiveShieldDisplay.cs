using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SwordsInSpace
{
    public class PassiveShieldDisplay : Display
    {

        public float BeatTime = 3f;
        public float BeatChance = 0.8f;


        public float incrementPercent = 0.03f;

        [SerializeField]
        private float increment = 0;

        public float distanceThreshold = 0.5f;

        [SerializeField]
        private float baseMoveSpeed = 0.25f;

        [SerializeField]
        private Timer beatTracker;

        [SerializeField]
        private GameObject drumBeat;

        [SerializeField]
        private GameObject drum;

        [SerializeField]
        private List<Drumbeat> drumbeats;

        [SerializeField]
        private TMPro.TextMeshProUGUI comboCounter;

        private int defaultFontSize = 36;

        private int fontSizeIncrement = 5;
        private int defaultCurrentShake = 5;
        private int currentShake = 5;

        private int cullDrumBeatsAt = -400;

        public void Start()
        {
            drumbeats = new List<Drumbeat>();
            startGame();

        }

        public void startGame()
        {
            increment = 0;
            updateAll();
            beatTracker.Start();



        }

        private void updateAll()
        {
            updateBeatTime();
            updateFallSpeed();
            updateComboCounter();
        }
        private void updateBeatTime()
        {
            beatTracker.waitTime = BeatTime * (Mathf.Pow((1 - incrementPercent), increment));
        }

        private void updateFallSpeed()
        {
            Drumbeat.moveSpeed = baseMoveSpeed * (Mathf.Pow((1 + incrementPercent), increment));
        }

        private void updateComboCounter()
        {
            comboCounter.text = increment.ToString();
            comboCounter.fontSize = defaultFontSize + increment * fontSizeIncrement;
            currentShake = defaultCurrentShake + (int)increment * 2;
            comboCounter.transform.DOShakePosition(0.2f, new Vector3(currentShake, currentShake, 0), currentShake, 90, false);
            comboCounter.transform.DOShakePosition(0.1f, new Vector3(currentShake, currentShake, 0), currentShake, 90, false);
            comboCounter.transform.DOShakePosition(0.15f, new Vector3(currentShake, currentShake, 0), currentShake, 90, false);
        }

        public void Beat()
        {
            if (Random.Range(0f, 1f) < BeatChance)
            {
                GameObject spawned = Instantiate(drumBeat, gameObject.transform);
                Drumbeat drumbeat = spawned.GetComponent<Drumbeat>();
                spawned.transform.position -= new Vector3(0, -600f, 0);
                drumbeat.SetIdentity(Random.Range(0, 2) == 0 ? "L" : "R");
                drumbeats.Add(drumbeat);
            }
        }

        public void Update()
        {
            if (drumbeats.Count == 0)
                return;
            Drumbeat drumbeat = drumbeats[0];
            if (drumbeat.gameObject.transform.position.y < cullDrumBeatsAt)
                Drum("Fail");
        }

        public void Drum(string dir)
        {
            if (drumbeats.Count == 0)
                return;
            Drumbeat drumbeat = drumbeats[0];
            drumbeats.RemoveAt(0);
            float distance = Vector3.SqrMagnitude(drumbeat.gameObject.transform.position - drum.gameObject.transform.position);
            if (drumbeat.identity == dir && distance < distanceThreshold * distanceThreshold)
            {
                increment++;
            }
            else
            {
                Debug.Log("CCCCOMBO BREAKER\t Combos achieved:\t" + increment);
                Ship.currentShip.addHp((int)increment / 5);
                increment = 0;
                while (drumbeats.Count > 0)
                {
                    Destroy(drumbeats[0].gameObject);
                    drumbeats.RemoveAt(0);
                }

            }
            updateAll();
            Destroy(drumbeat.gameObject);

        }

    }
};
