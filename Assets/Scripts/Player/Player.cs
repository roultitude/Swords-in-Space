using FishNet.Component.Prediction;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace SwordsInSpace
{
    public class Player : NetworkBehaviour
    {
        [SerializeField]
        private Transform spawnTransform;

        [SerializeField]
        private TextMeshProUGUI usernameText;

        [SerializeField]
        private GameObject playerCanvas;

        [SyncVar]
        public User controllingUser;

        [SyncVar]
        public float health;


        public float offset;
        private bool initialized;
        //private Canvas playerCanvas;
        public override void OnStartClient()
        {
            base.OnStartClient();
            
            SetParent();
            //playerCanvas = Instantiate(playerCanvasPrefab).GetComponent<Canvas>();
            StartCoroutine(checkForUsernameUpdate(controllingUser.username));

            if (!IsOwner)
            {
                return;
            }
            //SetupPlayer();
        }

        public void SetupPlayer()
        {
            if (offset == 0) offset = playerCanvas.transform.localPosition.y;
            DetachUsernameCanvas(true);
            transform.position = Ship.currentShip.spawnTransform.position;
            
            if (!IsOwner) return;
            CameraManager.instance.AttachToPlayer(transform);
        }
        public void SetParent()
        {
            //this.transform.parent = Ship.currentShip.spawnTransform;
        }

        public void updateUsernameText(string username)
        {
            playerCanvas.GetComponentInChildren<TextMeshProUGUI>().text = username;
        }

        IEnumerator checkForUsernameUpdate(string username)
        {
            if(username == "")
            {
                yield return new WaitForSeconds(1f);
                StartCoroutine(checkForUsernameUpdate(controllingUser.username));
            } else
            {
                updateUsernameText(controllingUser.username);
                yield return null;
            }
        }

        private void Update()
        {
            if(offset!= 0 && playerCanvas) playerCanvas.transform.position = new Vector2(transform.position.x, transform.position.y + offset);
        }

        [ObserversRpc(RunLocally =true)]
        public void DetachUsernameCanvas(bool detach)
        {
            if (detach)
            {
                playerCanvas.transform.SetParent(null);
                playerCanvas.transform.rotation = Quaternion.identity;
            } else
            {
                playerCanvas.transform.SetParent(transform);
            }


        }
    } 
}
