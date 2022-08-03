using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine;

public class ControllerFA : MonoBehaviourPun
{
    Player _localPlayer;
    [SerializeField]Camera _mainCam;
    private Vector2 lastPos;
    
    Vector2 _actualPos;
    [SerializeField]private GameObject _drawingSpace;
    int UILayer = 5;

     public Action ArtificialUpdateLeftClick;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _localPlayer = PhotonNetwork.LocalPlayer;
        ArtificialUpdateLeftClick = CreateBrush;

    }

    private void Update()
    {
        //DrawAction();

        if (Input.GetKeyDown(KeyCode.C))
        {
            MyServer.Instance.RequestClearDraw(_localPlayer);
        }
        ArtificialUpdateLeftClick();
    }

    public void CreateBrush()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            MyServer.Instance.RequestCreateBrush(_localPlayer, mousePos, mousePos);

            ArtificialUpdateLeftClick = UpdateDraw;
        }
    }

    public void UpdateDraw()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            
            if (mousePos != lastPos)
            {
                MyServer.Instance.RequestDrawAction(_localPlayer, mousePos);
                lastPos = mousePos;
            }
        }
        else
        {
            MyServer.Instance.RequestEndDrawAction(_localPlayer);
            ArtificialUpdateLeftClick = CreateBrush;
        }
    }
    


}
