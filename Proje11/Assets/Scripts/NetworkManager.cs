using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkManager : MonoBehaviourPunCallbacks//Photon �zel ayarlar� i�in
{

    public static NetworkManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) 
        {
            gameObject.SetActive(false);
            
        }
        else
        {
            instance = this;//Oturum a��ld�
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    void Update()
    {
        
    }
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }
 
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
