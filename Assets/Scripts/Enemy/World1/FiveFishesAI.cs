using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class FiveFishesAI : EnemyAI
    {
        [SerializeField]
        private float turnSpeed;
        [SerializeField]
        Transform fiveFishesSprites;
        [SerializeField]
        SpriteRenderer sprite;

        private Rigidbody2D rb;
        
        // Start is called before the first frame update
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        new void Start()
        {
            base.Start();
            //gameObject.GetComponent<Pathfinding.AIDestinationSetter>().target = Ship.currentShip.transform;
            
        }

        // Update is called once per frame
        void Update()
        {
            sprite.flipY = transform.rotation.eulerAngles.z < 180;
            fiveFishesSprites.localRotation = Quaternion.Euler(0,0,fiveFishesSprites.localRotation.eulerAngles.z + turnSpeed * Time.deltaTime);
            //rb.AddTorque(turnSpeed * Time.deltaTime);
        }
    }
}
