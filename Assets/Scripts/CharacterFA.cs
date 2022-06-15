using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class CharacterFA : MonoBehaviourPunCallbacks, IPunObservable
{ 
    Player _owner;
    public Brush brush;
    [SerializeField] private List<Brush> _brushMade = new List<Brush>();
    public LineRenderer currentLineRenderer;
    
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
          //  stream.SendNext(_currentLife);
            //stream.SendNext(_maxLife);
        }
        else
        {
         //   _currentLife = (float)stream.ReceiveNext();
          //  _maxLife = (float)stream.ReceiveNext();
         //   onLifeBarUpdate(_currentLife/_maxLife);
        }
    }

    public void CreateBrush(Vector2 startPos, Vector2 endPos)
    {
        var brushInstance = PhotonNetwork.Instantiate(brush.name, transform.position, transform.rotation)
            .GetComponent<Brush>()
            .SetOwner(this)
            .SetStartingPosition(startPos);
        _brushMade.Add(brushInstance);
        currentLineRenderer = brushInstance.lineRenderer;
        if (currentLineRenderer == null)Debug.Log("currentLineRenderer is null");
      
        // currentLineRenderer.SetPosition(0, startPos);
        // currentLineRenderer.SetPosition(1, endPos);
    }

    public void DrawAction(Vector2  pointPos)
    {
        Debug.Log("draw action");
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        Debug.Log(pointPos);
        currentLineRenderer.SetPosition(positionIndex, pointPos);
    }
    
    public void EndDrawAction()
    {
        currentLineRenderer = null;
    }
    public void ClearDraw()
    {
        foreach (var d in _brushMade)
        {
            Destroy(d.gameObject);
        }
        _brushMade.Clear();
    }

}