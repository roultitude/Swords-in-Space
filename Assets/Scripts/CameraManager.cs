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
        private Transform ship;

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            shipVCam.Follow = ship;
        }

        public void AttachToPlayer(Transform playerTransform)
        {
            playerVCam.Follow = playerTransform;
        }

        public void ToggleShipCamera()
        {
            int tmp = playerVCam.Priority;
            playerVCam.Priority = shipVCam.Priority;
            shipVCam.Priority = tmp;
        }
    }
}