using System;
using UnityEngine;

[Serializable]
public class BulletStat : StatWithCharacterStat, StatHasMoveSpeed
{
    /* Member Variable */
    [SerializeField]
    [Tooltip("공격력")]
    private float attack = 0;              // 공격력 (>= 0)
    [SerializeField]
    [Tooltip("이동 속도")]
    private float moveSpeed = 0;           // 이동 속도 (>= 0)
    [SerializeField]
    [Tooltip("넉백")]
    private float knockBack = 0;           // 넉백 (>= 0)
    [SerializeField]
    [Tooltip("산탄")]
    private float scatter = 0;             // 산탄 (>= 0)
    [SerializeField]
    [Tooltip("관통 여부")]
    private bool isPenetrable = false;     // 관통 여부

    /* Getter & Setter */
    public float Attack => attack * characterStat.Attack;
    public float MoveSpeed => moveSpeed * characterStat.BulletSpeed;
    public float KnockBack => knockBack;
    public float Scatter => Math.Max(0, scatter - (scatter * characterStat.BulletScatterResistance));
    public bool IsPenetrable => isPenetrable;

    /* Has Move Speed */
    public float GetMoveSpeed() => MoveSpeed;
}