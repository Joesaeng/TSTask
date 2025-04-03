using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform wheelBack;
    public Transform wheelFront;
    public float moveSpeed;
    public float accelation;
    [Header("적당한 바퀴 회전속도를 찾기 위한 Radius")]
    public float wheelRadius;

    private void FixedUpdate()
    {
        // rb의 velocity.x크기를 통해 wheel의 회전을 돌린다
        rb.AddForce(Vector2.right * accelation);
        if(rb.velocity.x > moveSpeed)
            rb.velocity = new Vector2 (moveSpeed, 0);

        WheelRotator(wheelBack, rb.velocity.x);
        WheelRotator(wheelFront, rb.velocity.x);
    }

    private void WheelRotator(Transform wheel, float truckSpeed)
    {
        float angularSpeed = truckSpeed / wheelRadius;
        wheel.Rotate(new Vector3(0, 0, -angularSpeed));
    }
}
