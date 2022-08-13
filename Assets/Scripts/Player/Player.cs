using FishNet.Component.Prediction;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Connection;

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
            SetupPlayer();
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
        }
        public void SetupPlayer()
        {
            if (offset == 0) offset = playerCanvas.transform.localPosition.y;
            if (IsServer)
            {
                DetachUsernameCanvasRPC(true);
            }
            else DetachUsernameCanvas(true);

            ResetPlayerPosition();
            
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

        [ServerRpc(RequireOwnership = false)]
        public void ResetPlayerPosition(NetworkConnection con = null)
        {
            transform.position = Ship.currentShip.spawnTransform.position;
            SyncClientPosition(con,transform.position);
        }

        [TargetRpc]
        public void SyncClientPosition(NetworkConnection con, Vector3 pos)
        {
            transform.position = pos;
        }
        private void Update()
        {
            if(offset!= 0 && playerCanvas) playerCanvas.transform.position = new Vector2(transform.position.x, transform.position.y + offset);
        }

        [ObserversRpc(RunLocally =true)]
        public void DetachUsernameCanvasRPC(bool detach)
        {
            DetachUsernameCanvas(detach);
        }


        private void DetachUsernameCanvas(bool detach)
        {
            if (detach)
            {
                playerCanvas.transform.SetParent(null);
                playerCanvas.transform.rotation = Quaternion.identity;
            }
            else
            {
                playerCanvas.transform.SetParent(transform);
            }
        }
    } 
}
