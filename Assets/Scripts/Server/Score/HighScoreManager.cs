using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class HighScoreManager : MonoBehaviourPun
{
    public GameObject highScoreCanvas;

    [SerializeField]private List<HighScoreCanvas> _highScoreCanvasList = new List<HighScoreCanvas>();
    public static HighScoreManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        highScoreCanvas.SetActive(false);
    }

    [PunRPC] 
    public void RPC_ServerLoadScores(string[] dataName, int[] dateScore)
    {
        highScoreCanvas.SetActive(true);
        
        for (int i = 0; i < dataName.Length; i++)
        {
            _highScoreCanvasList[i].playerNickName.text = dataName[i];
            _highScoreCanvasList[i].totalScore.text =dateScore[i].ToString();
        }
    }
    
    [System.Serializable]
    public class HighScoreCanvas
    {
        public string name;
        public TMP_Text playerNickName;
        public TMP_Text totalScore;
    }
}


