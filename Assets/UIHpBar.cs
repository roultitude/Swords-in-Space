using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SwordsInSpace
{
    public class UIHpBar : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField]
        GameObject Foreground;
        public void Resize(float fillamt)
        {
            Foreground.GetComponent<Image>().fillAmount = fillamt;
        }
    }
};
