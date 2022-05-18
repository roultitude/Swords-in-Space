using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private List<Interactable> interactables;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();
        if (otherInteractable)
        {
            interactables.Add(otherInteractable);
        }
        Debug.Log(other.name);
        Debug.Log(other.tag);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable otherInteractable = other.gameObject.GetComponentInParent<Interactable>();

        if (otherInteractable)
        {
               interactables.Remove(otherInteractable);
        }

    }


}
