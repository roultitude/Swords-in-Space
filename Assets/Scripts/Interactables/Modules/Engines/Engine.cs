using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class Engine : Module
    {


        public override void OnInteract(GameObject player)
        {
            Debug.Log("Hello!" + gameObject.name);
        }
    }
};