using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using TMPro;
namespace SwordsInSpace
{
    public class MenuNetworkInitializer : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField usernameInput;
        [SerializeField]
        private UserData userData;
        private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = InstanceFinder.NetworkManager;
            InstanceFinder.ServerManager.OnServerConnectionState += ServerStarted;
            if(userData.username == "")
            {
                userData.setUsername("Player " + Random.Range(0, 10000).ToString());
            }
            usernameInput.text = userData.username;
        }

        public void SetPort(string port)
        {
            int portInt = int.Parse(port);
            if(portInt > 65535 || portInt < 0)
            {
                Debug.LogError("port out of range");
                return;
            }
            InstanceFinder.TransportManager.Transport.SetPort((ushort) portInt);
        }

        public void SetClientIP(string ip)
        {
            InstanceFinder.TransportManager.Transport.SetClientAddress(ip);
        }

        public void SetHostIP(string ip)
        {
            
            InstanceFinder.TransportManager.Transport.SetServerBindAddress(ip, FishNet.Transporting.IPAddressType.IPv4);
        }

        public void JoinGame()
        {
            if (networkManager == null) return;
            networkManager.ClientManager.StartConnection();
        }

        public void HostGame()
        {
            if (networkManager == null) return;
            networkManager.ServerManager.StartConnection();
        }

        private void ServerStarted(FishNet.Transporting.ServerConnectionStateArgs args)
        {
            Debug.Log("kekw");
            SceneLoadData sld = new SceneLoadData("GameScene") { ReplaceScenes = ReplaceOption.All };
            networkManager.SceneManager.LoadGlobalScenes(sld);
            networkManager.ClientManager.StartConnection();
        }
    }
}