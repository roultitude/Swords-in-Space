using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SwordsInSpace
{
    public enum SteerState { NITRO, FORWARD, OFF, BACKWARD }
    public class Steering : Module
    {

        [SerializeField]
        public GameObject UIDisplay;

        [SyncVar]
        public SteerState currentSteerState;
        private DisplayManager manager;
        ShipMover shipMover;

        Vector2 turnAxis;
        PlayerInputManager currentPlayerInput;
        void Awake()
        {
            currentSteerState = SteerState.OFF;
        }
        private void OnEnable()
        {
        }
        private void OnDisable()
        {
            InstanceFinder.TimeManager.OnTick -= OnTick;
        }
        void Start()
        {
            UIDisplay = Instantiate(UIDisplay, Vector3.zero, Quaternion.identity);
            manager = DisplayManager.instance;
            UIDisplay.SetActive(false);

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDisplayClosed()
        {
            CameraManager.instance.ToggleShipCamera();
            currentPlayerInput.SwitchView("PlayerView");

            currentPlayerInput.playerInput.actions["NitroThrust"].performed -= SteeringInputNitro;
            currentPlayerInput.playerInput.actions["ForwardThrust"].performed -= SteeringInputForward;
            currentPlayerInput.playerInput.actions["BackThrust"].performed -= SteeringInputBack;
            currentPlayerInput.playerInput.actions["OffThrust"].performed -= SteeringInputOff;

            InstanceFinder.TimeManager.OnTick -= OnTick;
            DisplayManager.instance.DisplayCloseEvent -= OnDisplayClosed;

            Ship.currentShip.leavePilot();
            shipMover.canMove = false;
            currentPlayerInput = null;
        }

        public override void Interact(GameObject player)
        {
            if (Ship.currentShip.Owner.IsActive && !Ship.currentShip.IsOwner) return; //do nothing if someone else owns the ship

            if (manager.Offer(UIDisplay, this))
            {
                currentPlayerInput = player.GetComponent<PlayerInputManager>();
                shipMover = Ship.currentShip.shipMover;
                CameraManager.instance.ToggleShipCamera();
                currentPlayerInput.SwitchView("SteeringView");

                currentPlayerInput.playerInput.actions["NitroThrust"].performed += SteeringInputNitro;
                currentPlayerInput.playerInput.actions["ForwardThrust"].performed += SteeringInputForward;
                currentPlayerInput.playerInput.actions["BackThrust"].performed += SteeringInputBack;
                currentPlayerInput.playerInput.actions["OffThrust"].performed += SteeringInputOff;
                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
                InstanceFinder.TimeManager.OnTick += OnTick;

                Ship.currentShip.changePilot();
                shipMover.canMove = true;
            }
        }

        private void SteeringInputNitro(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed) ChangeSteerState(SteerState.NITRO);
            Debug.Log("trying change nitro");
        }

        private void SteeringInputForward(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed) ChangeSteerState(SteerState.FORWARD);
            Debug.Log("trying change forward");
        }

        private void SteeringInputOff(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed) ChangeSteerState(SteerState.OFF);
            Debug.Log("trying change off");
        }

        private void SteeringInputBack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.performed) ChangeSteerState(SteerState.BACKWARD);
            Debug.Log("trying change back");
        }


        [ServerRpc]
        private void ChangeSteerState(SteerState newSteerState)
        {
            if (newSteerState == currentSteerState) return;
            currentSteerState = newSteerState;
            Ship.currentShip.shipMover.currentSteerState = currentSteerState;
        }

        private void OnTick()
        {

        }
    }
};