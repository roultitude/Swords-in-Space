using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class ManualEntitySpawner : NetworkBehaviour
    {
        [SerializeField]
        NetworkObject nobPrefab;
        public override void OnStartServer()
        {
            base.OnStartServer();
            GameObject toSpawn = Instantiate(nobPrefab.gameObject, transform.position, transform.rotation);
            Spawn(toSpawn);
        }
    }

}
