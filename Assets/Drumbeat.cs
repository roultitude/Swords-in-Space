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
    private Color Lcolor, Rcolor;



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

        if (rt.position.y < -2000)
        {
            Destroy(this);
        }
    }

    public void SetIdentity(string dir)
    {
        identity = dir;
        switch (dir)
        {
            case "L":
                image.color = Lcolor;
                break;
            case "R":
                image.color = Rcolor;
                break;
        }
    }
}
