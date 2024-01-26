using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float speed;
    public float jump;
    public GameObject hat;

    [HideInInspector]
    public float curHatTime;

    [Header("Components")]
    public Rigidbody rb;
    public Player photonPlayer;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(curHatTime >= GameManager1.instance.timeToWin && !GameManager1.instance.gameEnded)
            {
                GameManager1.instance.gameEnded = true;
                GameManager1.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }
        if (photonView.IsMine)
        {
            Move();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryJump();
            }
            if (hat.activeInHierarchy)
            {
                curHatTime += Time.deltaTime;
            }      
        }
    }
    void Move()
    {
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;
        rb.velocity = new Vector3(x, rb.velocity.y, z);
    }
    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, 0.7f))
        {
            rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
        }
    }
    public void SetHat(bool hasHat)
    {
        hat.SetActive(hasHat);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {

            if (GameManager1.instance.GetPlayer(collision.gameObject).id == GameManager1.instance.playerWithHat)
            {

                if (GameManager1.instance.CanGetHat())
                {
                    GameManager1.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager1.instance.players[id - 1] = this;

        if (id == 1)
            GameManager1.instance.GiveHat(id, true);

        if (!photonView.IsMine)
            rb.isKinematic = true;
    }
    public void OnPhotonSerializeWiew(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHatTime);
        }
        else if (stream.IsReading)
        {
            curHatTime = (float)stream.ReceiveNext();
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHatTime);
        }
        else if (stream.IsReading)
        {
            curHatTime = (float)stream.ReceiveNext();
        }
    }
}
