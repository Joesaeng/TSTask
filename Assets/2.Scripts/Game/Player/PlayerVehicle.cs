using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
    [Header("Truck Movement")]
    public Rigidbody2D rb;
    public Transform wheelBack;
    public Transform wheelFront;
    public float moveSpeed;
    public float accelation;
    [Header("적당한 바퀴 회전속도를 찾기 위한 Radius")]
    public float wheelRadius;

    [Header("Box & PlayerCharacter")]
    public GameObject boxPrefab;
    public Transform extraBoxPoint;
    public Transform boxTrans;
    public GameObject playerCharacter;
    public GameObject belowBox;
    private int boxCount;

    private void OnEnable()
    {
        Init();
    }

    private void FixedUpdate()
    {
        // rb의 velocity.x크기를 통해 wheel의 회전을 돌린다
        rb.AddForce(Vector2.right * accelation);
        if (rb.velocity.x > moveSpeed)
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        WheelRotator(wheelBack, rb.velocity.x);
        WheelRotator(wheelFront, rb.velocity.x);
    }

    private void WheelRotator(Transform wheel, float truckSpeed)
    {
        float angularSpeed = truckSpeed / wheelRadius;
        wheel.Rotate(new Vector3(0, 0, -angularSpeed));
    }

    public void Init()
    {
        boxCount = 0;
        playerCharacter.GetComponent<IOnTheTruck>().Activate(null, null);
    }

    public void NewBox()
    {
        if (boxCount >= 5)
            return;
        GameObject box = ObjectManager.Ins.Spawn(boxPrefab, boxTrans);
        box.name = $"Box_{boxCount}";
        Vector3 spawnPos = belowBox == null ? extraBoxPoint.position : belowBox.transform.position;

        box.transform.position = spawnPos;

        // box.SetActive(true);
        var boxOntheTruck = box.GetComponent<IOnTheTruck>();
        var belowBoxOntheTruck = belowBox == null ? null : belowBox.GetComponent<IOnTheTruck>();

        boxOntheTruck.Activate(playerCharacter.GetComponent<IOnTheTruck>(), belowBoxOntheTruck);

        belowBox = box;
        boxCount++;
    }
}
