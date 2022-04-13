using System;
using UnityEngine;

[Serializable]
public class LinearShootStat : ShootStat
{
    [Tooltip("총알 개수")]
    private int bulletCount = 1;
    [SerializeField]
    [Tooltip("총알 간 사이 시간")]
    private float bulletDeltaTime;
    [SerializeField]
    [Tooltip("총알 생성 사이 거리")]
    private float bulletDeltaDistance;
    [SerializeField]
    [Tooltip("시작 총알 인덱스")]
    private int bulletIndexAtStart;

    public int BulletCount => bulletCount;
    public float BulletDeltaTime => bulletDeltaTime / characterStat.AttackSpeed;
    public float BulletDeltaDistance => bulletDeltaDistance;
    public int BulletIndexAtStart => bulletIndexAtStart;
}