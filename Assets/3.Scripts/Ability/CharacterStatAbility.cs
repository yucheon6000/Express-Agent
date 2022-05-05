using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStatType
{
    Health, MoveSpeed, Attack, KnockBack, KnockBackResistance,
    AttackSpeed, BulletSpeed, BulletScatterResistance,
    CurrentHp
}

public class CharacterStatAbility : Ability
{
    [Header("[CharacterStatAbility]")]
    [SerializeField]
    private Character character;
    [SerializeField]
    private CharacterStatType targetStat;
    [SerializeField]
    private bool percent = false;
    [SerializeField]
    private float amount = 0;

    public override void OnClickAbility()
    {
        switch (targetStat)
        {
            case CharacterStatType.Health:
                if (character.CharacterStat.CharacterType == CharacterType.Player)
                    Player.IncreaseMaxHp(percent ? Player.MaxHp * (amount / 100) : amount);
                else
                    character.CharacterStat.Health += percent ? (int)(character.CharacterStat.Health * (amount / 100)) : (int)amount;
                break;

            case CharacterStatType.CurrentHp:
                if (character.CharacterStat.CharacterType == CharacterType.Player)
                    Player.IncreaseCurrentHp(percent ? Player.MaxHp * (amount / 100) : amount);
                else
                    character.IncreaseHp(percent ? (int)(character.CharacterStat.Health * (amount / 100)) : (int)amount);
                break;

            case CharacterStatType.MoveSpeed:
                character.CharacterStat.MoveSpeed += percent ? character.CharacterStat.MoveSpeed * (amount / 100) : amount;
                break;

            case CharacterStatType.Attack:
                character.CharacterStat.Attack += percent ? character.CharacterStat.Attack * (amount / 100) : amount;
                break;

            case CharacterStatType.KnockBack:
                character.CharacterStat.KnockBack += percent ? character.CharacterStat.KnockBack * (amount / 100) : amount;
                break;

            case CharacterStatType.KnockBackResistance:
                character.CharacterStat.KnockBackResistance += percent ? character.CharacterStat.KnockBackResistance * (amount / 100) : amount;
                break;

            case CharacterStatType.AttackSpeed:
                character.CharacterStat.AttackSpeed += percent ? character.CharacterStat.AttackSpeed * (amount / 100) : amount;
                break;

            case CharacterStatType.BulletSpeed:
                character.CharacterStat.BulletSpeed += percent ? character.CharacterStat.BulletSpeed * (amount / 100) : amount;
                break;

            case CharacterStatType.BulletScatterResistance:
                character.CharacterStat.BulletScatterResistance += percent ? character.CharacterStat.BulletScatterResistance * (amount / 100) : amount;
                break;
        }
        
        base.OnClickAbility();
    }
}
