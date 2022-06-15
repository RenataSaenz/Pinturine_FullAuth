using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartBrush : MonoBehaviour
{
    public Draw playerBrush;
    public bool unableBrush = true;

    void Start()
    {
        //PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        UnableDraw();
    }

    public void UnableDraw()
    {
        if (unableBrush)playerBrush.gameObject.SetActive(true);
    }
}
