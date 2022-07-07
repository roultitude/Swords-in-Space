using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class UILoadingBar : MonoBehaviour
    {
        [SerializeField]
        RectTransform bar;

        public void UpdateGraphic(float percentageFull)
        {
            bar.localScale = new Vector3(percentageFull, 1, 1);
        }
    }

}
