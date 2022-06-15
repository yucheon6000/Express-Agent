using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearShoot : ShootWithBullet
{
    [Header("[Shoot Stats]")]
    [SerializeField]
    private LinearShootStat[] shootStats;
    private LinearShootStat shootStat;
    private Stepper<LinearShootStat> statStepper;

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

    private void Awake()
    {
        statStepper = new Stepper<LinearShootStat>(shootStats);
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
        WaitForSeconds wait = new WaitForSeconds(shootStat.BulletDeltaTime);
        int count = 0;

        if (shootStat.BulletDeltaTime == 0) PlayAudio();

        while (count < shootStat.BulletCount)
        {
            if (count >= shootStat.BulletIndexAtStart)
            {
                // 생성 위치                
                Vector3 spawnDir = (spawnDirectionTransfrom.position - spawnTransform.position).normalized;
                Vector3 spawnPosition =
                    spawnTransform.position
                    + (spawnDir * shootStat.BulletStartDistance)
                    + (spawnDir * shootStat.BulletDeltaDistance * count);

                // 이동 방향
                Vector3 directionPoint = mouseDirectionMode ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : directionTransform.position;
                Vector2 moveDir = directionPoint - spawnTransform.position;

                // 총알 생성
                Bullet bullet = ObjectPooler.SpawnFromPool<Bullet>(bulletPrefab.name, spawnPosition);
                bullet.Init(new BulletInitInfo(transform, moveDir, characterStat, attackLevel));
            }

            count++;

            if (shootStat.BulletDeltaTime > 0 && count < shootStat.BulletCount)
            {
                PlayAudio();
                yield return wait;
            }
        }

        isShooting = false;
        coroutine = null;
    }

    protected override void UpdateShootStat()
    {
        if (debugMode)
            statStepper = new Stepper<LinearShootStat>(shootStats);

        shootStat = statStepper.GetStep(attackLevel);
    }

    public override void SetCharacterStat(CharacterStat characterStat)
    {
        if (this.characterStat == characterStat) return;

        this.characterStat = characterStat;
        shootStat.SetCharacterStat(characterStat);
    }
}
