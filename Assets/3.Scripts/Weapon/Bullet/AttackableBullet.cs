using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackableBullet : StraightBullet
{
    [Header("[Attackable Bullet]")]
    [SerializeField]
    private Attack attack;

    [Header("[Trigger]")]
    [SerializeField]
    private bool onInit = false;
    [SerializeField]
    private bool onHitEnemy = false;
    [SerializeField]
    private bool onHitObstacle = false;

    public override void Init(BulletInitInfo info)
    {
        base.Init(info);

        if (onInit)
            StartAttack();
    }

    protected override void HitEnemy(CharacterCollision enemy)
    {
        base.HitEnemy(enemy);

        if (onHitEnemy)
            StartAttack();
    }

    protected override void HitObstacle()
    {
        base.HitObstacle();

        if (onHitObstacle)
            StartAttack();
    }

    private void StartAttack()
    {
        attack.SetCharacterStat(bulletStat.CharacterStat);
        attack.SetAttackLevel(info.attackLevel);
        attack.StartAttack();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        attack.StopAttack();
    }
}
