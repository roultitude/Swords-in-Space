using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public abstract class Engine : Module
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public override void Interact(GameObject player)
        {
            Debug.Log("Hello!");
        }
    }
};