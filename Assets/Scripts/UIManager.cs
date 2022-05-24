using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    /// <summary>
    /// There can only be 1 UI active at one time. Having a UI active stops all
    /// input commands until ESC/GUI is pressed. 
    /// </summary>
    // Start is called before the first frame update

    public static UIManager manager;  //There can only be one.
    GameObject currentUiElement;

    private void Start()
    {
        UIManager.manager = this;
    }

    public bool Offer(GameObject uiElement)
    {
        if (currentUiElement)
            return false;
        currentUiElement = uiElement;
        currentUiElement.SetActive(true);
        return true;
    }

    public bool Close()
    {
        if (!currentUiElement)
            return false;
        currentUiElement.SetActive(false);
        currentUiElement = null;
        return true;
    }
}
