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
    [SerializeField]private DrawingCanvas _drawingSpace;
    int UILayer = 5;

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

            if (!IsPointerOverUIElement())
            {
                MyServer.Instance.RequestCreateBrush(_localPlayer, mousePos, mousePos);
            }
               
        }
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            
            if (mousePos != lastPos && !IsPointerOverUIElement())
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

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }
    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    
}
