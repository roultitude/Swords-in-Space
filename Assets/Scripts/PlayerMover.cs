using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

namespace SwordsInSpace
{
    public class PlayerMover : NetworkBehaviour
    {
        [SerializeField]
        private float speed, rotationSpeed;

        private Rigidbody2D rb;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            
        }
        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (!base.IsOwner) return; //guard for not owner

            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            Vector2 moveXY = new Vector2(moveX, moveY).normalized * speed * Time.fixedDeltaTime;
            if (moveXY.sqrMagnitude == 0) return;
            Vector2 newPos = new Vector2(rb.transform.position.x + moveXY.x, rb.transform.position.y + moveXY.y);
            rb.MovePosition(newPos);
            float angle = Mathf.Atan2(moveY, moveX) * Mathf.Rad2Deg;
            rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.fixedDeltaTime * rotationSpeed);
        }
    }
}