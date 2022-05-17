using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableBullet : StraightBullet
{
    [Header("[Shootable Bullet]")]
    [SerializeField]
    private Shoot shoot;

    [Header("[Trigger]")]
    [SerializeField]
    private bool onHitEnemy = false;
    [SerializeField]
    private bool onHitObstacle = false;

    public override void Init(BulletInitInfo info)
    {
        base.Init(info);
        shoot.StopShoot();
    }

    protected override void HitEnemy(CharacterCollision enemy)
    {
        base.HitEnemy(enemy);

        if (onHitEnemy)
            StartShoot();
    }

    protected override void HitObstacle()
    {
        base.HitObstacle();

        if (onHitObstacle)
            StartShoot();
    }

    private void StartShoot()
    {
        shoot.SetCharacterStat(bulletStat.CharacterStat);
        shoot.SetAttackLevel(info.attackLevel);
        shoot.StartShoot();
    }

    protected override void OnDisable()
    {
        shoot.StopShoot();
        base.OnDisable();
    }
}
