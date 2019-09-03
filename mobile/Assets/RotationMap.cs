using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMap : MonoBehaviour
{

    void Start()
    {
        InvokeRepeating("TimeRotation90", 30,30);
    }

    void Update()
    {
        
    }

    public void TimeRotation90()
    {
        gameObject.transform.Rotate (90, 0, 0);
    }
}
