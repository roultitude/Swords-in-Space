using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMarkerTracker : MonoBehaviour
{
    [SerializeField]
    Transform target;
    
    [SerializeField]
    RectTransform pointerRect;

    [SerializeField]
    float borderThickness;
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    void Setup(Transform target)
    {
        this.target = target;
        gameObject.SetActive(true);
    }
    void Update()
    {
        if (!target) 
        {
            gameObject.SetActive(false); 
            return; 
        } 
        Vector3 fromPos = Camera.main.transform.position;
        fromPos.z = 0;
        Vector3 dir = (target.position - fromPos).normalized;
        pointerRect.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right,dir));

        Vector3 targetPosScreenSpace = Camera.main.WorldToScreenPoint(target.position);
        bool isOffScreen = targetPosScreenSpace.x <= borderThickness || targetPosScreenSpace.x >= Screen.width - borderThickness || 
            targetPosScreenSpace.y <= borderThickness|| targetPosScreenSpace.y >= Screen.height - borderThickness;
        if (isOffScreen)
        {
            targetPosScreenSpace = new Vector3(Mathf.Clamp(targetPosScreenSpace.x,borderThickness,Screen.width - borderThickness),
                Mathf.Clamp(targetPosScreenSpace.y,borderThickness,Screen.height - borderThickness),0);
        } else
        {
            
        }
        pointerRect.position = targetPosScreenSpace;
    }
}
