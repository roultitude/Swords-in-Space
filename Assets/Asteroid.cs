using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class Asteroid : NetworkBehaviour
{
    // Start is called before the first frame update

    float shotSpeed = 1f;

    public void Setup(float shotSpeed)
    {
        this.shotSpeed = shotSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * Time.deltaTime * shotSpeed;
    }
}
