using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class MenuScript : MonoBehaviour
    {
        public string menuName;
        public bool isOpened;
        public void Open()
        {
            isOpened = true;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            isOpened = false;
            gameObject.SetActive(false);
        }
    }
}
