using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MenuUI : MonoBehaviour
{
    [Header("ESC選單UI")]
    public GameObject ESCUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ESCUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

    }

    public void ESCMunuUi(int UIint)
    {
        if (UIint == 0)//關閉起始畫面
        {
            Cursor.lockState = CursorLockMode.None;
            Application.Quit();
        }
        else if (UIint == 1)//關閉起始畫面
        {
            ESCUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (UIint == 2)//關閉起始畫面
        {
            Cursor.lockState = CursorLockMode.None;
            PhotonNetwork.LeaveRoom();//離開房間
            PhotonNetwork.LoadLevel("遊戲大廳");//返回大廳

        }
    }
}
