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
    public class Steering : Module
    {

        [SerializeField]
        public GameObject UIDisplayPrefab;

        private DisplayManager manager;
        ShipMover shipMover;

        Vector2 turnAxis;
        PlayerInputManager currentPlayerInput;
        private GameObject UIDisplay;
        private void OnEnable()
        {
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

                DisplayManager.instance.DisplayCloseEvent += OnDisplayClosed;
                InstanceFinder.TimeManager.OnTick += OnTick;

                Ship.currentShip.ChangePilot();
                shipMover.canMove = true;
            }
        }


        private void OnTick()
        {

        }
    }
};