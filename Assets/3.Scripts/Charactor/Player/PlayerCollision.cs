using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : CharacterCollision
{
    public static readonly string TAG = "Player Collision";

    [SerializeField]
    private Player player;
    [SerializeField]
    private PlayerColliderChanger colliderChanger;

    protected override void Awake()
    {
        base.Awake();
        gameObject.tag = TAG;
        character = player;
    }

    private void Update()
    {
        UpdateColliderPosition();
    }

    private Vector3 colliderPosition;
    private void UpdateColliderPosition()
    {
        colliderPosition = colliderChanger.CurrentCollider.bounds.center;
    }

    protected override Vector3 GetColliderPosition()
    {
        return colliderPosition;
    }
}
