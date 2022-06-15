using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using Photon.Pun;


public class StartLevel : MonoBehaviour
{   public GameObject playerPrefab;

    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
    }
}
