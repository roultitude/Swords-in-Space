using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pathfinding.AIDestinationSetter;

namespace SwordsInSpace
{
    public class PlayerTargetter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            gameObject.GetComponent<Pathfinding.AIDestinationSetter>().target = Ship.currentShip.transform;

        }

    }
};
