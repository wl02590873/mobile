using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoorialCamera : MonoBehaviour
{
    public GameObject target;
    public float xSpeed = 10.0f;

    void Update()
    {
        transform.RotateAround(target.transform.position, transform.up, Input.GetAxis("Mouse X") * xSpeed*Time.deltaTime);
        transform.RotateAround(target.transform.position, transform.right, Input.GetAxis("Mouse Y") * -xSpeed*Time.deltaTime);
    }
}
