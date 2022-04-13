using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncMultipleShoot : MultipleShoot
{
    [Header("[Shoot Stat]")]
    [SerializeField]
    private SyncMultipleShootStat shootStat;

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
        if (!shootStat.Breakable) return;
        if (coroutine == null) return;

        isShooting = false;
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator ShootRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(shootStat.ShootDeltaTime);
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

                if (shootStat.ShootDeltaTime > 0)
                    yield return wait;

                target = shoots[curIndex];
                target.StartShoot();
            }
        }

        isShooting = false;
        coroutine = null;
    }

    public override void SetCharacterStat(CharacterStat characterStat)
    {
        if (this.characterStat == characterStat) return;

        this.characterStat = characterStat;
        shootStat.SetCharacterStat(characterStat);

        foreach (Shoot shoot in shoots)
            shoot.SetCharacterStat(characterStat);
    }
}
