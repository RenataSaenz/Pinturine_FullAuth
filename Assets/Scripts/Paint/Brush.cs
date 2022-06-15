using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour
{
    Draw _owner;

    public Brush SetOwner(Draw owner)
    {
        _owner = owner;
        return this;
    }

    public Brush SetMaterialColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
        return this;
    }

}
