using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : Ability
{
    [Header("[AttackAbility]")]
    [SerializeField]
    private Weapon weapon;
    [SerializeField]
    private Attack attack;

    public override void OnClickAbility()
    {
        weapon.AddAttack(attack);
        base.OnClickAbility();
    }
}
