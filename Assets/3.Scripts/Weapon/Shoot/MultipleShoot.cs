using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MultipleShoot : Shoot
{
    [Header("[Shoots]")]
    [SerializeField]
    protected Shoot[] shoots;
}
