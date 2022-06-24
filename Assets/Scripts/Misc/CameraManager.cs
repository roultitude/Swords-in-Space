using UnityEngine;
using Cinemachine;


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

        private void Awake()
        {
            if (instance)
            {
                Debug.Log("There exists a CameraManager already! Destroying self!");
                Destroy(this);
            } else instance = this;
            GameManager.OnNewSceneLoadEvent += () =>
            {
                SetupShipCams();
            };
        }
        private void Start()
        {
            if (!Ship.currentShip) return;
            SetupShipCams(); //after ship is spawned
        }
        private void OnDisable()
        {
            Destroy(playerVCam);
        }
        public void SetupShipCams()
        {
            shipVCam.Follow = Ship.currentShip.shipExterior;
            weaponVCam.Follow = Ship.currentShip.shipExterior;
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
    }
}