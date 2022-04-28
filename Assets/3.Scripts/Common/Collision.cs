using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public static readonly string LAYER = "Collision";

    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(LAYER);
    }
}
