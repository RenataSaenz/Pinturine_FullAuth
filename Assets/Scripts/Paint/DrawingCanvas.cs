using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingCanvas : MonoBehaviour, IPointerDownHandler,IPointerExitHandler
{
    [SerializeField]private ControllerFA _controller;
    private void Start()
    {
        _controller = FindObjectOfType<ControllerFA>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //_controller.ArtificialUpdateLeftClick = _controller.CreateBrush;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       // _controller.ArtificialUpdateLeftClick = delegate {  };
    }
}
