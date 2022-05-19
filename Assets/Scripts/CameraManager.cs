using UnityEngine;
using Cinemachine;


namespace SwordsInSpace
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        [SerializeField]
        private CinemachineVirtualCamera playerVCam;
        [SerializeField]
        private CinemachineVirtualCamera shipVCam;
        [SerializeField]
        private Transform ship;

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            shipVCam.Follow = ship;
        }

        public void AttachToPlayer(GameObject player)
        {
            playerVCam.Follow = player.transform;
        }

        public void ToggleShipCamera()
        {
            int tmp = playerVCam.Priority;
            playerVCam.Priority = shipVCam.Priority;
            shipVCam.Priority = tmp;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ToggleShipCamera();
            }
        }
    }
}