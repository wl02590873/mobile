using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region 欄位，屬性
    [Header("輸出文字")]
    public Text textPrint;
    [Header("輸出文字")]
    public InputField playerIF;//輸入欄位:玩家名稱
    public InputField roomCreateIF;//輸入欄位:建立房間
    public InputField roomJoinIF;//輸入欄位:加入房間 
    public Button BtnCreate, BtnJoin;


    public string namePlayer, nameCreateRoom, nameJoinRoom;//字串:玩家名稱，建房名稱，加入房間名稱

    //屬性:給UI設定
    public string NamePlayer
    {
        get => namePlayer;
        set
        {
            namePlayer = value;
            PhotonNetwork.NickName = namePlayer;
        }
    }
    public string NameCreateRoom { get => nameCreateRoom; set => nameCreateRoom = value; }
    public string NameJoinRoom { get => nameJoinRoom; set => nameJoinRoom = value; }

    #endregion
    private void Start()
    {
        Screen.SetResolution(800, 450, false);
        Connect();
    }

    /// <summary>
    /// 連線到伺服器
    /// </summary>
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();//連線到伺服器
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        textPrint.text = "連線成功";
        PhotonNetwork.JoinLobby();//連線成功時加入大廳
    }

    public void BtnCreateRoom()
    {
        PhotonNetwork.CreateRoom(NameCreateRoom, new RoomOptions { MaxPlayers = 20 });//建立房間(房間名稱，房間選項，{最多人數})
    }

    public void BtnJoinRoom()
    {
        PhotonNetwork.JoinRoom(NameJoinRoom);
    }


    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        textPrint.text = "進入大廳";
        playerIF.interactable = true;
        roomCreateIF.interactable = true;
        roomJoinIF.interactable = true;
        BtnCreate.interactable = true;
        BtnJoin.interactable = true;
    }

    public override void OnCreatedRoom()
    {
        textPrint.text = "創建房間成功，房間名稱:" + NameCreateRoom;
        PhotonNetwork.LoadLevel("遊戲場景");
    }

    public override void OnJoinedRoom()
    {
        textPrint.text = "加入房間成功，房間名稱:" + NameJoinRoom;
        PhotonNetwork.LoadLevel("遊戲場景");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        textPrint.text = "建立房間失敗" + returnCode + "訊息:" + message;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        textPrint.text = "加入房間失敗" + returnCode + "訊息:" + message;
    }
}
