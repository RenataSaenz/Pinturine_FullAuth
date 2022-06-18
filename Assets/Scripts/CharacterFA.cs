using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using Random = UnityEngine.Random;

public class CharacterFA : MonoBehaviourPunCallbacks, IPunObservable
{ 
    Player _owner;
   [SerializeField]private List<GameObject> _buttons= new List<GameObject>();
  // [SerializeField] private List<string> _words = new List<string>();

    public CharacterFA SetInitialParameters(Player player)
    {
        _owner = player;

        photonView.RPC("SetLocalParams", _owner);

        _buttons.Add(FindObjectOfType<CanvasButtons>().GetButtons()[0]);
        _buttons.Add(FindObjectOfType<CanvasButtons>().GetButtons()[1]);
        _buttons.Add(FindObjectOfType<CanvasButtons>().GetButtons()[2]);
        
        
        foreach (var button in _buttons)
        {
            button.SetActive(false);
        }

        return this;
    }
    [PunRPC]
    void DisconnectOwner()
    {
        PhotonNetwork.Disconnect();
    }
    [PunRPC]
    void SetLocalParams()
    {
        _owner = PhotonNetwork.LocalPlayer;

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
        }
        else
        {
        }
    }

    public void SetMenu(Player clientOwner, string[] words)
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
        // List<string> randomWords = new List<string>();
        //
        // for (int i = 0; i < words.Count; i++)
        // {
        //     randomWords.Add(words[i]);
        // }

        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].SetActive(true);
            var b =_buttons[i].GetComponentInChildren<TMP_Text>();
            //string randomString = words[Random.Range (0, words.Length)];
            //words.Remove(randomString);
            b.text =  words[i];
        }
    }

    void WaitingButtons()
    {
        Debug.Log("set buttons");
        foreach (var button in _buttons)
        {
            button.SetActive(true);
            var b = button.GetComponentInChildren<TMP_Text>();
            b.text = "waiting for player";
        }
    }
    public void Move(Vector2 pos)
    {
        transform.position = pos;
    }


}