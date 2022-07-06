using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBullet : PenetrativeBullet
{
    protected override void Update()
    {
        base.Update();

        transform.position = info.ownerTransform.position;
    }
}
