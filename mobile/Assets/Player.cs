using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        ClampXAxisRotationToValue(0);
        LockCursor();
        xAxisClamp = 0.0f;
    }
    //讓滑鼠在螢幕內不會跑到視窗外
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public float speed = 10;

    [Header("滑鼠靈敏度")]
    public float mouseSpeed;
    //X軸最大90度
    private float xAxisClamp;
    void Start()
    {
    }

    void Update()
    {
        playerCamera();
    }
    private void playerCamera()
    {
        #region 視角
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        mouseY += xAxisClamp;

        if (xAxisClamp > 90)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90);
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
