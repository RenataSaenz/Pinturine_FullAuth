using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using Random = UnityEngine.Random;
public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;
    
    //[SerializeField] private int _pointsCounter1;
   // [SerializeField] private int _pointsCounter2;
    [SerializeField] private int _round = 0;
  //  [SerializeField] private TMP_Text _totalPointsShowPlayer1;
   // [SerializeField] private TMP_Text _totalPointsShowPlayer2;
    [SerializeField] private TMP_Text _totalRound;
    
    [SerializeField] private TMP_Text _wordSpace;
    
    public GameObject player1WonPrefab; 
    public GameObject player2WonPrefab;  
    public GameObject tieWonPrefab;
    
    [SerializeField] private List<string> _words = new List<string>();
    public List<PlayersData> _playersData = new List<PlayersData>();
    
    public string savedWord;
    
    public List<string> words { get { return _words; } }
    
    [SerializeField] private Player _turn;
    public Player turn { get { return _turn; } set { _turn = turn; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _round = 1;
        _totalRound.text = "Round " + _round + "/5";
        
       // if (!Equals(PhotonNetwork.LocalPlayer, clientOwner)
       // photonView.RPC("RPC_SetScore", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    public void RPC_SetScore(string clientOwner)
    {
        _playersData[PhotonNetwork.PlayerList.Length - 1].nickname = clientOwner;
        _playersData[PhotonNetwork.PlayerList.Length - 1].UpdateData();
    }


    [PunRPC] 
    public void RPC_ChangeTurn(Player clientOwner)
    {
        _turn = clientOwner;
        _round += 1;
        _totalRound.text = "Round " + _round + "/5";

        _playersData.SingleOrDefault(x => x.nickname == clientOwner.NickName)!.pointsCounter +=50;
        _playersData.SingleOrDefault(x => x.nickname == clientOwner.NickName)!.UpdateData();

        
        
        // for (int i = 0; i < _playersData.Count; i++)
        // {
        //     _playersData[i].UpdateData();
        // }
    }
    [PunRPC] 
    public void RPC_RemoveWords(string[] selectedWords)
    {
        for (int i = 0; i < selectedWords.Length; i++)
        {
            _words.Remove(selectedWords[i]);
        }
    }


    [PunRPC] 
    public void RPC_ReceiveWord(Player clientOwner,string word)
    {
        _wordSpace.text = "";
        
        if (!Equals(PhotonNetwork.LocalPlayer, clientOwner))
        {
            savedWord = word;
            
            for (int i = 0; i < savedWord.Length; i++)
            {
                _wordSpace.text = _wordSpace.text + "_ ";
            }
        }
        else
        {
            savedWord = word;
            
            _wordSpace.text = savedWord;
        }
    }

    public void TryWord(Player plr, string wordTry)
    {
        if (wordTry != savedWord) return;
        photonView.RPC("RPC_ChangeTurn", RpcTarget.All, plr);
        ButtonsManager.Instance.photonView.RPC("RPC_SetMenu", RpcTarget.All, turn,words.ToArray());
        
    }

    
    
}
[Serializable]
public class PlayersData
{
    public string nickname;
    public int pointsCounter = 0;
    public TMP_Text text;

    public void UpdateData()
    {
        text.text =nickname +": " + pointsCounter.ToString();
    }
    
}
