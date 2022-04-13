using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shoot : MonoBehaviour, NeedCharacterStat
{
    protected CharacterStat characterStat = CharacterStat.Default;

    protected bool isShooting = false;
    public bool IsShooting => isShooting;

    public abstract void StartShoot();
    public abstract void StopShoot();

    public abstract void SetCharacterStat(CharacterStat characterStat);
}
