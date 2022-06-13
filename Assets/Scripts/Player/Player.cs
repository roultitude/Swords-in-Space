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
        private Canvas playerCanvas;

        [SyncVar]
        public User controllingUser;

        [SyncVar]
        public float health;


        private float offset;
        private bool initialized;
        public override void OnStartClient()
        {
            base.OnStartClient();
            SetParent();
            offset = playerCanvas.transform.localPosition.y;
            StartCoroutine(checkForUsernameUpdate(controllingUser.username));
            playerCanvas.transform.parent = null;
            playerCanvas.transform.rotation = Quaternion.identity;

            if (!IsOwner)
            {
                return;
            }
            CameraManager.instance.AttachToPlayer(transform);

        }

        public void SetParent()
        {
            this.transform.parent = Ship.currentShip.spawnTransform;
        }

        public void updateUsernameText(string username)
        {
            usernameText.text = username;
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
            if(offset!= 0) playerCanvas.transform.position = new Vector2(transform.position.x, transform.position.y + offset);
        }
    } 
}
