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
        [SerializeField]
        private GameObject levelCompleteDisplay;
        public static DisplayManager instance;  //There can only be one.

        GameObject currentUIDisplay;

        public DisplayClose DisplayCloseEvent;
        public delegate void DisplayClose();

        private void Awake()
        {
            if (instance) //override singleton, destroy oldest
            {
                Destroy(instance);
            }
            instance = this;
            mobilePlayerDisplay = Instantiate(mobilePlayerDisplay);
            levelCompleteDisplay = Instantiate(levelCompleteDisplay);
            levelCompleteDisplay.SetActive(false);
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
            StartCoroutine(CloseUI());
            return true;
        }

        public void ToggleMobilePlayerDisplay(bool on)
        {
            if (!mobilePlayerDisplay) return;
            mobilePlayerDisplay.SetActive(on);
        }

        public void ShowLevelCompleteDisplay()
        {
            if (!levelCompleteDisplay) return;
            levelCompleteDisplay.SetActive(true);
        }

        IEnumerator CloseUI() //delay by 1 frame to avoid assertion failure on use of Exit button in UI.
        {
            yield return 0;
            currentUIDisplay.SetActive(false);
            currentUIDisplay = null;
            DisplayCloseEvent.Invoke();
        }
    }

}
