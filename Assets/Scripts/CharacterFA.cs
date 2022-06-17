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

        photonView.RPC("SetLocalParms", _owner);

        Debug.Log("INITIAL PARAMETERS");
        
        _buttons.Add(GameObject.FindGameObjectWithTag("BTN_1"));
        _buttons.Add(GameObject.FindGameObjectWithTag("BTN_2"));
        _buttons.Add(GameObject.FindGameObjectWithTag("BTN_3"));
        
        foreach (var button in _buttons)
        {
         button.SetActive(false);
        }

        return this;
    }
    [PunRPC]
    void DisconnectOwner()
    {
        Debug.LogWarning("SE DESCONECTO");
        PhotonNetwork.Disconnect();
    }
    [PunRPC]
    void SetLocalParms()
    {
        _owner = PhotonNetwork.LocalPlayer;

        Debug.Log("LOCAL PARAMETERS");
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

    public void SetMenu(Player clientOwner, List<String> words)
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
    
    void SetButtons( List<String> words)
    {
        // List<String> randomWords = new List<String>();
        //
        // for (int i = 0; i < words.Count; i++)
        // {
        //     randomWords.Add(words[i]);
        // }
        foreach (var button in _buttons) {button.SetActive(true);}

        Debug.Log("set buttons");
        foreach (var button in _buttons)
        {
            var b = button.GetComponentInChildren<TMP_Text>();
            string randomString = words[Random.Range (0, words.Count)];
            words.Remove(randomString);
            b.text = randomString;
            Debug.Log(randomString);
        }
    }

    void WaitingButtons()
    {
        
        foreach (var button in _buttons) {button.SetActive(true);}

        Debug.Log("set buttons");
        foreach (var button in _buttons)
        {
            var b = button.GetComponentInChildren<TMP_Text>();
            b.text = "waiting for player";
        }
    }
    public void Move(Vector2 pos)
    {
        transform.position = pos;
    }


}