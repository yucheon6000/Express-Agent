using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : Collision
{
    protected Character character;


    protected override void Awake()
    {
        base.Awake();
    }

    public virtual void Hit(float attack)
    {
        character.Hit(attack);
    }

    public virtual void KnockBack(Vector2 hitPosition, float knockBack)
    {
        character.KnockBack(hitPosition, knockBack);
    }
}
