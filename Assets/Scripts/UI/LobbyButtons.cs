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

        [SerializeField]
        Sprite readyButtonReady, readyButtonNotReady;

        public void ToggleReady()
        {
            User.localUser.ServerSetIsReady(!User.localUser.isReady);
        }

        
        public void StartGame()
        {
            if (!UserManager.instance.allUsersReady) return;
            Ship.currentShip.AllPlayerExitUI();
            GameManager.instance.GoToLevelClient("GameScene",false, false);
            
        }

        public void UpdateReadyButtonGraphic()
        {
            if (!User.localUser) return;
            if (User.localUser.isReady) readyButton.sprite = readyButtonReady;
            else readyButton.sprite = readyButtonNotReady;
        }

        public void UpdateStartButtonGraphic()
        {
            startButton.GetComponent<Button>().interactable = UserManager.instance.allUsersReady;
        }

        private void Update()
        {
            UpdateReadyButtonGraphic();
            UpdateStartButtonGraphic();
        }
    }

}
