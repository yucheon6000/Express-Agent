using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootWithBullet : Shoot
{
    [Header("[Bullet]")]
    [SerializeField]
    [Tooltip("총알 프리팹")]
    protected GameObject bulletPrefab;
}
