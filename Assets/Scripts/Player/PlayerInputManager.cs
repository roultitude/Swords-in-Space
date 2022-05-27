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
        private bool awaitingDash;

        public struct MoveData
        {
            //public bool Interact;
            public float Horizontal;
            public float Vertical;
            //public float Anchor;
            public bool Dashing;
            public MoveData(float horizontal, float vertical, bool dashing
                //, float anchor
                )
            {
                //Interact = interact;
                Horizontal = horizontal;
                Vertical = vertical;
                //Anchor = anchor;
                Dashing = dashing;
            }
        }

        public struct ReconcileData
        {
            public Vector2 Position;
            public Quaternion Rotation;
            public Vector2 Velocity;
            public float AngularVelocity;
            //public float Anchor;
            public ReconcileData(Vector2 position, Quaternion rotation, Vector2 velocity, float angularVelocity
                //, float anchor
                )
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
                //Anchor = anchor;
            }
        }

        public struct PlayerReconcileData
        {
            public Vector2 LocalPosition;
            public Quaternion LocalRotation;
            public Vector2 Velocity;
            public float AngularVelocity;
            public PlayerReconcileData(Vector2 localPosition, Quaternion localRotation, Vector2 velocity, float angularVelocity
                //, float anchor
                )
            {
                LocalPosition = localPosition;
                LocalRotation = localRotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
                //Anchor = anchor;
            }
        }

        private void Awake()
        {
            mover = GetComponent<PlayerMover>();
            shipMover = Ship.currentShip.shipMover;
            interactor = GetComponent<PlayerInteractionManager>();
            rb = mover.rb;
        }
        private void OnDisable()
        {
            InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
        }
        private void OnEnable()
        {
            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
        }
        public override void OnStartClient()
        {
            base.OnStartClient();

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
            if (Input.GetKeyDown(KeyCode.E))
            {
                awaitingDash = true;
            }
        }

        void CheckInput(out MoveData md)
        {
            md = default;
            bool dashing = awaitingDash;
            if (awaitingDash) awaitingDash = false;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (horizontal == 0f && vertical == 0f) return;
            md = new MoveData(horizontal, vertical, dashing
                //, 1
                );
        }

        private void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                mover.Reconciliation(default, false);
                CheckInput(out MoveData md);
                if (Ship.currentShip.IsOwner) //add check for whether in steering
                {
                    shipMover.Reconciliation(default, false);
                    shipMover.Move(md, false);
                }
                mover.Move(md, false);
            }
            if (base.IsServer)
            {
                mover.Move(new MoveData(0f, 0f, false
                    //, 1f
                    ), true);
                shipMover.Move(new MoveData(0f, 0f, false
                    //, 1f
                    ), true);
            }
        }
        private void TimeManager_OnPostTick()
        {
            if (base.IsServer)
            {
                PlayerReconcileData rdMover = new PlayerReconcileData(mover.transform.localPosition, mover.transform.localRotation, rb.velocity, rb.angularVelocity
                    //, 1f
                    );
                ReconcileData rdShip = new ReconcileData(shipMover.transform.position, shipMover.transform.rotation, shipMover.rb.velocity, shipMover.rb.angularVelocity
                    //, 1f
                    );
                mover.Reconciliation(rdMover, true);
                shipMover.Reconciliation(rdShip, true);
            }
        }
    }

}

