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


    public void title(int UIint)
    {
        if (UIint == 0)
        {
        titleUI.SetActive(false);
        }
        if (UIint == 1)
        {
            NewRoom.SetActive(true);
            JoinUI.SetActive(false);
        }
        if (UIint == 2)
        {
            JoinUI.SetActive(true);
            NewRoom.SetActive(false);
        }
    }
}
