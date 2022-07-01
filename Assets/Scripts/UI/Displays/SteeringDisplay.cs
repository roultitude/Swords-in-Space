using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    
namespace SwordsInSpace
{
    public class SteeringDisplay : Display
    {

        [SerializeField]
        private TextMeshProUGUI nitroText;
        private Steering currentSteeringModule;
        
        
        public override void Setup(Interactable callingMod)
        {
            if (callingMod is not Steering)
            {
                Debug.Log("Non steering module opening steering UI!");
                return;
            }
            currentSteeringModule = (Steering) callingMod;
            
        }

        public void UpdateDisplay()
        {
            nitroText.text = Ship.currentShip.CurrentNitroFuel.ToString() + " fuel left";
        }

        private void Update()
        {
            UpdateDisplay();
        }
    }
}