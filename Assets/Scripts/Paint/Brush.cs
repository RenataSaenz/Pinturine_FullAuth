using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class Brush :  MonoBehaviourPun
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
    
    public void SetNewPoint(Vector2 pos)
    {
        lineRenderer.positionCount++;
        int positionIndex = lineRenderer.positionCount - 1;
        Debug.Log(pos);
        Debug.Log("entre");
        lineRenderer.SetPosition(positionIndex, pos);
    } 
}
