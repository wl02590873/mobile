using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Photon.Pun;

public class HallUI : MonoBehaviourPunCallbacks
{
    [Header("標題UI")]
    public GameObject titleUI;
    [Header("文字訊息")]
    public GameObject textError;
    [Header("新增房間UI")]
    public GameObject NewRoom;
    [Header("加入房間UI")]
    public GameObject JoinUI;
    [Header("玩家名稱輸入UI")]
    public GameObject PlayerNameUI;

    private void Start()
    {
        titleUI.SetActive(true);
        textError.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            titleUI.SetActive(false);
            textError.SetActive(true);
        }
    }

    public void title(int UIint)
    {
        if (UIint == 0 || (Input.anyKey))//關閉起始畫面
        {
            titleUI.SetActive(false);
            textError.SetActive(true);
        }
        if (UIint == 1)//新增房間
        {
            NewRoom.SetActive(true);
            JoinUI.SetActive(false);
            PlayerNameUI.SetActive(true);
        }
        if (UIint == 2)//加入房間
        {
            JoinUI.SetActive(true);
            NewRoom.SetActive(false);
            PlayerNameUI.SetActive(true);
        }
    }
}
