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
/*
    public void CreateBrush(Vector2 startPos, Vector2 endPos)
    {
        // PhotonNetwork.Instantiate(brush.name, transform.position, transform.rotation)
        //     .GetComponent<Brush>()
        //     .SetStartingPosition(startPos)
        //     .transform.SetParent(this.gameObject.transform);
    }

    private void Update()
    {
        var instBrush = FindObjectOfType<Brush>();
        if (instBrush == null) return;
        if (!_brushMade.Contains(instBrush))_brushMade.Add(instBrush);
    }
     public void DrawAction( Vector2  pointPos)
    {
        if (_brushMade.LastOrDefault() != default)
        {
            Debug.Log("draw action");
            _brushMade.Last().SetNewPoint(pointPos);
        }
    }
*/
    public void Move(Vector2 pos)
    {
        transform.position = pos;
    }

   
    
    
   /* public void EndDrawAction()
    {
        //currentLineRenderer = null;
    }
    public void ClearDraw()
    {
        foreach (var d in _brushMade)
        {
            Destroy(d.gameObject);
        }
        _brushMade.Clear();
    }*/

}