using UnityEngine;
using Cinemachine;
using System.Collections;

namespace SwordsInSpace
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager instance;

        [SerializeField]
        private CinemachineVirtualCamera playerVCam;
        [SerializeField]
        private CinemachineVirtualCamera shipVCam;
        [SerializeField]
        private CinemachineVirtualCamera weaponVCam;
        [SerializeField]
        private MeshRenderer bgBack, bgMid, bgFront;

        bool bgTexturesUpdated = false;
        private void Awake()
        {
            if (instance)
            {
                Debug.Log("There exists a CameraManager already! Destroying self!");
                Destroy(this);
            } else instance = this;
            GameManager.OnNewSceneLoadEvent += SetupShipCams;
        }
        private void Start()
        {
            if (!Ship.currentShip) return;
            SetupShipCams(); //after ship is spawned
        }
        private void OnDisable()
        {
            GameManager.OnNewSceneLoadEvent -= SetupShipCams;
            Destroy(playerVCam);
        }
        public void SetupShipCams()
        {
            shipVCam.Follow = Ship.currentShip.shipExterior;
            weaponVCam.Follow = Ship.currentShip.shipExterior;
            StartCoroutine(UpdateBGTextures());
        }
        public void AttachToPlayer(Transform playerTransform)
        {
            playerVCam.Follow = Ship.currentShip.playerTracker;
            playerVCam.transform.SetParent(Ship.currentShip.shipInteriorView);
            playerVCam.transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        public void ToggleShipCamera()
        {
            int tmp = playerVCam.Priority;
            playerVCam.Priority = shipVCam.Priority;
            shipVCam.Priority = tmp;
        }

        public void ToggleWeaponCamera()
        {
            int tmp = playerVCam.Priority;
            playerVCam.Priority = weaponVCam.Priority;
            weaponVCam.Priority = tmp;

        }

        IEnumerator UpdateBGTextures()
        {
            while (!bgTexturesUpdated)
            {
                if (WorldManager.currentWorld)
                {
                    bgBack.material.mainTexture = WorldManager.currentWorld.backgroundTextureBack;
                    bgMid.material.mainTexture = WorldManager.currentWorld.backgroundTextureMid;
                    bgFront.material.mainTexture = WorldManager.currentWorld.backgroundTextureFront;
                    bgTexturesUpdated = true;
                }
                yield return 0;
            }
        }
    }
}