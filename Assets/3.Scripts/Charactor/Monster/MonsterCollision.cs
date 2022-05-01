using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCollision : CharacterCollision
{
    public static readonly string TAG = "Monster Collision";

    [SerializeField]
    private Monster monster;

    protected override void Awake()
    {
        base.Awake();
        gameObject.tag = TAG;
        character = monster;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals(PlayerCollision.TAG))
        {
            PlayerCollision playerCol = other.GetComponent<PlayerCollision>();
            playerCol.Hit(
                character.CharacterStat.Attack,
                character.CharacterStat.KnockBack,
                transform.position
            );
            Debug.Log("HIT: Monster -> " + other.transform.parent.name);
        }
    }
}
