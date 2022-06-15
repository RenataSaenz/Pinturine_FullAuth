using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class PlayerMessenger :  MonoBehaviourPun
{
    public string message;
    [SerializeField]private string player;

    public TMP_InputField inputField;
    
    private void Start()
    {
        //if (!photonView.IsMine) return;
        
        inputField = GameObject.FindWithTag("InputField").GetComponent<TMP_InputField>();
        
        player = PlayerManager.instace.player;
        Debug.Log("we  " + player);
    }

    private void Update()
    {
        //if (!photonView.IsMine) return;
        
        if (inputField != null) message = inputField.text;
        if (Input.GetKeyDown(KeyCode.Return) && inputField.text != "")
        {
            SendMessageToPlayers(message);
            inputField.text = "";
        }
    }

    void SendMessageToPlayers( string sendMessageText)
    {
        photonView.RPC("RPC_RecieveMessage", RpcTarget.All,player, sendMessageText);
    }

}
