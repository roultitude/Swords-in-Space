using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace {
    public abstract class Interactable : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

         public abstract void Interact(GameObject player);



    }
};
