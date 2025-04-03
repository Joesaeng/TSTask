using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPosX;

    [Range(0f, 1f)]
    public float parallaxFactor;
    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        startPosX = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = cam.position.x * (1 - parallaxFactor);
        float distance = cam.position.x * parallaxFactor;

        transform.position = new Vector3(startPosX + distance, transform.position.y, transform.position.z);

        if (temp > startPosX + (length * 0.5f))
            startPosX += length;
        else if (temp < startPosX - (length * 0.5f))
            startPosX -= length;
    }
}
