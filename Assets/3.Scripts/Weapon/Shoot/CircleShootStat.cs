using System;
using UnityEngine;

[Serializable]
public class CircleShootStat : ShootStat
{
    [SerializeField]
    [Tooltip("총알 개수")]
    private int bulletCount = 1;
    [SerializeField]
    [Tooltip("총알 간 사이 시간")]
    private float bulletDeltaTime;
    [SerializeField]
    [Tooltip("총알 생성 시작 각도")]
    private float bulletStartAngle;
    [SerializeField]
    [Tooltip("총알 생성 사이 각도")]
    private float bulletDeltaAngle;
    [SerializeField]
    [Tooltip("시작 총알 인덱스")]
    private int bulletIndexAtStart;
    [SerializeField]
    [Tooltip("생성 원 반지름")]
    private float spawnCircleRadius;

    public int BulletCount => bulletCount;
    public float BulletDeltaTime => bulletDeltaTime / characterStat.AttackSpeed;
    public float BulletStartAngle => bulletStartAngle;
    public float BulletDeltaAngle => bulletDeltaAngle;
    public int BulletIndexAtStart => bulletIndexAtStart;
    public float SpawnCircleRadius => spawnCircleRadius;
}