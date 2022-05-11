using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : CharacterCollision
{
    public static readonly string TAG = "Player Collision";

    private Player player;
    private PlayerColliderChanger colliderChanger;

    protected override void Awake()
    {
        base.Awake();
        gameObject.tag = TAG;
        player = character.GetComponent<Player>();
        colliderChanger = player.GetComponentInChildren<PlayerColliderChanger>();
    }

    private void Update()
    {
        UpdateColliderPosition();
    }

    private Vector3 colliderPosition;
    private void UpdateColliderPosition()
    {
        if (colliderChanger)
            colliderPosition = colliderChanger.CurrentCollider.bounds.center;
        else
            colliderPosition = transform.position;
    }

    protected override Vector3 GetColliderPosition()
    {
        return colliderPosition;
    }
}
