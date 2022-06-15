using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerFA : MonoBehaviourPun
{
    Player _localPlayer;
    [SerializeField]Camera _mainCam;
    private Vector2 lastPos;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _localPlayer = PhotonNetwork.LocalPlayer;
        Debug.Log("HOL");
    }

    private void Update()
    {
        DrawAction();
    }

    public void DrawAction()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //_mainCam = Camera.main;
            Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            MyServer.Instance.RequestCreateBrush(_localPlayer, mousePos, mousePos);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos != lastPos)
            {
                MyServer.Instance.RequestDrawAction(_localPlayer, mousePos);
                lastPos = mousePos;
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            MyServer.Instance.RequestEndDrawAction(_localPlayer);
        }
    }
}
