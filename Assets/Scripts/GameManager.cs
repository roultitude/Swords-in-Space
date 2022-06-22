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


        int currentLevel;
        List<NetworkObject> CarryNetworkObjects;
        bool nextSceneLobby;

        private bool transitioning;

        private void Awake()
        {
            if (instance)
            {
                Debug.Log("More than 1 GameManager present at once! Destroying old GameManager.");
                Destroy(instance);
            }
            instance = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            GameObject userManager = Instantiate(userManagerPrefab);
            Spawn(userManager);
            SceneManager.OnClientPresenceChangeEnd += arg => OnNewSceneBroadcast(arg.Connection);
        }

        public void OnLoseGame()
        {
            Ship.currentShip.AllPlayerExitUI();
            GetCarryNetworkObjects(false, false);
            SceneLoadData sld = new SceneLoadData("LobbyScene") { ReplaceScenes = ReplaceOption.All, MovedNetworkObjects = CarryNetworkObjects.ToArray(), };
            InstanceFinder.NetworkManager.SceneManager.LoadGlobalScenes(sld);
        }

        public void OnLevelComplete()
        {
            OnLevelCompleteRPC();
            StartCoroutine(OnLevelCompleteStartCountdown());
        }


        [ServerRpc(RequireOwnership =false)]
        public void GoToLevel(string sceneName, bool bringShip = false, bool bringPlayers = true)
        {
            Ship.currentShip.AllPlayerExitUI();
            GetCarryNetworkObjects(bringShip,bringPlayers);
            SceneLoadData sld= new SceneLoadData(sceneName) { ReplaceScenes = ReplaceOption.All, MovedNetworkObjects = CarryNetworkObjects.ToArray(), };
            InstanceFinder.NetworkManager.SceneManager.LoadGlobalScenes(sld);
        }

        [TargetRpc]
        public void OnNewSceneBroadcast(NetworkConnection conn)
        {
            Debug.Log("entered new scene");
            OnNewSceneLoadEvent.Invoke();
        }

        [ObserversRpc]
        public void OnLevelCompleteRPC()
        {
            DisplayManager.instance.ShowLevelCompleteDisplay();
        }

        public IEnumerator OnLevelCompleteStartCountdown()
        {
            yield return new WaitForSeconds(10f);
            Ship.currentShip.AllPlayerExitUI();
            GetCarryNetworkObjects(true, true);
            SceneLoadData sld = new SceneLoadData("TempScene") { ReplaceScenes = ReplaceOption.All, MovedNetworkObjects = CarryNetworkObjects.ToArray(), };
            InstanceFinder.NetworkManager.SceneManager.LoadGlobalScenes(sld);
        }
        public void GetCarryNetworkObjects(bool includeShip, bool includePlayers)
        {
            CarryNetworkObjects = new List<NetworkObject>();
            foreach (User user in UserManager.instance.users) //get all connected users
            {
                CarryNetworkObjects.Add(user.GetComponent<NetworkObject>());
                if (includePlayers) 
                {
                    CarryNetworkObjects.Add(user.controlledPlayer.GetComponent<NetworkObject>());
                    user.controlledPlayer.DetachUsernameCanvas(false);
                }
            }
            if(includeShip) CarryNetworkObjects.Add(Ship.currentShip.GetComponentInParent<NetworkObject>()); //get current ship
            CarryNetworkObjects.Add(UserManager.instance.GetComponent<NetworkObject>());
        }
    }




}
