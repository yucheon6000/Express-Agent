using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncMultipleShoot : Shoot
{
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
        foreach (Shoot shoot in shoots)
            shoot.StartShoot();


        while (true)
        {
            bool flag = true;

            foreach (Shoot shoot in shoots)
                if (shoot.IsShooting)
                {
                    flag = false;
                    break;
                }

            if (!flag)
                yield return null;
            else
                break;
        }

        isShooting = false;
        coroutine = null;
    }
}
