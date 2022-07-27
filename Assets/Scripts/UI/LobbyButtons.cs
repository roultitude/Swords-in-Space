using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SwordsInSpace
{
    public class LobbyButtons : MonoBehaviour
    {
        [SerializeField]
        Image readyButton, startButton;


        public void ToggleReady()
        {
            User.localUser.ServerSetIsReady(!User.localUser.isReady);
        }

        
        public void StartGame()
        {
            if (!UserManager.instance.allUsersReady) return;
            GameManager.instance.GoToLevel("GameScene",false, true);
            
        }

        public void UpdateReadyButtonGraphic()
        {
            if (!User.localUser) return;
            readyButton.color = User.localUser.isReady? Color.green : Color.red;
        }

        public void UpdateStartButtonGraphic()
        {
            startButton.color = UserManager.instance.allUsersReady? Color.green : Color.red;
        }

        private void Update()
        {
            UpdateReadyButtonGraphic();
            UpdateStartButtonGraphic();
        }
    }

}
