using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using Random = UnityEngine.Random;

public class CharacterFA : MonoBehaviourPunCallbacks, IPunObservable
{ 
    Player _owner;
   [SerializeField]private List<GameObject> _buttons= new List<GameObject>();
  // [SerializeField] private List<string> _words = new List<string>();

    public CharacterFA SetInitialParameters(Player player)
    {
        _owner = player;

        photonView.RPC("SetLocalParams", _owner);
        
        return this;
    }
    
    [PunRPC]
    void DisconnectOwner()
    {
        PhotonNetwork.Disconnect();
    }
    
    [PunRPC]
    void SetLocalParams()
    {
        _owner = PhotonNetwork.LocalPlayer;
        
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