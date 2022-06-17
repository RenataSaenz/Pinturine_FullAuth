using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class CharacterFA : MonoBehaviourPunCallbacks, IPunObservable
{ 
    Player _owner;
   // public Brush brush;
   // [SerializeField] private List<Brush> _brushMade = new List<Brush>();

    public CharacterFA SetInitialParameters(Player player)
    {
        _owner = player;

        photonView.RPC("SetLocalParms", _owner);

        Debug.Log("INITIAL PARAMETERS");

        return this;
    }
    [PunRPC]
    void DisconnectOwner()
    {
        Debug.LogWarning("SE DESCONECTO");
        PhotonNetwork.Disconnect();
    }
    [PunRPC]
    void SetLocalParms()
    {
        _owner = PhotonNetwork.LocalPlayer;

        Debug.Log("LOCAL PARAMETERS");
    }
    private void OnApplicationQuit()
    {
        if (_owner == PhotonNetwork.LocalPlayer)
        {
            MyServer.Instance.RequestDisconnection(_owner);
        }
        PhotonNetwork.Disconnect();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
        }
        else
        {
        }
    }
    public void Move(Vector2 pos)
    {
        transform.position = pos;
    }


}