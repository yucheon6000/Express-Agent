using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangBullet : PenetrativeBullet
{
    private enum State { Forward, Turn, Backward }

    [Header("[BoomerangBullet]")]
    [SerializeField]
    float forwardTime = 3;
    float forwardTimer = 0;
    private Vector3 moveDir;

    public override void Init(BulletInitInfo info)
    {
        base.Init(info);
        moveDir = movement.MoveDirection;
        movement.enabled = false;
        forwardTimer = 0;
    }

    protected override void Update()
    {
        base.Update();

        forwardTimer += Time.deltaTime;
        float percent = (forwardTimer / forwardTime);
        float sin;

        // 뒤로
        if (percent > 1.5f)
        {
            percent = 1.5f;
            sin = Mathf.Sin(percent * Mathf.PI);
            sin *= percent;
        }
        // 앞으로
        else
            sin = Mathf.Sin(percent * Mathf.PI);


        transform.position += moveDir * sin * bulletStat.MoveSpeed * Time.deltaTime;
    }
}
