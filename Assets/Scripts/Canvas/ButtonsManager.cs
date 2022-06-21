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
       
        photonView.RPC("RPC_SetMenu", RpcTarget.All, GameManager.Instance.Turn,GameManager.Instance.words.ToArray());
    }

    public void SetButtonsForGame()
    {
        photonView.RPC("RPC_SetWordsInServer", RpcTarget.MasterClient, GameManager.Instance.Turn);
    }
    

    [PunRPC]
    void RPC_SetWordsInServer(Player playerTurn)
    {
        List<string> selectedWords = new List<string>();
        List<string> copyWords = new List<string>(GameManager.Instance.words);
        
        for (int i = 0; i < _buttons.Count; i++)
        {
           // _buttons[i].GetComponent<Button>().interactable = true;
           // _buttons[i].SetActive(true);
            //var b =_buttons[i].GetComponentInChildren<TMP_Text>();
            string randomString = copyWords[Random.Range (0, copyWords.Count)];
            copyWords.Remove(randomString);
            //b.text =  randomString;
            selectedWords.Add(randomString);
        }
        
        for (int i = 0; i < selectedWords.Count; i++)
        {
            GameManager.Instance._words.Remove(selectedWords[i]);
        }

        
        photonView.RPC("RPC_SetMenuForTurn", playerTurn, selectedWords.ToArray());
        //GameManager.Instance.photonView.RPC("RPC_RemoveWords",RpcTarget.All,selectedWords.ToArray());
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
    public void RPC_SetMenuForTurn(string[] words)
    {
        List<string> selectedWords = new List<string>(words);
        
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].GetComponent<Button>().interactable = true;
            _buttons[i].SetActive(true);
            var b =_buttons[i].GetComponentInChildren<TMP_Text>();
            b.text =  selectedWords[i];
           // selectedWords.Add(words[i]);
        }
        
        //GameManager.Instance.photonView.RPC("RPC_RemoveWords",RpcTarget.All,selectedWords.ToArray());
    }
    
    [PunRPC]
    public void RPC_SetMenu(Player playerTurn, string[] words)
    {
        if (PhotonNetwork.LocalPlayer != playerTurn)
        {
            WaitingButtons();
        }
        else
        {
            SetButtons(words);
        }
    }
    
    [PunRPC]
    public void RPC_SetWaitingButtons()
    {
        foreach (var button in _buttons)
        {
            button.SetActive(true);
            button.GetComponent<Button>().interactable = false;
            var b = button.GetComponentInChildren<TMP_Text>();
            b.text = "Waiting for player";
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

    public void WaitingButtons()
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