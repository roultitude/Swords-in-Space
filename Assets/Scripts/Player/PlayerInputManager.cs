using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;
using UnityEngine.InputSystem;

namespace SwordsInSpace
{
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
    public class PlayerInputManager : NetworkBehaviour
    {

        public ShipMover shipMover;
        public PlayerInput playerInput;

        private PlayerMover mover;
        private PlayerInteractionManager interactor;
        private Rigidbody2D rb;
        private bool awaitingDash;
        private InputActionMap alwaysOnInput;
        private InputActionMap currentInputMap;




        private void Awake()
        {
            mover = GetComponent<PlayerMover>();
            shipMover = Ship.currentShip.shipMover;
            interactor = GetComponent<PlayerInteractionManager>();
            rb = mover.rb;
            playerInput = GetComponent<PlayerInput>();
            alwaysOnInput = playerInput.actions.FindActionMap("AlwaysOn");
            alwaysOnInput.Enable();
            currentInputMap = playerInput.actions.FindActionMap("PlayerView");
            GameManager.OnNewSceneLoadEvent += () =>
            {
                shipMover = Ship.currentShip.shipMover;
            };
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
            Debug.Log("onclientstart");
            if (!base.IsOwner)
            {
                playerInput.enabled = false;
                return;
            }
            playerInput.actions["Interact"].performed += context => interactor.Interact();
            playerInput.actions["Dash"].performed += context => { awaitingDash = true; };
            playerInput.actions["ExitUI"].performed += context => OnExitUI(context);
            playerInput.actions["NextLevel"].performed += context => { 
                if (context.performed) 
                {
                    WorldManager.currentWorld.levelComplete = true;
                    //GameManager.instance.OnLoseGame();
                    //GameManager.instance.GoToLevel("GameScene");
                } 
            };
        }

        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
                InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }

        void CheckInput(out MoveData md)
        {
            md = default;
            bool dashing = awaitingDash;
            if (awaitingDash) awaitingDash = false;
            float horizontal = playerInput.actions["Move"].ReadValue<Vector2>().x;
            float vertical = playerInput.actions["Move"].ReadValue<Vector2>().y;
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
                
                ReconcileData rdMover = new ReconcileData(mover.transform.position, mover.transform.rotation, rb.velocity, rb.angularVelocity
                    //, 1f
                    );
                ReconcileData rdShip = default;
                if (shipMover) rdShip = new ReconcileData(shipMover.transform.position, shipMover.transform.rotation, shipMover.rb.velocity, shipMover.rb.angularVelocity);
                mover.Reconciliation(rdMover, true);
                if (shipMover) shipMover.Reconciliation(rdShip, true);
            }
        }

        public void SwitchView(string viewName)
        {

            DisplayManager.instance.toggleMobilePlayerDisplay(viewName == "PlayerView");

            if (currentInputMap.name == "PlayerView")
            {
                if (viewName == "PlayerView")
                {
                    mover.canMove = true;
                    return;
                }
                currentInputMap.Disable(); // exiting player view
                currentInputMap = playerInput.actions.FindActionMap(viewName);
                currentInputMap.Enable();
                mover.canMove = false;
            }
            else
            {
                //if not playerview controls dont move player body
                if (viewName != currentInputMap.name)
                {
                    currentInputMap.Disable();
                    currentInputMap = playerInput.actions.FindActionMap(viewName);
                    StartCoroutine(DelayEnableByOne(currentInputMap));
                    //currentInputMap.Enable();
                }
                //currentInputMap = playerInput.actions.FindActionMap(viewName);

            }
            Debug.Log(currentInputMap.name + " " + playerInput.currentControlScheme);
        }

        IEnumerator DelayEnableByOne(InputActionMap map) //temp to prevent assert failure
        {
            yield return new WaitForEndOfFrame();
            map.Enable();
        }
        private void OnExitUI(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (obj.ReadValue<float>() == 1)
            {
                ExitUI();
            }
        }

        public void ExitUI()
        {
            Debug.Log("esc pressed");
            DisplayManager.instance.Close();
            mover.canMove = true;
        }
    }

}

