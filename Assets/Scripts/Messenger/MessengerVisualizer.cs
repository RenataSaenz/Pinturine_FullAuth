using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Pun;

public class MessengerVisualizer : MonoBehaviourPun
{
    public TMP_Text _chat;
    string _chatBefore;
    int countMessage;
    
    private void Awake()
    {
        //if (!photonView.IsMine) return;
        
        _chat = GameObject.FindWithTag("Chat").GetComponent<TMP_Text>();
    }

    [PunRPC] 
    public void RPC_RecieveMessage(string player, string message)
    {
        UpdateVisualizedMessages(player + ": " + message);
        PlayerManager.instace.TryWord(player, message);
    } 

    private void UpdateVisualizedMessages(string message)
    {
        _chat.text = _chat.text + Environment.NewLine + message;
    }
    
}
