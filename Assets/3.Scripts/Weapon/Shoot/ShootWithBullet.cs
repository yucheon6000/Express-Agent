using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootWithBullet : Shoot
{
    [Header("[Bullet]")]
    [SerializeField]
    [Tooltip("총알 프리팹")]
    protected GameObject bulletPrefab;

    [SerializeField]
    protected bool bulletStep;
    [SerializeField]
    protected Stepper<GameObject> bulletPrefabStepper;

    public override void SetAttackLevel(int level)
    {
        base.SetAttackLevel(level);

        if (bulletStep)
            bulletPrefab = bulletPrefabStepper.GetStep(attackLevel);
    }
}
