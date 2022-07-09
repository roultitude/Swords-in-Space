using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SwordsInSpace
{
    public class ScrollUV : MonoBehaviour
    {
        MeshRenderer mr;
        Material mat;
        [SerializeField]
        Vector2 scrollDir;
        [SerializeField]
        float scrollSpeed;
        [SerializeField]
        Texture scrollTexture;
        private void Awake()
        {
            mr = GetComponent<MeshRenderer>();
            mat = mr.material;
            mr.sortingOrder = -10;
            scrollDir = scrollDir.normalized;
            mat.mainTexture = scrollTexture;
        }
        // Update is called once per frame
        void Update()
        {
            Vector2 offset = mat.mainTextureOffset;
            offset.x += scrollDir.x * scrollSpeed * Time.deltaTime;
            offset.y += scrollDir.y * scrollSpeed * Time.deltaTime;
            mat.mainTextureOffset = offset;
        }
    }
}
