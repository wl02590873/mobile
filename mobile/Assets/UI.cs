using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("標題UI")]
    public GameObject titleUI;
    [Header("新增房間UI")]
    public GameObject NewRoom;
    [Header("加入房間UI")]
    public GameObject JoinUI;
    [Header("玩家名稱輸入UI")]
    public GameObject PlayerNameUI;


    public void title(int UIint)
    {
        if (UIint == 0)
        {
        titleUI.SetActive(false);
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
