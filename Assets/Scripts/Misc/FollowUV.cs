using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUV : MonoBehaviour
{
    MeshRenderer mr;
    Material mat;
    [SerializeField]
    float parallax = 2f;
    [SerializeField]
    Vector2 initialOffset;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        mat = mr.material;
        mr.sortingOrder = -10;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 offset = mat.mainTextureOffset;
        offset.x = (initialOffset.x + transform.position.x) / (transform.localScale.x*parallax);
        offset.y = (initialOffset.y + transform.position.y) / (transform.localScale.y*parallax);
        mat.mainTextureOffset = offset;
    }
}
