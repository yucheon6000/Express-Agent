using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : CharacterCollision
{
    public static readonly string TAG = "Player Collision";

    [SerializeField]
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        gameObject.tag = TAG;
        character = player;
    }
}
