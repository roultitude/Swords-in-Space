using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;

namespace SwordsInSpace
{
    public class MenuNetworkInitializer : MonoBehaviour
    {
        private NetworkManager networkManager;
        private void Awake()
        {
            networkManager = InstanceFinder.NetworkManager;
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
            networkManager.ClientManager.StartConnection();
            SceneLoadData sld = new SceneLoadData("LobbyScene") {ReplaceScenes = ReplaceOption.All};
            networkManager.SceneManager.LoadGlobalScenes(sld);
        }
    }
}