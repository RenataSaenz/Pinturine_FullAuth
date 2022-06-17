using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class ControllerFA : MonoBehaviourPun
{
    Player _localPlayer;
    [SerializeField]Camera _mainCam;
    private Vector2 lastPos;
    
    Vector2 _actualPos;
    
    
    
   // Action _ArtificialUpdateLeftClick;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _localPlayer = PhotonNetwork.LocalPlayer;
       // _ArtificialUpdateLeftClick = SimpleClick;
    }

    private void Update()
    {
        DrawAction();

       // _ArtificialUpdateLeftClick();
    }

    public void DrawAction()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
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
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            MyServer.Instance.RequestClearDraw(_localPlayer);
        }
    }
    
    // void SimpleClick()
    // { 
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         //Transform hittedObject;
    //
    //         _actualPos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
    //         
    //         //if (hittedObject == null) return;
    //         
    //         MyServer.Instance.RequestCreateBrush(_localPlayer, _actualPos, _actualPos);
    //
    //         _ArtificialUpdateLeftClick = DuringSimpleClick;
    //     }
    // }
    //
    // void DuringSimpleClick()
    // {
    //     //Transform hittedObject;
    //
    //     _actualPos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
    //
    //     if (_actualPos != lastPos)
    //     {
    //             MyServer.Instance.RequestDrawAction(_localPlayer, _actualPos);
    //             lastPos = _actualPos;
    //         }
    //     
    //     if (Input.GetMouseButtonUp(0))
    //     {
    //         _ArtificialUpdateLeftClick = SimpleClick;
    //     }
    // }
    //


}
