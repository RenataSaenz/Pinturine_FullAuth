using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Linq;
public class WaitingPlayers : MonoBehaviourPun
{
    [SerializeField] private TMP_Text _waitForPlayers;
    [SerializeField] private float time = 0.3f;
    private int count = 1;
    private bool loadPlayer;

    void Update()
    {
        var playerCount = PhotonNetwork.PlayerList.Length - 1;
        _waitForPlayers.text = "WAITNG FOR PLAYERS" + Environment.NewLine +playerCount + "/3";

        // if (playerCount >= 1)
        // {
        //     GameManager.Instance._playersData[playerCount-1].nickname = PhotonNetwork.LocalPlayer.NickName;
        //     GameManager.Instance._playersData[playerCount-1].UpdateData();
        // }

        if (playerCount == 3 && count == 1)
        {
            if (!photonView.IsMine) return;
            StartCoroutine(ExampleCoroutine(time));
            count --;
        }
        
    }
    
    IEnumerator ExampleCoroutine(float t)
    {
        yield return new WaitForSeconds(t);
        //Debug.Log("is the turn of " + GameManager.Instance.Turn);
      //  ButtonsManager.Instance.SetButtons();
      
      //ButtonsManager.Instance.WaitingButtons();
      ButtonsManager.Instance.photonView.RPC("RPC_SetWaitingButtons", RpcTarget.All);
      ButtonsManager.Instance.photonView.RPC("RPC_SetWordsInServer", RpcTarget.MasterClient, GameManager.Instance.Turn);


      PhotonNetwork.Destroy(gameObject);
    }
}
