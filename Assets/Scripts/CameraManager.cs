using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using FishNet.Object;
public class CameraManager : NetworkBehaviour
{
    ICinemachineCamera virtualCamera;
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            return;
        }
        

        virtualCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        virtualCamera.Follow = transform;
    }


}
