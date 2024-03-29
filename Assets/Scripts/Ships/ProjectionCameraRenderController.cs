using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionCameraRenderController : MonoBehaviour
{
    Camera cam;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
    }

    private void Update()
    {
#if (!UNITY_SERVER || UNITY_EDITOR)
        cam.Render();
#endif
    }
}
