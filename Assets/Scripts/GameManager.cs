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

    [SerializeField] private int _round = 0;
    
    [SerializeField] private TMP_Text _totalRound;
    
    [SerializeField] private TMP_Text _wordSpace;
    

    public List<string> _words = new List<string>();

    public string savedWord;
    public List<string> words { get { return _words; }  set { _words = words; }}
    
    public Player Turn;
    
    [SerializeField] private Animator[] _facesInScene;
    public TMP_Text[] textsInScene;
    public Dictionary<TMP_Text, bool> dictAreasPlayers = new Dictionary<TMP_Text, bool>();
    public Dictionary<Animator, bool> dictAnimatorPlayers = new Dictionary<Animator, bool>();
    private List<PlayersData> _playersData = new List<PlayersData>();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        _round = 1;
        _totalRound.text = "Round " + _round + "/7";

        for (int i = 0; i < textsInScene.Length; i++)
        {
            dictAreasPlayers.Add(textsInScene[i],false);
        }
        
        for (int i = 0; i < _facesInScene.Length; i++)
        {
            dictAnimatorPlayers.Add(_facesInScene[i], false);
        }
        
    }
    
    [PunRPC] 
    public void RPC_SetDrawingFaces(string player, bool b)
    {
        _playersData.Find(data => data.Nickname == player).AnimatorPlayer.SetBool("IsDrawing", b);
    }
    [PunRPC] 
    public void RPC_SetThinkingFaces(string player, bool b)
    {
        _playersData.Find(data => data.Nickname == player).AnimatorPlayer.SetBool("IsThinking", b);
    }

    [PunRPC] 
    public void RPC_SetDefeatFaces(string player)
    {
        _playersData.Find(data => data.Nickname == player).AnimatorPlayer.SetTrigger("IsLoser");
    }
    [PunRPC] 
    public void RPC_SetWinnerFaces(string player)
    {
        _playersData.Find(data => data.Nickname == player).AnimatorPlayer.SetTrigger("IsWinner");
       
    }
    
    [PunRPC] 
    public void RPC_ResetFaces(string player)
    {
        _playersData.Find(data => data.Nickname == player).AnimatorPlayer.SetTrigger("IsWinner");
        
        PlayersData playerToChange = _playersData.Find(data => data.Nickname == player);
        playerToChange.AnimatorPlayer.SetBool("IsThinking", false);
        playerToChange.AnimatorPlayer.SetBool("IsDrawing", false);
        playerToChange.AnimatorPlayer.SetTrigger("IsHappy");
    }

    [PunRPC] 
    public void RPC_ChangeTurn(Player clientOwner)
    {
        Turn = clientOwner;
        _round += 1;
        _totalRound.text = "Round " + _round + "/7";
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
        
        TimeManager.Instance.photonView.RPC("RPC_ServerTimeUpdate", RpcTarget.MasterClient);

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
        photonView.RPC("RPC_AskServer", RpcTarget.MasterClient, plr,wordTry);
    }

    [PunRPC]
    public void RPC_ReactionToMistakeFaces(string plr, string turn)
    {
        _playersData.Find(data => data.Nickname == plr).AnimatorPlayer.SetTrigger("IsMistake");
        _playersData.Find(data => data.Nickname == turn).AnimatorPlayer.SetTrigger("IsDisappointed");  
    }

    [PunRPC]
    public void RPC_AskServer(Player plr, string wordTry)
    {
        if (wordTry != savedWord)
        {
            photonView.RPC("RPC_ReactionToMistakeFaces", RpcTarget.All, plr.NickName, Turn.NickName);
            return;
        }
        
        MyServer.Instance.RequestClearDraw(Turn);
        photonView.RPC("RPC_ResetFaces", RpcTarget.All, Turn.NickName);
        
        TimeManager.Instance.photonView.RPC("RPC_ServerForceTimeStop", RpcTarget.MasterClient);
        photonView.RPC("RPC_SetScore", RpcTarget.All, plr.NickName);
        

        if (_round == 7 ||_words.Count < 3)
        {
            List<string> orderDataNames = new List<string>();
            List<int> orderDataScore = new List<int>();
            orderDataNames = _playersData.OrderByDescending(x => x.TotalScore).Select(x => x.Nickname).ToList();
            orderDataScore = _playersData.OrderByDescending(x => x.TotalScore).Select(x => x.TotalScore).ToList();

            photonView.RPC("RPC_SetWinnerFaces", RpcTarget.All, orderDataNames[0]);
            
            for (int i = 1; i < orderDataNames.Count; i++)
            {
                photonView.RPC("RPC_SetDefeatFaces", RpcTarget.All, orderDataNames[i]);
            }

            HighScoreManager.Instance.photonView.RPC("RPC_ServerLoadScores", RpcTarget.All, orderDataNames.ToArray(), orderDataScore.ToArray());
            return;
        }
        photonView.RPC("RPC_ChangeTurn", RpcTarget.All, plr);
       ButtonsManager.Instance.photonView.RPC("RPC_SetWaitingButtons", RpcTarget.All);
       ButtonsManager.Instance.photonView.RPC("RPC_SetWordsInServer", RpcTarget.MasterClient, plr);

    }

    [PunRPC]
    public void RPC_SetScore(string playerNickName) //void RPC_SetScore(string[] players)
    {
        var roundWinner = _playersData.Find(data => data.Nickname == playerNickName);
        roundWinner.TotalScore += 50;
        roundWinner.UpdateData();
        roundWinner.AnimatorPlayer.SetTrigger("IsPoint");
    }
    
    [PunRPC]
    public void RPC_AddPlayerToScore(string player)
    {
        _playersData.Add(new PlayersData(player,0));
        _playersData[_playersData.Count - 1].UpdateData();
        _playersData[_playersData.Count - 1].AnimatorPlayer.SetTrigger("IsHappy");
    }

}

[Serializable]
public class PlayersData
{
    public string Nickname{ set; get; }
    public int TotalScore{ set; get; }
    public TMP_Text Text{ set; get; }
    public Animator AnimatorPlayer{ set; get; }

    public PlayersData(string nickName, int totalScore)
    {
        this.Nickname = nickName;
        this.TotalScore = totalScore;

        for (int index = 0; index < GameManager.Instance.dictAnimatorPlayers.Count; index++) 
        {
            var item = GameManager.Instance.dictAnimatorPlayers.ElementAt(index);
            var itemKey = item.Key;
            var itemValue = item.Value;
            
            if (!itemValue)
            {
                AnimatorPlayer = itemKey;
                GameManager.Instance.dictAnimatorPlayers[itemKey] = true;
                break;
            }
        }
        for (int index = 0; index < GameManager.Instance.dictAreasPlayers.Count; index++) 
        {
            var item = GameManager.Instance.dictAreasPlayers.ElementAt(index);
            var itemKey = item.Key;
            var itemValue = item.Value;
            
            if (!itemValue)
            {
                Text = itemKey;
                GameManager.Instance.dictAreasPlayers[itemKey] = true;
                break;
            }
        }
        
    }
    public void UpdateData()
    {
        Text.text =Nickname +": " + TotalScore.ToString();
    }
}

