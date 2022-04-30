using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public static readonly string LAYER = "Collision";

    private new Collider2D collider2D;

    public Vector3 ColliderPosition => GetColliderPosition();

    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(LAYER);
    }

    protected virtual Vector3 GetColliderPosition()
    {
        return transform.position;
    }
}
