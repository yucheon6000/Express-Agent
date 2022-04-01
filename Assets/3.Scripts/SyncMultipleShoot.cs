using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncMultipleShoot : Shoot
{
    [SerializeField]
    [Tooltip("Shoot 사이 시간")]
    private float shootDeltaTime;

    [Header("[Shoots]")]
    [SerializeField]
    private Shoot[] shoots;

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
        WaitForSeconds wait = new WaitForSeconds(shootDeltaTime);
        Shoot target = null;
        int curIndex = 0;

        while (true)
        {
            if (target == null)
            {
                target = shoots[curIndex];
                target.StartShoot();
            }
            else
            {
                if (target.IsShooting)
                {
                    yield return null;
                    continue;
                }

                curIndex++;
                if (curIndex == shoots.Length)
                    break;

                if (shootDeltaTime > 0)
                    yield return wait;

                target = shoots[curIndex];
                target.StartShoot();
            }
        }

        isShooting = false;
        coroutine = null;
    }
}
