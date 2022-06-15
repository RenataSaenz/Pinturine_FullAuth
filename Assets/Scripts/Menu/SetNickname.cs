using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SetNickname : MonoBehaviour
{
    public void ChangeName(string newName)
    {
        Player playerLocal = PhotonNetwork.LocalPlayer;
        playerLocal.NickName = newName;
    }
}
