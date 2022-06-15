using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyUI : MonoBehaviourPunCallbacks
{
    public TMP_InputField serverNameField;

    public void BTN_CreateRoom()
    {
        RoomOptions options = new RoomOptions();

        options.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(serverNameField.text, options);
  
    }

    public void BTN_JoinRoom()
    {
        PhotonNetwork.JoinRoom(serverNameField.text);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Level");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to create room {returnCode}, message {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to join room {returnCode}, message {message}");
    }
}
