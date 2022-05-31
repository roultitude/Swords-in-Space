using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;

public class ShipInteriorSyncer : NetworkBehaviour
{
    public Transform syncTarget;

    private void Awake()
    {
        //InstanceFinder.TimeManager.OnFixedUpdate += OnTick;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
    }
    private void OnTick()
    {
        
    }
    private void Update()
    {
        transform.position = syncTarget.position;
        transform.rotation = syncTarget.rotation;
    }
}
