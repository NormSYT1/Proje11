using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager1 : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invictibleDuration;

    private float hatPickupTime;


    [Header("Player")]
    public string playerPrefabLocation;

    public Transform[] spawnPoints;
    public PlayerControl[] players;
    public int playerWithHat;
    private int playersInGame;

    public static GameManager1 instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        players = new PlayerControl[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.All);
    }
    [PunRPC]
    public void WinGame(int playerId)
    {
        gameEnded = true;
        PlayerControl player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);
        Invoke("BackToMenu", 5.0f);
    }
    public void BackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }
    [PunRPC]
    public void GiveHat(int playerId, bool initialGive)
    {
        if (!initialGive)

            GetPlayer(playerWithHat).SetHat(false);
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickupTime = Time.time;
    }
    public bool CanGetHat()
    {
        if (Time.time > hatPickupTime + invictibleDuration)
            return true;
        else
            return false;
    }
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation,
            spawnPoints[Random.Range(0, spawnPoints.Length)].position,
            Quaternion.identity);
        PlayerControl playerScript = playerObj.GetComponent<PlayerControl>();

        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
    public PlayerControl GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }
    public PlayerControl GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }
}
