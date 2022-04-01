using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearShoot : Shoot
{
    [SerializeField]
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

    [Header("[Transform]")]
    [SerializeField]
    [Tooltip("총알 생성 위치")]
    private Transform spawnTransform;
    [SerializeField]
    [Tooltip("총알 이동 방향 위치")]
    private Transform directionTransform;
    [SerializeField]
    [Tooltip("총알 생성 방향")]
    private Transform spawnDirectionTransfrom;

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
        coroutine = null;
        StopCoroutine(coroutine);
    }

    private IEnumerator ShootRoutine()
    {
        int count = 0;
        WaitForSeconds wait = new WaitForSeconds(bulletDeltaTime);

        while (count < bulletCount)
        {
            if (count >= bulletIndexAtStart)
            {
                // 생성 위치                
                Vector3 spawnDir = (spawnDirectionTransfrom.position - spawnTransform.position).normalized;
                Vector3 spawnPosition = spawnTransform.position + (spawnDir * bulletDeltaDistance * count);

                // 이동 방향
                Vector2 moveDir = directionTransform.position - spawnTransform.position;

                // 총알 생성
                GameObject bulletGameObject = Instantiate(bulletPrefab);
                Bullet bullet = bulletGameObject.GetComponent<Bullet>();
                bullet.Init(spawnPosition, moveDir);
            }

            count++;

            if (bulletDeltaTime > 0 && count < bulletCount)
                yield return wait;
        }

        isShooting = false;
        coroutine = null;
    }
}
