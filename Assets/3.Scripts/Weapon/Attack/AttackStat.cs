using System;
using UnityEngine;

[Serializable]
public class AttackStat : StatWithCharacterStat
{
    public static AttackStat Default => new AttackStat();

    /* Member Variable */
    [SerializeField]
    [Tooltip("Shoot 사이 시간")]
    private float shootDeltaTime = 0;

    [SerializeField]
    [Tooltip("Shoot 시작 전 딜레이 시간")]
    private float shootDelayTimeAtStart = 0;

    /* Getter & Setter */
    public float ShootDeltaTime => shootDeltaTime / characterStat.AttackSpeed;
    public float ShootDelayTimeAtStart => shootDelayTimeAtStart / characterStat.AttackSpeed;
}