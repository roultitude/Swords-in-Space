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
        public static GameManager instance;
        public delegate void OnNewSceneLoad();
        public static OnNewSceneLoad OnNewSceneLoadEvent;

        MessageDisplay messageDisplay;

        public int currentLevel = 0;
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
            messageDisplay = GetComponentInChildren<MessageDisplay>();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            //Debug.Log("Subscribing to onLoadEnd ");
            //SceneManager.OnLoadEnd += args => OnClientLoadEnd(LocalConnection);
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            SceneManager.OnClientPresenceChangeEnd += arg => {OnNewSceneBroadcast(arg.Connection); };
        }

        public void OnLoseGame()
        {
            Ship.currentShip.AllPlayerExitUI();
            GetCarryNetworkObjects(false, false);
            SceneLoadData sld = new SceneLoadData("LobbyScene") { ReplaceScenes = ReplaceOption.All, MovedNetworkObjects = CarryNetworkObjects.ToArray(), };
            InstanceFinder.NetworkManager.SceneManager.LoadGlobalScenes(sld);
            currentLevel = 0;
        }

        public void OnLevelComplete()
        {
            currentLevel++;
            OnLevelCompleteRPC();
            StartCoroutine(OnLevelCompleteStartCountdown());
        }


        [ServerRpc(RequireOwnership = false)]
        public void GoToLevel(string sceneName, bool bringShip = false, bool bringPlayers = true)
        {
            Ship.currentShip.AllPlayerExitUI();
            GetCarryNetworkObjects(bringShip, bringPlayers);
            SceneLoadData sld = new SceneLoadData(sceneName) { ReplaceScenes = ReplaceOption.All, MovedNetworkObjects = CarryNetworkObjects.ToArray(), };
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
            //DisplayManager.instance.ShowLevelCompleteDisplay();
        }

        public IEnumerator OnLevelCompleteStartCountdown()
        {
            MessageRPC("Boss defeated! Warping in 10 seconds!");
            yield return new WaitForSeconds(5f);
            MessageRPC("Boss defeated! Warping in 5 seconds!");
            yield return new WaitForSeconds(2f);
            MessageRPC("Boss defeated! Warping in 3 seconds!");
            yield return new WaitForSeconds(1f);
            MessageRPC("Boss defeated! Warping in 2 seconds!");
            yield return new WaitForSeconds(1f);
            MessageRPC("Boss defeated! Warping in 1 second!");
            Ship.currentShip.AllPlayerExitUI();
            yield return new WaitForSeconds(1f);
            GetCarryNetworkObjects(true, true);
            SceneLoadData sld = new SceneLoadData("TempScene") { ReplaceScenes = ReplaceOption.All, MovedNetworkObjects = CarryNetworkObjects.ToArray(), };
            InstanceFinder.NetworkManager.SceneManager.LoadGlobalScenes(sld);
            yield return new WaitForSeconds(15f);
            Debug.Log("levelTransitioning called");
            Ship.currentShip.LevelTransition();
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
                    user.controlledPlayer.DetachUsernameCanvasRPC(false);
                }
            }
            if (includeShip) CarryNetworkObjects.Add(Ship.currentShip.GetComponentInParent<NetworkObject>()); //get current ship
            CarryNetworkObjects.Add(UserManager.instance.GetComponent<NetworkObject>()); //always bring UserManager and GameManager
            CarryNetworkObjects.Add(GameManager.instance.GetComponent<NetworkObject>());
        }

        [ServerRpc(RequireOwnership =false)]
        void OnClientLoadEnd(NetworkConnection conn)
        {
            Debug.Log("Received Client Load End");
            OnNewSceneBroadcast(conn);
            
        }

        [ObserversRpc]
        public void MessageRPC(string message)
        {
            messageDisplay.DisplayMessage(message);
        }
    }




}
