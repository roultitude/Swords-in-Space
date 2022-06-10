using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SwordsInSpace
{
    public class DisplayManager : MonoBehaviour
    {

        /// <summary>
        /// There can only be 1 UI active at one time. Having a UI active stops all
        /// input commands until ESC/GUI is pressed. 
        /// </summary>

        [SerializeField]
        private GameObject mobilePlayerDisplay;

        public static DisplayManager instance;  //There can only be one.

        GameObject currentUIDisplay;

        public DisplayClose DisplayCloseEvent;
        public delegate void DisplayClose();

        private void Awake()
        {
            DisplayManager.instance = this;
        }

        public bool Offer(GameObject display, Interactable interactable = null)
        {
            if (currentUIDisplay) return false;
            currentUIDisplay = display;
            currentUIDisplay.SetActive(true);
            currentUIDisplay.GetComponent<Display>().Setup(interactable);
            return true;
        }

        public bool Close()
        {
            if (!currentUIDisplay)
                return false;
            currentUIDisplay.SetActive(false);
            currentUIDisplay = null;
            DisplayCloseEvent.Invoke();
            return true;
        }

        public void toggleMobilePlayerDisplay(bool on)
        {
            if (!mobilePlayerDisplay) return;
            mobilePlayerDisplay.SetActive(on);
        }
    }

}