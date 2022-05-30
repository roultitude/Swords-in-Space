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
        [SerializeField]
        private Transform ship;

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            shipVCam.Follow = Ship.currentShip.shipExterior;
            weaponVCam.Follow = Ship.currentShip.shipExterior;
        }
        public void AttachToPlayer(Transform playerTransform)
        {
            playerVCam.Follow = playerTransform;
            playerVCam.transform.SetParent(Ship.currentShip.shipInterior);
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