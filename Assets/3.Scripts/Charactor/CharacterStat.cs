using System;
using UnityEngine;

[Serializable]
public class CharacterStat : Stat, StatHasMoveSpeed
{
    public static CharacterStat Default => new CharacterStat();

    /* Member Variable */
    [SerializeField]
    [Tooltip("체력")]
    private int health = 0;                        // 체력(HP) (>= 0)
    [SerializeField]
    [Tooltip("이동 속도")]
    private float moveSpeed = 0;                   // 이동 속도 (>= 0)
    [SerializeField]
    [Tooltip("공격력")]
    private float attack = 0;                      // 공격력 (>= 1)
    [SerializeField]
    [Tooltip("넉백 저항")]
    private float knockBackResistance = 0;         // 넉백 저항 (>= 0, <= 1)  // 1이면 모두 저항
    [SerializeField]
    [Tooltip("공격 속도")]
    private float attackSpeed = 1;                 // 공격 속도 (>= 1)
    [SerializeField]
    [Tooltip("탄환 수 단계")]
    private int bulletAmountStep = 0;              // 탄환 수 단계 (>= 0)
    [SerializeField]
    [Tooltip("탄환 속도")]
    private float bulletSpeed = 1;                 // 탄환 속도 (>= 1)
    [SerializeField]
    [Tooltip("탄환 산탄 저항")]
    private float bulletScatterResistance = 0;     // 탄환 산탄 저항 (>= 0, <= 1)  // 1이면 모두 저항

    /* Getter & Setter */
    public int Health { get { return health; } set { health = Math.Max(0, value); } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = Math.Max(0, value); } }
    public float Attack { get { return attack; } set { attack = Math.Max(1, value); } }
    public float KnockBackResistance { get { return knockBackResistance; } set { knockBackResistance = Math.Clamp(value, 0f, 1f); } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = Math.Max(1, value); } }
    public int BulletAmountStep { get { return bulletAmountStep; } set { bulletAmountStep = Math.Max(0, value); } }
    public float BulletSpeed { get { return bulletSpeed; } set { bulletSpeed = Math.Max(1, value); } }
    public float BulletScatterResistance { get { return bulletScatterResistance; } set { bulletScatterResistance = Math.Clamp(value, 0f, 1f); } }

    /* Has Move Speed */
    public float GetMoveSpeed() => MoveSpeed;
}