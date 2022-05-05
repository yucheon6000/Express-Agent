using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLevelAbility : Ability
{
    [Header("[AttackLevelAbility]")]
    [SerializeField]
    private Attack attack;
    [SerializeField]
    private int level = 1;

    public override void OnClickAbility()
    {
        attack.IncreaseAttackLevel(level);
        base.OnClickAbility();
    }
}
