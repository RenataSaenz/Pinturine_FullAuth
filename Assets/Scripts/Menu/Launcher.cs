using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject mainScreen, connectedScreen;

    public void BTN_Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        mainScreen.SetActive(false);
        connectedScreen.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Connection failed: {cause.ToString()}");
    }
}