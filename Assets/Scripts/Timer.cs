using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Timer : MonoBehaviourPun
{
    public float targetTime = 60;
    private float time;
    TMP_Text _clock;

    private bool startTimer = false;
    
    public Animator _anim;
    void Start()
    {
        startTimer = false;
        time = targetTime;
        _clock = GameObject.FindWithTag("Clock").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("start timer: "+startTimer);

        if (!startTimer)
        {
            _clock.text = "0";
            return;
        }
        
        targetTime -= Time.deltaTime;

        _clock.text = targetTime.ToString("f0");
 
        if (targetTime <= 0f)
        {
            string word = PlayerManager.instace.savedWord;
            photonView.RPC("RPC_RecieveMessage", RpcTarget.All,"ANSWER", word);
        }
 
    }
    
    [PunRPC]
    void RPC_TimeUpdate(bool b)
    {
        TimerEnded();
        startTimer = b;
        _anim.SetBool("IsClocking", b);
    }
    
    void TimerEnded()
    {
        targetTime = time;
    }
        
}
