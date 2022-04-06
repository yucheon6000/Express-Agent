using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShoot : Shoot
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


    [Header("[Transform]")]
    [SerializeField]
    [Tooltip("총알 생성 위치")]
    private Transform spawnCircleCenterTransform;
    [SerializeField]
    [Tooltip("총알 생성 원 각도 기준 위치")]
    private Transform spawnCircleStandardTransform;
    [SerializeField]

    private Coroutine coroutine;

    [ContextMenu("Start Shoot")]
    public override void StartShoot()
    {
        if (isShooting) return;

        isShooting = true;
        coroutine = StartCoroutine(ShootRoutine());
    }

    [ContextMenu("Stop Shoot")]
    public override void StopShoot()
    {
        if (!isShooting) return;
        if (!breakable) return;
        if (coroutine == null) return;

        isShooting = false;
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator ShootRoutine()
    {
        int count = 0;
        WaitForSeconds wait = new WaitForSeconds(bulletDeltaTime);

        while (count < bulletCount)
        {
            if (count >= bulletIndexAtStart)
            {
                // 기준(시작) 각도 (standardAngle)
                Vector3 standardDir = spawnCircleStandardTransform.position - spawnCircleCenterTransform.position;
                float standardAngle = Mathf.Atan2(standardDir.y, standardDir.x) * Mathf.Rad2Deg;
                standardAngle = (standardAngle + bulletStartAngle + 360) % 360;

                // 생성원 중앙에서 생성 위치쪽을 바라보는 방향 (spawnDir)
                float spawnAngle = (standardAngle + (bulletDeltaAngle * count)) % 360;
                float theta = spawnAngle * Mathf.PI / 180;
                Vector3 spawnDir = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));

                // 생성 위치
                Vector3 spawnPosition = spawnCircleCenterTransform.position + (spawnDir * spawnCircleRadius);

                // 총알 생성
                Bullet bullet = ObjectPooler.SpawnFromPool<Bullet>(bulletPrefab.name, spawnPosition);
                bullet.Init(spawnDir);
            }

            count++;

            if (bulletDeltaTime > 0 && count < bulletCount)
                yield return wait;
        }

        isShooting = false;
        coroutine = null;
    }
}
