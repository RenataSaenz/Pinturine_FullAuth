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
    
    Vector2 _actualPos;
    
    
    
    Action _ArtificialUpdateLeftClick;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _localPlayer = PhotonNetwork.LocalPlayer;
        Debug.Log("HOL");
        _ArtificialUpdateLeftClick = SimpleClick;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _mainCam = Camera.main;
            MyServer.Instance.RequestClearDraw(_localPlayer);
        }

        _ArtificialUpdateLeftClick();
    }

    public void DrawAction()
    {
        // if (Input.GetKeyDown(KeyCode.Mouse0))
        // {
        //     //_mainCam = Camera.main;
        //     Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        //     MyServer.Instance.RequestCreateBrush(_localPlayer, mousePos, mousePos);
        // }
        //
        // if (Input.GetKey(KeyCode.Mouse0))
        // {
        //     Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        //     if (mousePos != lastPos)
        //     {
        //         MyServer.Instance.RequestDrawAction(_localPlayer, mousePos);
        //         lastPos = mousePos;
        //     }
        // }
        //
        // if (Input.GetKeyUp(KeyCode.Mouse0))
        // {
        //     MyServer.Instance.RequestEndDrawAction(_localPlayer);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     MyServer.Instance.RequestClearDraw(_localPlayer);
        // }
    }
    
    void SimpleClick()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            //Transform hittedObject;

            _actualPos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            
            //if (hittedObject == null) return;

            MyServer.Instance.RequestCreateBrush(_localPlayer, _actualPos, _actualPos);

            _ArtificialUpdateLeftClick = DuringSimpleClick;
        }
    }
    
    void DuringSimpleClick()
    {
        //Transform hittedObject;

        _actualPos = _mainCam.ScreenToWorldPoint(Input.mousePosition);

        if (_actualPos != lastPos)
            {
                MyServer.Instance.RequestDrawAction(_localPlayer, _actualPos);
                lastPos = _actualPos;
            }
        
        if (Input.GetMouseButtonUp(0))
        {
            _ArtificialUpdateLeftClick = UpSimpleClick;
        }
    }
    
    void UpSimpleClick()
    {
        MyServer.Instance.RequestEndDrawAction(_localPlayer);
        
        _ArtificialUpdateLeftClick = SimpleClick;
    }

}
