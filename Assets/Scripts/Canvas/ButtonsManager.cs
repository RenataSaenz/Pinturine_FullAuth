using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class ButtonsManager : MonoBehaviourPun
{
    [SerializeField]private List<GameObject> _buttons= new List<GameObject>();
    public static ButtonsManager Instance;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        photonView.RPC("RPC_SetOffButtons", RpcTarget.All);
    }

    public void BTN_SelectWord(TMP_Text word)
    {
        GameManager.Instance.photonView.RPC("RPC_ReceiveWord", RpcTarget.All, PhotonNetwork.LocalPlayer, word.text);
        photonView.RPC("RPC_SetOffButtons", RpcTarget.All);
    }

    public void SetButtons()
    {
        photonView.RPC("RPC_SetMenu", RpcTarget.All, GameManager.Instance.turn,GameManager.Instance.words.ToArray());
    }
    
    [PunRPC]
    void RPC_SetOffButtons()
    {
       foreach (var button in _buttons)
       {
           button.SetActive(false);
       }
       
    }
    
    [PunRPC]
    public void RPC_SetMenu(Player clientOwner, string[] words)
    {
        if (PhotonNetwork.LocalPlayer != clientOwner)
        {
            WaitingButtons();
        }
        else
        {
            SetButtons(words);
        }
    }
    
    void SetButtons( string[] words)
    {
        List<string> selectedWords = new List<string>();
        
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].GetComponent<Button>().interactable = true;
            _buttons[i].SetActive(true);
            var b =_buttons[i].GetComponentInChildren<TMP_Text>();
            b.text =  words[i];
            selectedWords.Add(words[i]);
        }
        GameManager.Instance.photonView.RPC("RPC_RemoveWords",RpcTarget.All,selectedWords.ToArray());

    }

    void WaitingButtons()
    {
        foreach (var button in _buttons)
        {
            button.SetActive(true);
            button.GetComponent<Button>().interactable = false;
            var b = button.GetComponentInChildren<TMP_Text>();
            b.text = "Waiting for player";
        }
    }
    
}