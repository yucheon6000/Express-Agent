using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shoot : MonoBehaviour
{
    [Header("[Bullet]")]
    [SerializeField]
    [Tooltip("총알 프리팹")]
    protected GameObject bulletPrefab;

    [Header("[Property]")]
    [SerializeField]
    [Tooltip("중간에 정지가 가능한지 여부")]
    protected bool breakable = true;

    protected bool isShooting = false;
    public bool IsShooting => isShooting;

    public abstract void StartShoot();
    public abstract void StopShoot();
}
