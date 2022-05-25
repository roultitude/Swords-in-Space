using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;

namespace SwordsInSpace
{
    public class PlayerInputManager : NetworkBehaviour
    {

        public ShipMover shipMover;

        private PlayerMover mover;
        private PlayerInteractionManager interactor;
        private Rigidbody2D rb;

        public struct MoveData
        {
            //public bool Interact;
            public float Horizontal;
            public float Vertical;
            public float Anchor;
            public MoveData(float horizontal, float vertical, float anchor)
            {
                //Interact = interact;
                Horizontal = horizontal;
                Vertical = vertical;
                Anchor = anchor;
            }
        }

        public struct ReconcileData
        {
            public Vector2 Position;
            public Quaternion Rotation;
            public Vector2 Velocity;
            public float AngularVelocity;
            public ReconcileData(Vector2 position, Quaternion rotation, Vector2 velocity, float angularVelocity)
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
            }
        }

        private void Awake()
        {
            mover = GetComponent<PlayerMover>();
            shipMover = Ship.currentShip.shipMover;
            interactor = GetComponent<PlayerInteractionManager>();
            rb = mover.rb;

            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
        }

        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
                InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }

        private void Update()
        {
            if (!base.IsOwner) return; //guard for not owner
            if (Input.GetKeyDown("f"))
            {
                interactor.Interact();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.manager.Close();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
               CameraManager.instance.ToggleShipCamera();
               mover.canMove = !mover.canMove;
               shipMover.canMove = !shipMover.canMove;
            }
        }

        void CheckInput(out MoveData md)
        {
            md = default;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (horizontal == 0f && vertical == 0f) return;
            md = new MoveData(horizontal, vertical, 1);
        }

        private void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                mover.Reconciliation(default, false);
                CheckInput(out MoveData md);
                mover.Move(md, false);
                if (Ship.currentShip.IsOwner) //add check for whether in steering
                {
                    shipMover.Reconciliation(default, false);
                    shipMover.Move(md, false);
                }
            }
            if (base.IsServer)
            {
                mover.Move(new MoveData(0f, 0f, 1f), true);
                shipMover.Move(new MoveData(0f, 0f, 1f), true);
            }
        }
        private void TimeManager_OnPostTick()
        {
            if (base.IsServer)
            {
                ReconcileData rdMover = new ReconcileData(mover.transform.position, mover.transform.rotation, rb.velocity, rb.angularVelocity);
                ReconcileData rdShip = new ReconcileData(shipMover.transform.position, shipMover.transform.rotation, shipMover.rb.velocity, shipMover.rb.angularVelocity);
                mover.Reconciliation(rdMover, true);
                shipMover.Reconciliation(rdShip, true);
            }
        }
    }

}

