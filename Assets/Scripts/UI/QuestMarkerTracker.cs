using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class QuestMarkerTracker : MonoBehaviour
    {
        [SerializeField]
        List<Transform> targets;

        Transform currentTarget;

        [SerializeField]
        RectTransform pointerRect;

        [SerializeField]
        float borderThickness;
        private void Awake()
        {
            targets = new List<Transform>();
            gameObject.SetActive(false);
        }

        public void Setup(Transform[] targets)
        {
            this.targets = new List<Transform>();
            this.targets.AddRange(targets);
            gameObject.SetActive(true);
        }

        public void AddTarget(Transform target)
        {
            this.targets.Add(target);
            gameObject.SetActive(true);
        }
        void Update()
        {
            if (!currentTarget)
            {
                if (targets.Count == 0) //no pending targets
                {
                    gameObject.SetActive(false);
                    return;
                }
                currentTarget = targets[0];
                foreach (Transform t in targets)
                {
                    if (Vector2.Distance(Ship.currentShip.shipExterior.transform.position, t.position) < Vector2.Distance(Ship.currentShip.shipExterior.transform.position, currentTarget.position))
                    {
                        currentTarget = t;
                    }
                    
                }
                targets.Remove(currentTarget);
            }
            gameObject.SetActive(true);
            Vector3 fromPos = Camera.main.transform.position;
            fromPos.z = 0;
            Vector3 dir = (currentTarget.position - fromPos).normalized;
            pointerRect.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, dir));

            Vector3 targetPosScreenSpace = Camera.main.WorldToScreenPoint(currentTarget.position);
            bool isOffScreen = targetPosScreenSpace.x <= borderThickness || targetPosScreenSpace.x >= Screen.width - borderThickness ||
                targetPosScreenSpace.y <= borderThickness || targetPosScreenSpace.y >= Screen.height - borderThickness;
            if (isOffScreen)
            {
                targetPosScreenSpace = new Vector3(Mathf.Clamp(targetPosScreenSpace.x, borderThickness, Screen.width - borderThickness),
                    Mathf.Clamp(targetPosScreenSpace.y, borderThickness, Screen.height - borderThickness), 0);
            }
            else
            {

            }
            pointerRect.position = targetPosScreenSpace;
        }
    }

}
