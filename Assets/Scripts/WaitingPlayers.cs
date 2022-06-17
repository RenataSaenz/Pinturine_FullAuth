using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
public class WaitingPlayers : MonoBehaviourPun
{
    [SerializeField] private TMP_Text _waitForPlayers;
    [SerializeField] private float time = 0.3f;
    // [SerializeField] private GameObject _canvas;
    private int count = 1;
    void Start()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    { 
        var playerCount = PhotonNetwork.PlayerList.Length - 1;
        _waitForPlayers.text = "WAITNG FOR PLAYERS" + Environment.NewLine +playerCount + "/2" ;

        if (playerCount == 2 && count == 1)
        {
            StartCoroutine(ExampleCoroutine(time));
            count --;
        }
    }
    
    public void Clear()
    {
        PhotonNetwork.Destroy(gameObject);
    } 
    
    IEnumerator ExampleCoroutine(float t)
    {
        yield return new WaitForSeconds(t);
        MyServer.Instance.RequestStartGame(this);

    }
}
