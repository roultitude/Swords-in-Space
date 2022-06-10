using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class PassiveShield : Module
    {
        // Start is called before the first frame update
        [SerializeField]
        public GameObject UIDisplay;
        private DisplayManager manager;

        private PassiveShieldDisplay display;




        PlayerInputManager currentPlayerInput;
        void Start()
        {
            UIDisplay = Instantiate(UIDisplay, Vector3.zero, Quaternion.identity);
            manager = DisplayManager.instance;
            UIDisplay.SetActive(false);
            display = UIDisplay.GetComponent<PassiveShieldDisplay>();

        }




        public override void Interact(GameObject player)
        {
            if (manager.Offer(UIDisplay, this))
            {
                currentPlayerInput = player.GetComponent<PlayerInputManager>();


                currentPlayerInput.SwitchView("PassiveShieldView");

                currentPlayerInput.playerInput.actions["LeftDrumstick"].performed += PassiveShieldInputLeft;
                currentPlayerInput.playerInput.actions["RightDrumstick"].performed += PassiveShieldInputRight;

                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;


            }
        }
        void OnDisplayClosed()
        {

            currentPlayerInput.SwitchView("PlayerView");

            currentPlayerInput.playerInput.actions["LeftDrumstick"].performed -= PassiveShieldInputLeft;
            currentPlayerInput.playerInput.actions["RightDrumstick"].performed -= PassiveShieldInputRight;


            DisplayManager.instance.DisplayCloseEvent -= OnDisplayClosed;
            currentPlayerInput = null;
        }

        public void PassiveShieldInputLeft(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed)
            {
                display.Drum("L");
            }
        }
        public void PassiveShieldInputRight(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed)
            {
                display.Drum("R");
            }
        }


    }
};