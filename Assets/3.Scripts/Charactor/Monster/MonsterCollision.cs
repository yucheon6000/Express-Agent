using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCollision : CharacterCollision
{
    public static readonly string TAG = "Monster Collision";

    private Monster monster;

    protected override void Awake()
    {
        base.Awake();
        gameObject.tag = TAG;
        monster = character.GetComponent<Monster>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (monster.IsDead) return;

        if (other.tag.Equals(PlayerCollision.TAG))
        {
            PlayerCollision playerCol = other.GetComponent<PlayerCollision>();
            playerCol.Hit(
                character.CharacterStat.Attack,
                character.CharacterStat.KnockBack,
                transform.position
            );
        }
    }
}
