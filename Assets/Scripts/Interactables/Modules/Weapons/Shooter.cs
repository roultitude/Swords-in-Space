using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{
    public class Shooter : NetworkBehaviour
    {
        // Start is called before the first frame update

        public Vector2 turnAxis;



        [SerializeField]
        private float rotationMin, rotationMax;

        private float rotationSpeed;

        public void UpdateRotSpeed(float rotSpeed)
        {
            this.rotationSpeed = rotSpeed;
        }

        private void Update()
        {
            if (!IsServer) return; //only rotate on server
            transform.localRotation = Quaternion.Euler(0, 0,
                Mathf.Clamp(transform.localRotation.eulerAngles.z - rotationSpeed * turnAxis.x * Time.fixedDeltaTime, rotationMin, rotationMax - 0.25f));
        }










    }
};
