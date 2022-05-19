using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class MenuManager : MonoBehaviour
    {

        [SerializeField]
        private MenuScript[] menus;

        public void OpenMenu(string menuName)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].menuName == menuName)
                {
                    menus[i].Open();
                }
                else if (menus[i].isOpened)
                {
                    menus[i].Close();
                }
            }
        }

        public void OpenMenu(MenuScript menu)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].isOpened)
                {
                    CloseMenu(menus[i]);
                }
            }

            menu.Open();
        }

        public void CloseMenu(MenuScript menu)
        {
            menu.Close();
        }
    }
}

