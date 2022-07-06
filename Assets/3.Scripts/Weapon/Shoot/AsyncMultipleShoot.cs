using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncMultipleShoot : MultipleShoot
{
    [Header("[Shoot Stat]")]
    [SerializeField]
    private ShootStat[] shootStats;
    private ShootStat shootStat;
    private Stepper<ShootStat> statStepper;

    private Coroutine coroutine;

    private void Awake()
    {
        statStepper = new Stepper<ShootStat>(shootStats);
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

    protected override void UpdateShootStat()
    {
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
