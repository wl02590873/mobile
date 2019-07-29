using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        LockCursor();
        xAxisClamp = 0.0f;
    }
    //讓滑鼠在螢幕內不會跑到視窗外
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public float speed = 10;
    [Header("跳躍高度")]
    public float JumpHeight = 10;
    [Header("滑鼠靈敏度")]
    public float mouseSpeed;
    //X軸最大90度
    private float xAxisClamp;

    float Gravity = 200;//重力

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
        playerCamera();
        playerMobile();
        #region 地面控制
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -transform.up, out hit, 20))
        {
            DistanceToGround = hit.distance;
            GroundNomal = hit.normal;
            if (DistanceToGround <= 2.0f)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                OnGround = true;
            }
            else
            {
                OnGround = false;
            }
        }
        #endregion
    }
    private void playerMobile()
    {
        #region 移動
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        transform.Translate(x, 0, z);
        #endregion

        #region 跳躍
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * 4000 * JumpHeight * Time.deltaTime);
        }
        #endregion

    }

    private void playerCamera()
    {
        #region 視角
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
        mouseX += xAxisClamp;
        if (xAxisClamp >= 90)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -90)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }

        transform.Rotate(-transform.right*mouseY);

        #endregion
    }
    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }

}
