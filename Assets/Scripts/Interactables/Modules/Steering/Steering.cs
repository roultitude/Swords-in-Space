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
        public GameObject UIDisplayPrefab;

        [SyncVar]
        public SteerState currentSteerState;
        private DisplayManager manager;
        ShipMover shipMover;

        Vector2 turnAxis;
        PlayerInputManager currentPlayerInput;
        private GameObject UIDisplay;
        private void OnEnable()
        {
            currentSteerState = SteerState.OFF;
            GameManager.OnNewSceneLoadEvent += SetupUI;
        }
        private void OnDisable()
        {
            GameManager.OnNewSceneLoadEvent -= SetupUI;
            InstanceFinder.TimeManager.OnTick -= OnTick;
        }

        void SetupUI()
        {
            UIDisplay = Instantiate(UIDisplayPrefab, Vector3.zero, Quaternion.identity);
            manager = DisplayManager.instance;
            UIDisplay.SetActive(false);
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

            Ship.currentShip.LeavePilot();
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

                Ship.currentShip.ChangePilot();
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