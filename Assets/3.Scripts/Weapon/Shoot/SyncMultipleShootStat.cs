using System;
using UnityEngine;

[Serializable]
public class SyncMultipleShootStat : ShootStat
{
    [SerializeField]
    [Tooltip("Shoot 사이 시간")]
    private float shootDeltaTime;

    public float ShootDeltaTime => shootDeltaTime / characterStat.AttackSpeed;
}