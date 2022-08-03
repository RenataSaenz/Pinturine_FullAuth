using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class ColorManager : MonoBehaviourPun
{
    //[SerializeField]private List<GameObject> _colorsButtons= new List<GameObject>();
    [SerializeField]private Brush _brush;

    public void BTN_SelectColor(Brush color)
    {
        MyServer.Instance.RequestChangeColor(PhotonNetwork.LocalPlayer,color.name);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("hry");
            MyServer.Instance.RequestChangeColor(PhotonNetwork.LocalPlayer,_brush.name);
        }
            
    }
}
