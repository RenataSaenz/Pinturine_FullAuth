using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class TimeManager : MonoBehaviourPun
{
    public float targetTime = 20;
    private float time;
    [SerializeField]TMP_Text _clock;

    private bool startTimer = false;
    
    public Animator _anim;

    public static TimeManager Instance;
    private bool _restart;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        startTimer = false;
        time = targetTime;
    }

    // Update is called once per frame
    void Update()
    {

        //string word = PlayerManager.instace.savedWord;
            //photonView.RPC("RPC_RecieveMessage", RpcTarget.All,"ANSWER", word);

            if(!photonView.IsMine)return;
            
            if (startTimer)
            {
                targetTime -= Time.deltaTime;
                photonView.RPC("RPC_TimeUpdate",RpcTarget.All, startTimer,targetTime);
            }
            
            if (targetTime <= 0f && _restart)
            {
                startTimer = false;
                targetTime = 0;
                photonView.RPC("RPC_TimeUpdate",RpcTarget.All, startTimer,targetTime);
                ButtonsManager.Instance.photonView.RPC("RPC_SetWaitingButtons", RpcTarget.All);
                ButtonsManager.Instance.photonView.RPC("RPC_SetWordsInServer", RpcTarget.MasterClient, GameManager.Instance.Turn);
                _restart = false;
            }
    }

    [PunRPC]
    void RPC_ServerTimeUpdate()
    {
        targetTime = time;
        startTimer = true;
        _restart = true;
    }
    
    [PunRPC]
    void RPC_ServerForceTimeStop()
    {
        _restart = false;
        startTimer = false;
        targetTime = 0;
        photonView.RPC("RPC_TimeUpdate",RpcTarget.All, startTimer,targetTime);
    }

    [PunRPC]
    void RPC_TimeUpdate(bool b, float seconds)
    {
       // targetTime = time;
        _anim.SetBool("IsClocking", b);
        // if (!startTimer)
        // {
        //     _clock.text = "0";
        //     return;
        // }
        _clock.text = seconds.ToString("f0");
    }

}
