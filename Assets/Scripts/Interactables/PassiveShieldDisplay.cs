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
        public float increment = 0;

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
        private GameObject LDrumButton, RDrumButton;

        [SerializeField]
        private TMPro.TextMeshProUGUI comboCounter;

        [SerializeField]
        private AudioClip hitSound, failSound, drumSound;

        private int defaultFontSize = 88;

        private int fontSizeIncrement = 5;
        private int defaultCurrentShake = 5;
        private int currentShake = 5;

        private int maxComboScale = 30;

        private int cullDrumBeatsAt = -400;

        public PassiveShield shield;



        public void Start()
        {
            drumbeats = new List<Drumbeat>();
            startGame();

        }

        public void SetupUI(PassiveShield shield)
        {
            this.shield = shield;
        }
        public void startGame()
        {
            increment = 0;
            updateAll();
            beatTracker.Start();
            while (drumbeats.Count > 0)
            {
                Destroy(drumbeats[0].gameObject);
                drumbeats.RemoveAt(0);
            }


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
            if (increment > maxComboScale)
                return;

            Drumbeat.moveSpeed = baseMoveSpeed * (Mathf.Pow((1 + incrementPercent), increment));
        }

        private void updateComboCounter()
        {
            comboCounter.text = "Combo " + increment.ToString();
            float clampedIncrement = Mathf.Clamp((float)increment, 0, maxComboScale);
            //Debug.Log(1f - Mathf.Clamp((float)increment, 0, maxComboScale) / (float)maxComboScale);
            comboCounter.color = new Color(1, 1f - clampedIncrement / (float)maxComboScale, 0);
            comboCounter.fontSize = defaultFontSize + clampedIncrement * fontSizeIncrement;
            currentShake = defaultCurrentShake + (int)clampedIncrement * 2;
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
            animateDrum(dir);
            AudioManager.instance.effectAudioSource.PlayOneShot(drumSound);
            if (drumbeats.Count == 0)
                return;
            Drumbeat drumbeat = drumbeats[0];
            drumbeats.RemoveAt(0);
            float distance = Vector3.SqrMagnitude(drumbeat.gameObject.transform.position - drum.gameObject.transform.position);
            if (drumbeat.identity == dir && distance < distanceThreshold * distanceThreshold)
            {
                increment++;
                shield.healFromBeat();
                AudioManager.instance.effectAudioSource.PlayOneShot(hitSound);
            }
            else
            {
                shield.healFromCombo(increment);

                increment = 0;
                while (drumbeats.Count > 0)
                {
                    Destroy(drumbeats[0].gameObject);
                    drumbeats.RemoveAt(0);
                }
                AudioManager.instance.effectAudioSource.PlayOneShot(failSound);
            }
            updateAll();
            Destroy(drumbeat.gameObject);

        }

        private void animateDrum(string dir)
        {
            switch (dir)
            {
                case "L":
                    LDrumButton.transform.DOScale(1.2f, 0.1f).SetEase(Ease.InOutSine).OnComplete(() => LDrumButton.transform.DOScale(1f, 0.1f).SetEase(Ease.InOutBounce));
                    break;
                case "R":
                    RDrumButton.transform.DOScale(1.2f, 0.1f).SetEase(Ease.InOutSine).OnComplete(() => RDrumButton.transform.DOScale(1f, 0.1f).SetEase(Ease.InOutBounce));
                    break;
            }
        }
    }
};
