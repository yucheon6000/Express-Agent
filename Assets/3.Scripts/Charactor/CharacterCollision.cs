using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : Collision
{
    protected Character character;


    protected override void Awake()
    {
        base.Awake();
        character = GetComponentInParent<Character>();
    }

    public virtual void Hit(float attack, float knockBack, Vector3 hitPosition)
    {
        character.Hit(attack, knockBack, hitPosition);
    }
}
