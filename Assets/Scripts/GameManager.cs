using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Scened;
using FishNet.Connection;

namespace SwordsInSpace
{
    public class GameManager : NetworkBehaviour
    {
        public GameObject userManagerPrefab;
        public static GameManager instance;
        public delegate void OnNewSceneLoad();
        public static OnNewSceneLoad OnNewSceneLoadEvent;

        private bool transitioning;

        private void Awake()
        {
            if (instance)
            {
                Debug.Log("More than 1 GameManager present at once! Destroying old GameManager.");
                Destroy(instance);
            }
            else
            {
                instance = this;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            GameObject userManager = Instantiate(userManagerPrefab);
            Spawn(userManager);
            SceneManager.OnClientPresenceChangeEnd += arg => OnNewSceneBroadcast(arg.Connection);
        }

        int currentLevel;
        List<NetworkObject> CarryNetworkObjects;

        
        [ServerRpc(RequireOwnership =false)]
        public void GoToNextLevel()
        {
            //if (transitioning) return;
            //transitioning = true;
            Ship.currentShip.powerDown();
            Ship.currentShip.powerUp(); //rmb to change
            GetCarryNetworkObjects();
            foreach (User user in UserManager.instance.users) //remove owner for all nobs
            {
                //user.controlledPlayer.RemoveOwnership();
            }
            //SceneLoadData sld = new SceneLoadData("GameScene") { ReplaceScenes = ReplaceOption.All, MovedNetworkObjects = CarryNetworkObjects.ToArray(), };
            SceneLoadData sldTemp = new SceneLoadData("TempScene") { ReplaceScenes = ReplaceOption.All, MovedNetworkObjects = CarryNetworkObjects.ToArray(), };
            InstanceFinder.NetworkManager.SceneManager.LoadGlobalScenes(sldTemp);
            //InstanceFinder.NetworkManager.SceneManager.LoadGlobalScenes(sld);
            foreach (User user in UserManager.instance.users) //remove owner for all nobs
            {
                //user.controlledPlayer.GiveOwnership(user.Owner);
            }
        }

        [TargetRpc]
        public void OnNewSceneBroadcast(NetworkConnection conn)
        {
            Debug.Log("entered new scene");
            OnNewSceneLoadEvent.Invoke();
            //transitioning = false;
        }

        public void GetCarryNetworkObjects()
        {
            CarryNetworkObjects = new List<NetworkObject>();
            foreach (User user in UserManager.instance.users) //get all connected users
            {
                CarryNetworkObjects.Add(user.GetComponent<NetworkObject>());
                CarryNetworkObjects.Add(user.controlledPlayer.GetComponent<NetworkObject>());
                user.controlledPlayer.DetachUsernameCanvas(false);
            }
            CarryNetworkObjects.Add(Ship.currentShip.GetComponentInParent<NetworkObject>()); //get current ship
            CarryNetworkObjects.Add(UserManager.instance.GetComponentInParent<NetworkObject>());
        }
    }




}
