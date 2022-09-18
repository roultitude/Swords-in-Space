using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drumbeat : MonoBehaviour
{
    [SerializeField]
    public static float moveSpeed = 0.25f;

    [SerializeField]
    private Image image;
    private RectTransform rt;

    [SerializeField]
    private Sprite LSprite, RSprite;



    public string identity;
    void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
        Debug.Assert(rt != null);

    }
    // Update is called once per frame
    void Update()
    {
        rt.position -= new Vector3(0, moveSpeed, 0) * Time.deltaTime;
    }

    public void SetIdentity(string dir)
    {
        identity = dir;
        switch (dir)
        {
            case "L":
                image.sprite = LSprite;
                break;
            case "R":
                image.sprite = RSprite;
                break;
        }
    }
}
