using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SwordsInSpace
{
    public class Timer : MonoBehaviour
    {
        public bool autoplay = false; // Do I start immediately
        public bool repeat = false; // Do I repeat after done?
        public double waitTime = 0.00;

        public UnityEvent timeout;
        private bool active = false;
        private double currentTime = 0.0;

        public void Setup(double waitTime, bool repeat, bool autoplay)
        {
            this.waitTime = waitTime;
            this.repeat = repeat;
            this.autoplay = autoplay;
            timeout = new UnityEvent();
            if (autoplay)
            {
                active = true;
                return;
            }
            enabled = false;
        }

        public void Setup(double waitTime, bool repeat)
        {
            Setup(waitTime, repeat, true);
        }

        public void Setup(double waitTime)
        {
            Setup(waitTime, false, true);
        }



        // Update is called once per frame
        void Update()
        {
            if (!active)
                return;

            if (waitTime <= 0.00)
            {
                Stop();
                return;
            }

            currentTime += Time.deltaTime;

            if (currentTime > waitTime)
            {

                if (repeat)
                {
                    currentTime = 0;
                }
                else
                {
                    Stop();
                }
                timeout.Invoke();

            }

        }

        public void Pause()
        {
            active = false;
        }

        public void Start()
        {
            enabled = true;
            active = true;
            currentTime = 0;

        }

        public void Stop()
        {
            enabled = false;
            currentTime = 0;
            active = false;
        }
    }
};
