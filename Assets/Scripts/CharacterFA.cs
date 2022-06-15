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
    [SerializeField] private List<GameObject> _drawMade = new List<GameObject>();
    private LineRenderer currentLineRenderer;
    
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
        var brushInstance = PhotonNetwork.Instantiate(brush.name, brush.transform.position, brush.transform.rotation);
        _drawMade.Add(brushInstance);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
      
        currentLineRenderer.SetPosition(0, startPos);
        currentLineRenderer.SetPosition(1, endPos);
    }

    public void DrawAction(Vector2  pointPos)
    {
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(positionIndex, pointPos);
    }
    
    public void EndDrawAction()
    {
        currentLineRenderer = null;
    }
    public void ClearDraw()
    {
        foreach (var d in _drawMade)
        {
            Destroy(d.gameObject);
        }
        _drawMade.Clear();
    }

}