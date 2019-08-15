using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("玩家生成物件")]
    public GameObject prefabPlayer;
    [Header("玩家重生點")]
    public Transform[] spawnPlayer;

    private void Start()
    {
        SpawnPlayer();
    }

    /// <summary>
    /// 玩家生成
    /// </summary>
    private void SpawnPlayer()
    {
        //隨機=隨機(0,陣列長度);
        int r = Random.Range(0, spawnPlayer.Length);

        //PhotonNetwork連線.實例化(物件名稱，座標，角度)
        PhotonNetwork.Instantiate(prefabPlayer.name, spawnPlayer[r].position, Quaternion.identity);
    }
}
