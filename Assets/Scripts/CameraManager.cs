using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    ICinemachineCamera virtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        virtualCamera.Follow= transform;
    }

}
