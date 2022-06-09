using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwordsInSpace
{
    public class SteeringDisplay : Display
    {
        [SerializeField]
        private Image nitroButton;
        [SerializeField]
        private Image forwardButton;
        [SerializeField]
        private Image offButton;
        [SerializeField]
        private Image backthrustButton;

        private Steering currentSteeringModule;
        
        public override void Setup(Interactable callingMod)
        {
            if (callingMod is not Steering)
            {
                Debug.Log("Non steering module opening steering UI!");
                return;
            }
            currentSteeringModule = (Steering) callingMod;
            UpdateDisplay(currentSteeringModule.currentSteerState);
        }

        public void UpdateDisplay(SteerState currentSteerState)
        {
            nitroButton.color = currentSteerState == SteerState.NITRO ? Color.cyan : Color.white;
            forwardButton.color = currentSteerState == SteerState.FORWARD ? Color.cyan : Color.white;
            offButton.color = currentSteerState == SteerState.OFF ? Color.cyan : Color.white;
            backthrustButton.color = currentSteerState == SteerState.BACKWARD ? Color.cyan : Color.white;
        }

        private void Update()
        {
            UpdateDisplay(currentSteeringModule.currentSteerState);
        }
    }
}