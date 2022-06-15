using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Brush : MonoBehaviourPun
{
    CharacterFA _owner;
    public LineRenderer lineRenderer;

    public Brush SetOwner(CharacterFA owner)
    {
        _owner = owner;
        return this;
    }

    public Brush SetMaterialColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
        return this;
    } 
    
    public Brush SetStartingPosition(Vector2 pos)
    {
        lineRenderer.SetPosition(0, pos);
        lineRenderer.SetPosition(1, pos);
        return this;
    } 
    // [PunRPC]
    // void SetStartingPosition(Player clientOwner, int index, Vector2 pos)
    // {  Debug.Log("draw action");
    //     lineRenderer.positionCount++;
    //     int positionIndex = lineRenderer.positionCount - 1;
    //     Debug.Log(pos);
    //     lineRenderer.SetPosition(positionIndex, pos);
    // }
    
    [PunRPC]
    void SetNewPosition(Player clientOwner,  Vector2 pos)
    {  Debug.Log("draw action");
        lineRenderer.positionCount++;
        int positionIndex = lineRenderer.positionCount - 1;
        Debug.Log(pos);
        lineRenderer.SetPosition(positionIndex, pos);
    }

}
