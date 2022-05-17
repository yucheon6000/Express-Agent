using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBullet : Bullet
{
    public override void Init(BulletInitInfo info)
    {
        base.Init(info);

        Vector3 newMoveDir = ConvertMoveDirection(info.moveDirection);
        RotateBullet(newMoveDir);

        if (movement)
            movement.SetMoveDirection(newMoveDir);
    }
}
