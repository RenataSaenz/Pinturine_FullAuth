using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviourPun
{
    [SerializeField] private List<string> _words = new List<string>();
    private TMP_Text _wordSpace;
    private List<GameObject> _buttons= new List<GameObject>();
    
    public Draw playerBrush;
    public string savedWord;
    
    public static PlayerManager instace;

    public GameObject player1WonPrefab; 
    public GameObject player2WonPrefab;  
    public GameObject tieWonPrefab; 

    public String player;

    private bool _startCount = true;
    
    public Action onShift = delegate { };

    private ImageShare _imageShare;

    [SerializeField] private int _pointsCounter1;
    [SerializeField] private int _pointsCounter2;
    [SerializeField] private int _round = 0;
    [SerializeField] private TMP_Text _totalPointsShowPlayer1;
    [SerializeField] private TMP_Text _totalPointsShowPlayer2;
    [SerializeField] private TMP_Text _totalRound;
    
    private void Awake()
    {
        _buttons.Add(GameObject.FindGameObjectWithTag("BTN_1"));
        _buttons.Add(GameObject.FindGameObjectWithTag("BTN_2"));
        _buttons.Add(GameObject.FindGameObjectWithTag("BTN_3"));
        _wordSpace = GameObject.FindWithTag("WordSpace").GetComponent<TMP_Text>();
        _totalPointsShowPlayer1 = GameObject.FindWithTag("PointsPlayer1").GetComponent<TMP_Text>();
        _totalPointsShowPlayer2 = GameObject.FindWithTag("PointsPlayer2").GetComponent<TMP_Text>();
        _totalRound = GameObject.FindWithTag("Round").GetComponent<TMP_Text>();
        _imageShare = GameObject.FindWithTag("ImageShare").GetComponent<ImageShare>();
        
        // if (!photonView.IsMine)
        // {
        //     return;
        // }
        
        if (instace == null) instace = this;
        
        
        playerBrush = Instantiate(playerBrush, Vector3.zero, Quaternion.identity);
        
        var playerCount = PhotonNetwork.PlayerList.Length;
        
        player = "Player " + playerCount;
        _totalPointsShowPlayer1.text ="Player 1 Points: 0";
        _totalPointsShowPlayer2.text = "Player 2 Points: 0";
        _totalRound.text = "Round " + _round + "/5";

    }
    private void Update()
    {
        onShift();
    }

    public void AssignShift()
    {
        if (player == "Player " + 1)
        {
            _imageShare.UnableShare();
            SetButtons();
            onShift = DrawingShift;
        }
        else
        {
           
            _imageShare.DisableShare();
            onShift = GuessingShift;
        }
    }

    void DrawingShift()
    {
        playerBrush.DrawAction();
        Debug.Log("draw sHIFT");
    }

    void GuessingShift()
    {
        Debug.Log("guess sHIFT");
    }
    void SetButtons()
    {
        if (_words.Count < 3)
        { 
            photonView.RPC("RPC_GameOver", RpcTarget.All);
            return;
        }
        
        foreach (var button in _buttons) {button.SetActive(true);}

        Debug.Log("set buttons");
        foreach (var button in _buttons)
        {
            var b = button.GetComponentInChildren<TMP_Text>();
            string randomString = _words[Random.Range (0, _words.Count)];
            _words.Remove(randomString);
            photonView.RPC("RPC_RemoveWord", RpcTarget.All, randomString);
            b.text = randomString;
            Debug.Log(randomString);
        }
    }
    public void StartGame(string word)
    {
        photonView.RPC("RPC_TimeUpdate", RpcTarget.All, true);
        photonView.RPC("RPC_RecieveWord", RpcTarget.All, word);
        photonView.RPC("RPC_SetRound", RpcTarget.All);
    }
    
    void DropHint(string word)
    {
        String str =  word;
        char c = str[word.Length/2];
        string newString =  _wordSpace.text.Replace("_", c.ToString());
        _wordSpace.text = newString;
    }

    public void TryWord(string plr, string wordTry)
    {
        if (wordTry == savedWord)
        {
            photonView.RPC("RPC_TimeUpdate", RpcTarget.All, false);
            AddPoints(plr);
            if (onShift == GuessingShift) ChangeShiftToDrawing();
            else if (onShift == DrawingShift) ChangeShiftToGuessing();
        }
    }
    void ChangeShiftToGuessing()
    {
        savedWord = "";
        _wordSpace.text = "";
        
        playerBrush.ClearDrawing();
        _imageShare.DisableShare();
        onShift = GuessingShift;
        Debug.Log("Changed to Guessing");
    }
    void ChangeShiftToDrawing()
    {
        savedWord = "";
        _wordSpace.text = "";
        
        _imageShare.UnableShare();
        SetButtons();
        onShift = DrawingShift;
        Debug.Log("Changed to Drawing");
    }

    [PunRPC] 
    public void RPC_RecieveWord(string word)
    {
        savedWord = word;
        for (int i = 0; i < savedWord.Length; i++)
        {
            _wordSpace.text = _wordSpace.text + "_ ";
        }
        Debug.Log("the word is: " + word);
    }
    
    [PunRPC] 
    public void RPC_SetRound()
    {
        _round += 1;
        _totalRound.text = "Round " + _round + "/5";
    }
    
    [PunRPC] 
    public void RPC_GameOver()
    {
        if (_pointsCounter1> _pointsCounter2) 
            Instantiate(player1WonPrefab, Vector3.zero, Quaternion.identity);
        else if(_pointsCounter1< _pointsCounter2) 
            Instantiate(player2WonPrefab, Vector3.zero, Quaternion.identity);
        else if(_pointsCounter1== _pointsCounter2)
            Instantiate(tieWonPrefab, Vector3.zero, Quaternion.identity);
    }
    
    public void AddPoints(string plr)
    {
        if (plr == "Player 1")
        {
            _pointsCounter1 += 50;
            _totalPointsShowPlayer1.text ="Player 1 Points: " + _pointsCounter1.ToString();
        } if (plr == "Player 2")
        {
            _pointsCounter2 += 50;
            _totalPointsShowPlayer2.text = "Player 2 Points: " +_pointsCounter2.ToString();
        }
    }

    [PunRPC] 
    public void RPC_RemoveWord(string randomWord)
    {
        _words.Remove(randomWord);
    }
}


