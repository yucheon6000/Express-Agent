using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncMultipleShoot : MultipleShoot
{
    [Header("[Shoot Stat]")]
    [SerializeField]
    private SyncMultipleShootStat[] shootStats;
    private SyncMultipleShootStat shootStat;
    private Stepper<SyncMultipleShootStat> statStepper;

    private Coroutine coroutine;

    private void Awake()
    {
        statStepper = new Stepper<SyncMultipleShootStat>(shootStats);
        UpdateShootStat();
    }

    [ContextMenu("Start Shoot")]
    public override void StartShoot()
    {
        if (isShooting) return;

        UpdateShootStat();

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

    protected override void UpdateShootStat()
    {
        if (debugMode)
            statStepper = new Stepper<SyncMultipleShootStat>(shootStats);

        shootStat = statStepper.GetStep(attackLevel);
    }

    public override void SetAttackLevel(int level)
    {
        base.SetAttackLevel(level);
        foreach (Shoot shoot in shoots)
            shoot.SetAttackLevel(level);
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
