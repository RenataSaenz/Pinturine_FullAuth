using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class Brush :  MonoBehaviourPunCallbacks, IPunObservable
{
    public LineRenderer lineRenderer;
    Player _owner;
    
    
    public Brush SetInitialParameters(Player player, Vector2 pos)
    {
        _owner = player;

        photonView.RPC("SetLocalParms", _owner);

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        
        lineRenderer.SetPosition(0, pos);
        lineRenderer.SetPosition(1, pos);

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


    public Brush SetMaterialColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
        return this;
    } 
    
    public Brush SetStartingPosition(Vector2 pos)
    {
        lineRenderer.SetPosition(0, pos);
        lineRenderer.SetPosition(1, pos);
        return this;
    } 
    
    public void SetNewPoint(Vector2 pos)
    {
        lineRenderer.positionCount++;
        int positionIndex = lineRenderer.positionCount - 1;
        lineRenderer.SetPosition(positionIndex, pos);
    } 
}
