﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayer : MonoBehaviour
{
    [Header("行星")]
    public GameObject Planet;
    public float speed = 10;
    [Header("跳躍高度")]
    public float JumpHeight = 10;

    float Gravity = 100;//重力

    bool OnGround = false;//地面

    float DistanceToGround;//地面距離
    Vector3 GroundNomal;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;//鎖定Rigidbody旋轉
    }


    void Update()
    {
        #region 移動
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        transform.Translate(x, 0, z);
        #endregion

        #region 視角
        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(0, 150 * Time.deltaTime, 0);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(0, -150 * Time.deltaTime, 0);
        }
        #endregion

        #region 跳躍
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * 4000 * JumpHeight * Time.deltaTime);
        }
        #endregion
        #region 地面控制
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10))
        {
            DistanceToGround = hit.distance;
            GroundNomal = hit.normal;
            if (DistanceToGround <= 2.0f)
            {
                OnGround = true;
            }
            else
            {
                OnGround = false;
            }
        }
        #endregion

        #region 重力和旋轉
        Vector3 GravDirection = (transform.position - Planet.transform.position).normalized;//重力方向
        if (OnGround == false)
        {
            rb.AddForce(GravDirection * -Gravity);//添加重力方向
        }

        Quaternion toRotation = Quaternion.FromToRotation(transform.up, GroundNomal) * transform.rotation;
        transform.rotation = toRotation;
        #endregion
    }
}
