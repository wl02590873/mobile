using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMap : MonoBehaviour
{

    void Start()
    {
        InvokeRepeating("TimeRotation90", 0,5);
    }

    void Update()
    {
        
    }

    public void TimeRotation90()
    {
        gameObject.transform.Rotate (0, 0, 0);
    }
}
