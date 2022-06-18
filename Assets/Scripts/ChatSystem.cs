using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChatSystem : MonoBehaviourPun
{
    [SerializeField] TMP_Text _chatText;
    [SerializeField] TMP_InputField _messageInputField;
    //[SerializeField] Scrollbar _verticalScrollBar;

    public static ChatSystem Instance;


    int _currentMsgsQuantity;
    int _oldMsgsQuantity;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        _messageInputField = GameObject.FindWithTag("InputField").GetComponent<TMP_InputField>();
        
        _chatText.text = "";
        _currentMsgsQuantity = 0;
    }

    void Update()
    {
        if (!_messageInputField.isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            _messageInputField.ActivateInputField();
        }

        TryEnterMessage();
      //  ScrollDownOnNewMsg();
    }

  

    void TryEnterMessage()
    {
        
        if (_messageInputField.text != "" && Input.GetKeyDown(KeyCode.Return))
        { 
            if (Equals(PhotonNetwork.LocalPlayer, GameManager.Instance.turn) &&_messageInputField.text == GameManager.Instance.savedWord) 
                _messageInputField.text = "***";
            
            //MyServer.Instance.RequestSendMessage(PhotonNetwork.LocalPlayer,PhotonNetwork.LocalPlayer.NickName,_messageInputField.text);
            
            photonView.RPC("RPC_SendMessageToChat",RpcTarget.All, PhotonNetwork.LocalPlayer,_messageInputField.text);
            
            if (!Equals(PhotonNetwork.LocalPlayer, GameManager.Instance.turn))
                GameManager.Instance.TryWord(PhotonNetwork.LocalPlayer,_messageInputField.text);
            
            _messageInputField.text = "";
        }
    }
    
    
    [PunRPC]
    public void RPC_SendMessageToChat(Player player,string newMsg)
    {
        _chatText.text += "\n" + player.NickName +": " +newMsg;
        _currentMsgsQuantity++;
    }

    // public void ScrollDownOnNewMsg()
    // {
    //     if (_oldMsgsQuantity != _currentMsgsQuantity)
    //     {
    //         _oldMsgsQuantity = _currentMsgsQuantity;
    //         _verticalScrollBar.value = 0;
    //     }
    // }

}
