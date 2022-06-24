using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class ManagerSpawner : NetworkBehaviour
    {
        [SerializeField]
        GameObject gameManagerPrefab, userManagerPrefab;
        public override void OnStartServer()
        {
            base.OnStartServer();
            if (!GameManager.instance)
            {
                GameObject gameManager = Instantiate(gameManagerPrefab);
                Spawn(gameManager);
            }
            if (!UserManager.instance)
            {
                GameObject userManager = Instantiate(userManagerPrefab);
                Spawn(userManager);
            }
        }
    }
}

